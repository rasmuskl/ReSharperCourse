namespace BlogEngine.Core.Json
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Comment type.
    /// </summary>
    public enum CommentType
    {
        /// <summary>
        ///     Pending Comment Type
        /// </summary>
        Pending,

        /// <summary>
        ///     Approved Comment Type
        /// </summary>
        Approved,

        /// <summary>
        ///     Pingbacks and trackbacks Comment Type
        /// </summary>
        Pingback,

        /// <summary>
        ///     Spam Comment Type
        /// </summary>
        Spam
    }

    /// <summary>
    /// List of comments
    /// </summary>
    public static class JsonComments
    {
        /// <summary>
        /// The current page.
        /// </summary>
        private static int currentPage = 1;    

        /// <summary>
        /// The comm cnt.
        /// </summary>
        private static int commCnt;

        /// <summary>
        /// List of comments based on type for a single page.
        /// </summary>
        /// <param name="commentType">
        /// The comment type.
        /// </param>
        /// <param name="page">
        /// The current page.
        /// </param>
        /// <returns>
        /// A list of JSON comments.
        /// </returns>
        public static List<JsonComment> GetComments(CommentType commentType, int page)
        {
            return GetComments(commentType, BlogSettings.Instance.CommentsPerPage, page);
        }

        /// <summary>
        /// List of comments based on type for a single page.
        /// </summary>
        /// <param name="commentType">
        /// The comment type.
        /// </param>
        /// <param name="pageSize">
        /// The number of comments per page.
        /// </param>
        /// <param name="page">
        /// The current page.
        /// </param>
        /// <returns>
        /// A list of JSON comments.
        /// </returns>
        public static List<JsonComment> GetComments(CommentType commentType, int pageSize, int page)
        {
            var cntTo = page * pageSize;
            var cntFrom = cntTo - pageSize;
            var cnt = 0;

            var allComments = new List<Comment>();
            var pageComments = new List<JsonComment>();

            foreach (var p in Post.Posts)
            {
                switch (commentType)
                {
                    case CommentType.Pending:
                        allComments.AddRange(p.NotApprovedComments);
                        break;
                    case CommentType.Pingback:
                        allComments.AddRange(p.Pingbacks);
                        break;
                    case CommentType.Spam:
                        allComments.AddRange(p.SpamComments);
                        break;
                    default:
                        allComments.AddRange(p.ApprovedComments);
                        break;
                }
            }

            allComments.Sort((x, y) => DateTime.Compare(y.DateCreated, x.DateCreated));

            // TODO: find a way to better handle deleting last comment on the last page
            // below is not working properly; after moving to the next page and deleting
            // another comment there, things get nasty
            if (allComments.Count > 0 && allComments.Count == cntFrom && page > 1)
            {
                // removed last comment(s) on the page
                // need to shift one page back

                //var context = System.Web.HttpContext.Current;
                //if (context.Request.Cookies["CommentPagerCurrentPage"] != null)
                //    context.Request.Cookies["CommentPagerCurrentPage"].Value = (page - 1).ToString();

                //return GetComments(commentType, page - 1);
            }

            foreach (var c in allComments)
            {
                cnt++;
                if (cnt <= cntFrom || cnt > cntTo)
                {
                    continue;
                }

                pageComments.Add(CreateJsonCommentFromComment(c));
            }

            currentPage = page;
            commCnt = cnt;

            return pageComments;
        }

        /// <summary>
        /// Single commnet by ID
        /// </summary>
        /// <param name="id">
        /// Comment id
        /// </param>
        /// <returns>
        /// A JSON Comment
        /// </returns>
        public static JsonComment GetComment(Guid id)
        {
            return (from p in Post.Posts
                    from c in p.AllComments
                    where c.Id == id
                    select CreateJsonCommentFromComment(c)).FirstOrDefault();
        }

        private static JsonComment CreateJsonCommentFromComment(Comment c)
        {
            var jc = new JsonComment
                {
                    Id = c.Id,
                    Email = c.Email,
                    Author = c.Author,
                    Title = c.Title,
                    Teaser = c.Teaser,
                    Website = c.Website == null ? "" : c.Website.ToString(),
                    AuthorAvatar = c.Avatar,
                    Content = c.Content,
                    Ip = c.IP,
                    Date = c.DateCreated.ToString("dd MMM yyyy"),
                    Time = c.DateCreated.ToString("t")
                };
            return jc;
        }

        /// <summary>
        /// Builds pager control for comments page
        /// </summary>
        /// <param name="page">
        /// Current page
        /// </param>
        /// <param name="srvs">
        /// The Srvs..
        /// </param>
        /// <returns>
        /// HTML with next and previous buttons
        /// </returns>
        public static string GetPager(int page)
        {
            if (commCnt == 0)
            {
                return string.Empty;
            }

            var prvLnk = string.Empty;
            var nxtLnk = string.Empty;
            var firstLnk = string.Empty;
            var lastLnk = string.Empty;

            const string linkFormat = "<a href=\"#\" id=\"{0}\" onclick=\"return LoadComments({1});\" class=\"{0}\"></a>";

            var PageSize = BlogSettings.Instance.CommentsPerPage;
            var pgs = Convert.ToDecimal(commCnt) / Convert.ToDecimal(PageSize);
            var p = pgs - (int)pgs;
            var lastPage = p > 0 ? (int)pgs + 1 : (int)pgs;

            var cntFrom = ((page * PageSize) - (PageSize - 1));
            var cntTo = (page * PageSize);
            
            // adjust for the last (or single) page
            if (commCnt < cntTo) cntTo = commCnt;

            // when last comment on the last page deleted
            // this will reset "from" counter
            if (cntFrom > cntTo) cntFrom = cntFrom - PageSize;

            if (commCnt > 0 && commCnt == cntFrom && page > 1)
            {
                // removed last comment(s) on the page
                // need to shift one page back
                //return GetPager(page - 1);
            }

            var currentScope = cntFrom + " - " + cntTo;

            var pageLink = string.Format("<span>Showing {0} of {1}</span>", currentScope, commCnt);

            if (currentPage > 1)
            {
                prvLnk = string.Format(linkFormat, "prevLink", currentPage - 1);
                firstLnk = string.Format(linkFormat, "firstLink", 1);
            }

            if (page < lastPage)
            {
                nxtLnk = string.Format(linkFormat, "nextLink", currentPage + 1);
                lastLnk = string.Format(linkFormat, "lastLink", lastPage);
            }

            return firstLnk + prvLnk + pageLink + nxtLnk + lastLnk;
        }
    }
}