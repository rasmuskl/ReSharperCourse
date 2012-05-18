namespace BlogEngine.Core.Json
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    /// <summary>
    /// Json friendly list of pages
    /// </summary>
    public class JsonPages
    {
        /// <summary>
        /// Get list of pages
        /// </summary>
        /// <param name="type">Published or Drafts</param>
        /// <returns>List of pages</returns>
        public static List<JsonPage> GetPages(string type)
        {
            var allPages = new List<Page>();
            var jPages = new List<JsonPage>();

            foreach (var p in Page.Pages)
            {
                switch (type)
                {
                    case "Published":
                        if (p.IsPublished)
                            allPages.Add(p);
                        break;
                    case "Draft":
                        if (!p.IsPublished)
                            allPages.Add(p);
                        break;
                    default:
                        allPages.Add(p);
                        break;
                }
            }

            allPages.Sort((x, y) => DateTime.Compare(y.DateCreated, x.DateCreated));

            foreach (var x in allPages)
            {
                string prId = "";
                string prTtl = "";

                if (x.Parent != Guid.Empty)
                {
                    prId = x.Parent.ToString();
                    prTtl = Page.Pages.FirstOrDefault(p => p.Id.Equals(x.Parent)).Title;
                }

                var jp = new JsonPage
                {
                    Id = x.Id,
                    ShowInList = x.ShowInList,
                    IsPublished = x.IsPublished,
                    Title = string.Format("<a href=\"{0}\">{1}</a>", x.RelativeLink, System.Web.HttpContext.Current.Server.HtmlEncode(x.Title)),
                    Date = x.DateCreated.ToString("dd MMM yyyy"),
                    Time = x.DateCreated.ToString("t"),
                    ParentId = prId,
                    ParentTitle = prTtl,
                    HasChildren = x.HasChildPages,
                    CanUserDelete = x.CanUserDelete,
                    CanUserEdit = x.CanUserEdit
                };
                jPages.Add(jp);
            }

            return jPages;
        }
    }
}
