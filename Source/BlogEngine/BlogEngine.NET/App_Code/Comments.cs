namespace App_Code
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Script.Services;
    using System.Web.Services;
    using BlogEngine.Core;
    using BlogEngine.Core.Json;
    using BlogEngine.Core.Web.Extensions;

    /// <summary>
    /// The comments.
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [ScriptService]
    public class Comments : WebService
    {
        #region Constants and Fields

        /// <summary>
        ///     JSON object that will be return back to client
        /// </summary>
        private readonly JsonResponse response;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes static members of the <see cref = "Comments" /> class.
        /// </summary>
        static Comments()
        {
            CurrentPage = 1;
            LastPage = 1;
            CommCnt = 1;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref = "Comments" /> class.
        /// </summary>
        public Comments()
        {
            this.response = new JsonResponse();
        }

        #endregion

        /// <summary>
        ///     Gets or sets the comm CNT.
        /// </summary>
        /// <value>The comm CNT.</value>
        protected static int CommCnt { get; set; }

        /// <summary>
        ///     Gets or sets the last page.
        /// </summary>
        /// <value>The last page.</value>
        protected static int LastPage { get; set; }

        /// <summary>
        ///     Gets or sets the current page.
        /// </summary>
        /// <value>The current page.</value>
        protected static int CurrentPage { get; set; }

        /// <summary>
        /// List of comments based on type for a single page.
        /// </summary>
        /// <param name="commentType">
        /// The comment type.
        /// </param>
        /// <param name="pageSize">
        /// The page size.
        /// </param>
        /// <param name="page">
        /// The current page.
        /// </param>
        /// <returns>
        /// A list of JSON comments.
        /// </returns>
        [WebMethod]
        public List<JsonComment> GetComments(CommentType commentType, int pageSize, int page)
        {
            try
            {
                Rights right;
                switch (commentType)
                {
                    case CommentType.Approved:
                        right = Rights.ViewPublicComments;
                        break;
                    case CommentType.Pending:
                        right = Rights.ViewUnmoderatedComments;
                        break;
                    case CommentType.Pingback:
                        right = Rights.ViewPublicComments;
                        break;
                    case CommentType.Spam:
                        right = Rights.ModerateComments;
                        break;
                    default:
                        throw new Exception("Cannot determine comment type.");
                }

                if (!Security.IsAuthorizedTo(right))
                {
                    throw new UnauthorizedAccessException();
                }

                return JsonComments.GetComments(commentType, pageSize, page);
            }
            catch (Exception ex)
            {
                Utils.Log("Api.Comments.GetComments", ex);
                throw new Exception("Error on Api.Comments.GetComments.  Check the log for more info.");
            }
        }

        /// <summary>
        /// Reject selected comments
        /// </summary>
        /// <param name="vals">
        /// Array of comments
        /// </param>
        /// <returns>
        /// Json response
        /// </returns>
        [WebMethod]
        public JsonResponse Reject(string[] vals)
        {
            response.Success = false;

            if (!Security.IsAuthorizedTo(Rights.ModerateComments))
            {
                response.Message = Resources.labels.notAuthorized;
                return response;
            }

            if (string.IsNullOrEmpty(vals[0]))
            {
                return response;
            }

            try
            {
                foreach (var p in Post.Posts.ToArray())
                {
                    foreach (var c in from c in p.Comments.ToArray() from t in vals where c.Id == new Guid(t) select c)
                    {
                        if (BlogSettings.Instance.AddIpToBlacklistFilterOnRejection)
                        {
                            CommentHandlers.AddIpToFilter(c.IP, true);
                        }

                        CommentHandlers.ReportMistake(c);

                        c.ModeratedBy = Security.CurrentUser.Identity.Name;
                        p.DisapproveComment(c);
                    }
                }
            }
            catch (Exception ex)
            {
                Utils.Log("Api.Comments.Reject", ex);
                response.Message = Resources.labels.errorRejectingComment;
                return response;
            }

            response.Success = true;
            response.Message = Resources.labels.selectedCommentRejected;
            return response;
        }

        /// <summary>
        /// Restore selected comments
        /// </summary>
        /// <param name="vals">
        /// Array of comments
        /// </param>
        /// <returns>
        /// Json response
        /// </returns>
        [WebMethod]
        public JsonResponse Approve(string[] vals)
        {
            this.response.Success = false;

            if (!Security.IsAuthorizedTo(Rights.ModerateComments))
            {
                response.Message = Resources.labels.notAuthorized;
                return response;
            }

            if (string.IsNullOrEmpty(vals[0]))
            {
                return response;
            }

            try
            {
                foreach (var p in Post.Posts.ToArray())
                {
                    foreach (var c in from c in p.Comments.ToArray() from t in vals where c.Id == new Guid(t) select c)
                    {
                        if (BlogSettings.Instance.AddIpToWhitelistFilterOnApproval)
                        {
                            CommentHandlers.AddIpToFilter(c.IP, false);
                        }

                        CommentHandlers.ReportMistake(c);

                        c.ModeratedBy = Security.CurrentUser.Identity.Name;
                        p.ApproveComment(c);
                    }
                }
            }
            catch (Exception ex)
            {
                Utils.Log("Api.Comments.Approve", ex);
                response.Message = string.Format(Resources.labels.errorApprovingComment, vals[0]);
                return response;
            }

            response.Success = true;
            response.Message = Resources.labels.selectedCommentRestored;
            return response;
        }

        /// <summary>
        /// Delete selected comments
        /// </summary>
        /// <param name="vals">
        /// Array of comments
        /// </param>
        /// <returns>
        /// Json response
        /// </returns>
        [WebMethod]
        public JsonResponse Delete(string[] vals)
        {
            response.Success = false;
            Comment tmpComment = null;

            if (!Security.IsAuthorizedTo(Rights.ModerateComments))
            {
                response.Message = Resources.labels.notAuthorized;
                return response;
            }

            if (string.IsNullOrEmpty(vals[0]))
            {
                return response;
            }

            try
            {
                var tmp = new List<Comment>();

                foreach (var post in Post.Posts)
                {
                    var post1 = post;
                    tmp.AddRange(
                        vals.Select(t => post1.Comments.Find(c => c.Id == new Guid(t))).Where(
                            comment => comment != null));
                }

                foreach (var c in tmp)
                {
                    tmpComment = c;
                    RemoveComment(tmpComment);
                }
            }
            catch (Exception ex)
            {
                Utils.Log(string.Format("Api.Comments.Delete: error deleting comment {0} by {1}", tmpComment.Teaser, tmpComment.Author), ex);
                response.Message = string.Format(Resources.labels.couldNotDeleteComment, ex.Message);
                return response;
            }

            response.Success = true;
            response.Message = Resources.labels.selectedCommentDeleted;
            return response;
        }

        /// <summary>
        /// Delete all spam comments
        /// </summary>
        /// <returns>
        /// Json response
        /// </returns>
        [WebMethod]
        public JsonResponse DeleteAll()
        {
            response.Success = false;

            if (!Security.IsAuthorizedTo(Rights.ModerateComments))
            {
                response.Message = Resources.labels.notAuthorized;
                return response;
            }

            try
            {
                DeleteAllComments();
                this.response.Success = true;
                this.response.Message = Resources.labels.commentsDeleted;
                return this.response;
            }
            catch (Exception ex)
            {
                Utils.Log(string.Format("Api.Comments.DeleteAll: {0}", ex.Message));
                response.Message = string.Format(Resources.labels.couldNotDeleteComments, ex.Message);
                return response;
            }

        }

        /// <summary>
        /// Save the comment.
        /// </summary>
        /// <param name="id">
        /// The comment id.
        /// </param>
        /// <param name="author">
        /// The author.
        /// </param>
        /// <param name="email">
        /// The email.
        /// </param>
        /// <param name="website">
        /// The website.
        /// </param>
        /// <param name="cont">
        /// The content.
        /// </param>
        /// <returns>
        /// A JSON Comment.
        /// </returns>
        [WebMethod]
        public static JsonComment SaveComment(string id, string author, string email, string website, string cont)
        {

            // There really needs to be validation here so people aren't just posting willy-nilly
            // as anyone they want.

            if (!Security.IsAuthorizedTo(Rights.CreateComments))
            {
                throw new System.Security.SecurityException("Can not create comment");
            }


            var gId = new Guid(id);
            var jc = new JsonComment();

            foreach (var p in Post.Posts.ToArray())
            {
                foreach (var c in p.Comments.Where(c => c.Id == gId).ToArray())
                {
                    c.Author = author;
                    c.Email = email;
                    c.Website = new Uri(website);
                    c.Content = cont;

                    // need to mark post as "dirty"
                    p.DateModified = DateTime.Now;
                    p.Save();

                    return JsonComments.GetComment(gId);
                }
            }

            return jc;
        }

        /// <summary>
        /// Removes the comment.
        /// </summary>
        /// <param name="comment">
        /// The comment.
        /// </param>
        protected void RemoveComment(Comment comment)
        {
            Post post = comment.Parent as Post;
            if (post != null)
            {
                post.RemoveComment(comment);
            }
        }

        /// <summary>
        /// Deletes all comments.
        /// </summary>
        protected void DeleteAllComments()
        {
            if (Post.Posts.Count <= 0)
            {
                return;
            }

            // loop backwards to avoid "collection was modified" error
            for (var i = Post.Posts.Count - 1; i >= 0; i--)
            {
                if (Post.Posts[i].Comments.Count <= 0)
                {
                    continue;
                }

                for (var j = Post.Posts[i].Comments.Count - 1; j >= 0; j--)
                {
                    var comment = Post.Posts[i].Comments[j];

                    // spam comments should never have children but
                    // be on a safe side insure we won't create
                    // orphan comment with deleted parent
                    if (!comment.IsApproved && comment.Comments.Count == 0)
                    {
                        RemoveComment(comment);
                    }
                }
            }
        }
    }
}