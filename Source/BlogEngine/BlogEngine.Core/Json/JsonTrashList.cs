namespace BlogEngine.Core.Json
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Web.Navigation;

    /// <summary>
    /// Type of deleted object
    /// </summary>
    public enum TrashType
    {
        /// <summary>
        ///     Any deleted object
        /// </summary>
        All,

        /// <summary>
        ///     Deleted comment
        /// </summary>
        Comment,

        /// <summary>
        ///     Deleted post
        /// </summary>
        Post,

        /// <summary>
        ///     Deleted page
        /// </summary>
        Page
    }

    /// <summary>
    /// List of deleted objects
    /// </summary>
    public class JsonTrashList
    {
        static int listCount = 0;
        static int currentPage = 1;   

        /// <summary>
        /// Paged list of deleted objects
        /// </summary>
        /// <param name="trashType">Type of delted object</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="page">Page number</param>
        /// <returns></returns>
        public static List<JsonTrash> GetTrash(TrashType trashType, int page)
        {
            var comments = new List<Comment>();
            var posts = new List<Post>();
            var pages = new List<Page>();

            var trashList = new List<JsonTrash>();
            currentPage = page;

            // comments
            if (trashType == TrashType.All || trashType == TrashType.Comment)
            {
                foreach (var p in Post.Posts)
                {
                    comments.AddRange(p.DeletedComments);
                }
            }

            if (comments.Count > 0)
            {
                foreach (var c in comments)
                {
                    JsonTrash t1 = new JsonTrash { 
                        Id = c.Id, 
                        Title = c.Title, 
                        ObjectType = "Comment",
                        Date = c.DateCreated.ToString("dd MMM yyyy"), 
                        Time = c.DateCreated.ToString("t") };

                    trashList.Add(t1);
                }
            }

            // posts
            if (trashType == TrashType.All || trashType == TrashType.Post)
            {
                posts = (from x in Post.DeletedPosts orderby x.DateCreated descending select x).ToList();
            }

            if (posts.Count > 0)
            {
                foreach (var p in posts)
                {
                    JsonTrash t2 = new JsonTrash
                    {
                        Id = p.Id,
                        Title = System.Web.HttpContext.Current.Server.HtmlEncode(p.Title),
                        ObjectType = "Post",
                        Date = p.DateCreated.ToString("dd MMM yyyy"),
                        Time = p.DateCreated.ToString("t")
                    };

                    trashList.Add(t2);
                }
            }

            // pages
            if (trashType == TrashType.All || trashType == TrashType.Page)
            {
                pages = (from x in Page.DeletedPages orderby x.DateCreated descending select x).ToList();
            }

            if (pages.Count > 0)
            {
                foreach (var p in pages)
                {
                    JsonTrash t3 = new JsonTrash
                    {
                        Id = p.Id,
                        Title = System.Web.HttpContext.Current.Server.HtmlEncode(p.Title),
                        ObjectType = "Page",
                        Date = p.DateCreated.ToString("dd MMM yyyy"),
                        Time = p.DateCreated.ToString("t")
                    };

                    trashList.Add(t3);
                }
            }

            var pageSize = 20;
            listCount = trashList.Count;

            if (trashList.Count < pageSize)
                return trashList;

            var skip = page == 1 ? 0 : page * pageSize - pageSize;
            var trashPage = trashList.Skip(skip).Take(pageSize).ToList();

            return trashPage;
        }
        
        public static string GetPager(int page)
        {
            if (listCount == 0)
                return string.Empty;

            IPager pager = new Pager(page, BlogConfig.GenericPageSize, listCount);

            return pager.Render(page, "LoadTrash(null,{1})");
        }

        /// <summary>
        /// If trash is empty.
        /// </summary>
        /// <returns>True if empty.</returns>
        public static bool IsTrashEmpty()
        {
            foreach (var p in Post.Posts)
            {
                if(p.DeletedComments.Count > 0) return false;
            }
            if (Post.DeletedPosts.Count > 0) return false;
            if (Page.DeletedPages.Count > 0) return false;
            return true;
        }

        /// <summary>
        /// Processes recycle bin actions
        /// </summary>
        /// <param name="action">Action</param>
        /// <param name="vals">Values</param>
        /// <returns>Response</returns>
        public static JsonResponse Process(string action, string[] vals)
        {
            try
            {
                string message = null;

                foreach (var s in vals)
                {
                    var ar = s.Split((":").ToCharArray());

                    if (action == "Purge" && ar[0] == "All" && ar[1] == "All")
                    {
                        PurgeAll();
                        message = "Trash is empty!";
                    }
                    else
                    {
                        if (action == "Purge")
                        {
                            Purge(ar[0], new Guid(ar[1]));
                            message = string.Format("Item{0} purged",(vals.Length > 1) ? "s" : "");
                        }
                        else if (action == "Restore")
                        {
                            Restore(ar[0], new Guid(ar[1]));
                            message = string.Format("Item{0} restored", (vals.Length > 1) ? "s" : "");
                        }
                    }
                }

                if (string.IsNullOrEmpty(message))
                    return new JsonResponse { Success = true, Message = "Nothing to process" };
                else
                    return new JsonResponse { Success = true, Message = message };
            }
            catch (Exception ex)
            {
                return new JsonResponse { Message = "BlogEngine.Core.Json.JsonTrashList.Restore: " + ex.Message };
            }
        }

        static void Restore(string trashType, Guid id)
        {
            switch (trashType)
            {
                case "Comment":
                    foreach (var p in Post.Posts.ToArray())
                    {
                        var cmnt = p.DeletedComments.FirstOrDefault(c => c.Id == id);
                        if (cmnt != null)
                        {
                            p.RestoreComment(cmnt);
                            break;
                        }
                    }
                    break;
                case "Post":
                    var delPost = Post.DeletedPosts.Where(p => p.Id == id).FirstOrDefault();
                    if (delPost != null) delPost.Restore();
                    break;
                case "Page":
                    var delPage = Page.DeletedPages.Where(pg => pg.Id == id).FirstOrDefault();
                    if (delPage != null) delPage.Restore();
                    break;
                default:
                    break;
            }
        }

        static void Purge (string trashType, Guid id)
        {
            switch (trashType)
            {
                case "Comment":
                    foreach (var p in Post.Posts.ToArray())
                    {
                        var cmnt = p.DeletedComments.FirstOrDefault(c => c.Id == id);
                        if (cmnt != null)
                        {
                            p.PurgeComment(cmnt);
                            break;
                        }
                    }
                    break;
                case "Post":
                    var delPost = Post.DeletedPosts.Where(p => p.Id == id).FirstOrDefault();
                    if (delPost != null) delPost.Purge();
                    break;
                case "Page":
                    var delPage = Page.DeletedPages.Where(pg => pg.Id == id).FirstOrDefault();
                    if (delPage != null) delPage.Purge();
                    break;
                default:
                    break;
            }
        }

        static void PurgeAll()
        {
            // remove deleted comments
            foreach (var p in Post.Posts.ToArray())
            {
                foreach (var c in p.DeletedComments.ToArray())
                {
                    p.PurgeComment(c);
                }
            }

            // remove deleted posts
            foreach (var p in Post.DeletedPosts.ToArray())
            {
                p.Purge();
            }

            // remove deleted pages
            foreach (var pg in Page.DeletedPages.ToArray())
            {
                pg.Purge();
            }
        }

    }
}
