namespace BlogEngine.Core.Json
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Web.Hosting;

    /// <summary>
    /// List of blogs
    /// </summary>
    public static class JsonBlogs
    {
        /// <summary>
        /// The current page.
        /// </summary>
        private static int currentPage = 1;

        /// <summary>
        /// The comm cnt.
        /// </summary>
        private static int blogCnt;

        /// <summary>
        /// Gets blog list based on selection for current page
        /// </summary>
        /// <param name="page">Current page</param>
        /// <param name="pageSize">Page Size</param>
        /// <returns>List of blogs</returns>
        public static List<JsonBlog> GetBlogs(int page, int pageSize)
        {
            pageSize = Math.Max(pageSize, 1);
            var cntTo = page * pageSize;
            var cntFrom = cntTo - pageSize;
            var cnt = 0;

            // putting blogs into its own list so we can sort it without
            // modifying the original collection.
            var blogs = new List<Blog>(Blog.Blogs);

            // sort so primary blog comes first, followed by other blogs
            // in alphabetical order.
            blogs.Sort((x, y) =>
            {
                if (x.IsPrimary && !y.IsPrimary)
                    return -1;
                else if (!x.IsPrimary && y.IsPrimary)
                    return 1;

                return x.Name.CompareTo(y.Name);
            });

            var jsonBlogs = new List<JsonBlog>();

            // convert each blog into smaller Json friendly object 
            foreach (var x in blogs)
            {
                cnt++;
                if (cnt <= cntFrom || cnt > cntTo)
                {
                    continue;
                }

                jsonBlogs.Add(CreateJsonBlog(x));
            }

            currentPage = page;
            blogCnt = cnt;

            return jsonBlogs;
        }

        public static JsonBlog CreateJsonBlog(Blog b)
        {
            var jb = new JsonBlog
            {
                Id = b.Id,
                Name = b.Name,
                StorageContainerName = b.StorageContainerName,
                Hostname = b.Hostname,
                IsAnyTextBeforeHostnameAccepted = b.IsAnyTextBeforeHostnameAccepted,
                VirtualPath = b.VirtualPath,
                IsActive = b.IsActive,
                IsSiteAggregation = b.IsSiteAggregation,
                IsPrimary = b.IsPrimary,
                RelativeWebRoot = b.RelativeWebRoot,
                AbsoluteWebRoot = b.AbsoluteWebRoot,
                PhysicalStorageLocation = HostingEnvironment.MapPath(b.StorageLocation),
                CanUserEdit = b.CanUserEdit,
                CanUserDelete = b.CanUserDelete
            };

            return jb;
        }

        /// <summary>
        /// Builds pager control for blogs page
        /// </summary>
        /// <param name="page">Current Page Number</param>
        /// <param name="pageSize">Page Size</param>
        /// <returns></returns>
        public static string GetPager(int page, int pageSize)
        {
            if (blogCnt == 0)
            {
                return string.Empty;
            }
            if (page < 1) page = 1;

            var prvLnk = string.Empty;
            var nxtLnk = string.Empty;
            var firstLnk = string.Empty;
            var lastLnk = string.Empty;
            const string linkFormat = "<a href=\"#\" id=\"{0}\" onclick=\"return LoadBlogsForPage('{1}');\" class=\"{0}\"></a>";

            pageSize = Math.Max(pageSize, 1);
            var pgs = Convert.ToDecimal(blogCnt) / Convert.ToDecimal(pageSize);
            var p = pgs - (int)pgs;
            var lastPage = p > 0 ? (int)pgs + 1 : (int)pgs;

            var blogTo = page * pageSize;
            if (blogTo > blogCnt) blogTo = blogCnt;

            var currentScope = ((page * pageSize) - (pageSize - 1)).ToString() + " - " + blogTo.ToString();

            var pageLink = string.Format("Showing <span id=\"PagerCurrentPage\">{0}</span> of {1}", currentScope, blogCnt);

            if (currentPage > 1)
            {
                prvLnk = string.Format(linkFormat, "prevLink", page - 1);
                firstLnk = string.Format(linkFormat, "firstLink", 1);
            }

            if (page < lastPage)
            {
                nxtLnk = string.Format(linkFormat, "nextLink", page + 1);
                lastLnk = string.Format(linkFormat, "lastLink", lastPage);
            }

            return "<div id=\"ListPager\">" + firstLnk + prvLnk + pageLink + nxtLnk + lastLnk + "</div>";
        }
    }
}
