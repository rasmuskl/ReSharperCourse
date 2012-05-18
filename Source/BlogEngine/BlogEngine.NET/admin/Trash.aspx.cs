namespace Admin
{
    using System;
    using System.Collections;
    using System.Web.Services;
    using BlogEngine.Core;
    using BlogEngine.Core.Json;
    using App_Code;

    public partial class Trash : System.Web.UI.Page
    {
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            Security.DemandUserHasRight(BlogEngine.Core.Rights.AccessAdminPages, true);
        }

        /// <summary>
        /// Number of items in the list
        /// </summary>
        protected static int TrashCounter { get; set; }

        [WebMethod]
        public static IEnumerable LoadTrash(string trashType, int page)
        {
            Security.DemandUserHasRight(BlogEngine.Core.Rights.AccessAdminPages, true);

            var tType = TrashType.All;
            switch (trashType)
            {
                case "Post":
                    tType = TrashType.Post;
                    break;
                case "Page":
                    tType = TrashType.Page;
                    break;
                case "Comment":
                    tType = TrashType.Comment;
                    break;
                default:
                    break;
            }
            var trashList = JsonTrashList.GetTrash(tType, page);
            TrashCounter = trashList.Count;
            return trashList;
        }

        [WebMethod]
        public static string LoadPager(int page)
        {
            WebUtils.CheckRightsForAdminSettingsPage(false);

            return JsonTrashList.GetPager(page);
        }

        [WebMethod]
        public static JsonResponse ProcessTrash(string action, string[] vals)
        {
            Security.DemandUserHasRight(BlogEngine.Core.Rights.AccessAdminPages, true);

            return JsonTrashList.Process(action, vals);
        }
    }
}