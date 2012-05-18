namespace Admin.Tracking
{
    using System;
    using System.Collections;
    using System.Web.Services;
    using BlogEngine.Core;
    using BlogEngine.Core.Json;

    public partial class Pingbacks : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Security.DemandUserHasRight(BlogEngine.Core.Rights.AccessAdminPages, true);
        }

        /// <summary>
        /// Number of comments in the list
        /// </summary>
        protected static int CommentCounter { get; set; }

        [WebMethod]
        public static IEnumerable LoadComments(int page)
        {
            Security.DemandUserHasRight(BlogEngine.Core.Rights.AccessAdminPages, true);

            var commentList = JsonComments.GetComments(CommentType.Pingback, page);
            CommentCounter = commentList.Count;
            return commentList;
        }

        [WebMethod]
        public static string LoadPager(int page)
        {
            Security.DemandUserHasRight(BlogEngine.Core.Rights.AccessAdminPages, true);

            return JsonComments.GetPager(page);
        }
    }
}