namespace Admin.Comments
{
    using System;
    using System.Collections;
    using System.Web.Services;
    using BlogEngine.Core.Json;
    using App_Code;

    public partial class Approved : System.Web.UI.Page
    {
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            WebUtils.CheckRightsForAdminCommentsPages(false);
        }

        /// <summary>
        /// Number of comments in the list
        /// </summary>
        protected static int CommentCounter { get; set; }

        [WebMethod]
        public static IEnumerable LoadComments(int page)
        {
            WebUtils.CheckRightsForAdminCommentsPages(false);

            var commentList = JsonComments.GetComments(CommentType.Approved, page);
            CommentCounter = commentList.Count;
            return commentList;
        }

        [WebMethod]
        public static string LoadPager(int page)
        {
            WebUtils.CheckRightsForAdminCommentsPages(false);

            return JsonComments.GetPager(page);
        }
    }
}