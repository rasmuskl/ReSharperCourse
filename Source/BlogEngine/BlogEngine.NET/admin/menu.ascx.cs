// --------------------------------------------------------------------------------------------------------------------
// <summary>
//   The Menu control.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Admin
{
    using System;
    using System.Linq;
    using System.Web;
    using System.Web.UI;
    using System.Web.UI.HtmlControls;

    using BlogEngine.Core;

    using Resources;

    /// <summary>
    /// The Menu control.
    /// </summary>
    public partial class Menu : UserControl
    {
        #region Public Methods

        /// <summary>
        /// Adds the item.
        /// </summary>
        /// <param name="text">
        /// The text string.
        /// </param>
        /// <param name="url">
        /// The URL string.
        /// </param>
        public void AddItem(string text, string url)
        {
            var a = new HtmlAnchor { InnerHtml = string.Format("<span>{0}</span>", text), HRef = url };

            var li = new HtmlGenericControl("li");
            li.Controls.Add(a);
            ulMenu.Controls.Add(li);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Load"/> event.
        /// </summary>
        /// <param name="e">
        /// The <see cref="T:System.EventArgs"/> object that contains the event data.
        /// </param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!Page.IsCallback)
            {
                BindMenu();
            }
        }

        /// <summary>
        /// Gets the sub URL.
        /// </summary>
        /// <param name="url">
        /// The URL string.
        /// </param>
        /// <returns>
        /// The sub-url.
        /// </returns>
        private static string SubUrl(string url, bool isFromCurrentHttpRequest)
        {
            if (isFromCurrentHttpRequest && Blog.CurrentInstance.IsSubfolderOfApplicationWebRoot)
                url = Utils.ApplicationRelativeWebRoot + url.Substring(Blog.CurrentInstance.RelativeWebRoot.Length);

            var i = url.LastIndexOf("/");

            return (i > 0) ? url.Substring(0, i) : string.Empty;
        }

        /// <summary>
        /// The bind menu.
        /// </summary>
        private void BindMenu()
        {
            var sitemap = SiteMap.Providers["SecuritySiteMap"];
            if (sitemap != null)
            {
                string adminRootFolder = string.Format("{0}admin", Utils.RelativeWebRoot);

                var root = sitemap.RootNode;
                if (root != null)
                {
                    foreach (
                        var adminNode in
                            root.ChildNodes.Cast<SiteMapNode>().Where(
                                adminNode => adminNode.IsAccessibleToUser(HttpContext.Current)).Where(
                                    adminNode =>
                                    Request.RawUrl.ToUpperInvariant().Contains("/ADMIN/") ||
                                    (!adminNode.Url.Contains("xmanager") && !adminNode.Url.Contains("PingServices"))))
                    {

                        var a = new HtmlAnchor
                            {
                                // replace the RelativeWebRoot in adminNode.Url with the RelativeWebRoot of the current
                                // blog instance.  So a URL like /admin/Dashboard.aspx becomes /blog/admin/Dashboard.aspx.
                                HRef = Utils.RelativeWebRoot + adminNode.Url.Substring(Utils.ApplicationRelativeWebRoot.Length), 
                                InnerHtml =
                                    string.Format("<span>{0}</span>", Utils.Translate(adminNode.Title, adminNode.Title))
                            };

                        // "<span>" + Utils.Translate(info.Name.Replace(".aspx", string.Empty)) + "</span>";
                        var startIndx = adminNode.Url.LastIndexOf("/admin/") > 0 ? adminNode.Url.LastIndexOf("/admin/") : 0;
                        var endIndx = adminNode.Url.LastIndexOf(".") > 0 ? adminNode.Url.LastIndexOf(".") : adminNode.Url.Length;
                        var nodeDir = adminNode.Url.Substring(startIndx, endIndx - startIndx);

                        if (Request.RawUrl.IndexOf(nodeDir, StringComparison.OrdinalIgnoreCase) != -1)
                        {
                            a.Attributes["class"] = "current";
                        }

                        // select "plugins" tab for extensions with custom admin pages
                        if (Request.RawUrl.IndexOf("User controls", StringComparison.OrdinalIgnoreCase) != -1)
                        {
                            if (nodeDir == "/admin/Extensions/default")
                                a.Attributes["class"] = "current";
                        }

                        // if "page" has its own subfolder (comments, extensions) should 
                        // select parent tab when navigating through child tabs
                        if (!SubUrl(Request.RawUrl, true).Equals(adminRootFolder, StringComparison.OrdinalIgnoreCase) &&
                            SubUrl(Request.RawUrl, true) == SubUrl(adminNode.Url, false))
                        {
                            a.Attributes["class"] = "current";
                        }

                        var li = new HtmlGenericControl("li");
                        li.Controls.Add(a);
                        ulMenu.Controls.Add(li);
                    }
                }
            }

            if (!Request.RawUrl.ToUpperInvariant().Contains("/ADMIN/"))
            {
                AddItem(
                    labels.myProfile, string.Format("{0}admin/Users/Profile.aspx?id={1}", Utils.RelativeWebRoot, HttpUtility.UrlPathEncode(Security.CurrentUser.Identity.Name)));

                AddItem(
                    labels.changePassword, string.Format("{0}Account/change-password.aspx", Utils.RelativeWebRoot));
            }
        }

        #endregion
    }
}