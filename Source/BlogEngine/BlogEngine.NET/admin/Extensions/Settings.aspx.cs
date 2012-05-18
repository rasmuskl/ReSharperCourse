namespace Admin.Extensions
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using BlogEngine.Core;
    using BlogEngine.Core.Web.Extensions;

    public partial class Settings : System.Web.UI.Page
    {
        /// <summary>
        /// Gets or sets enabled/disabled link in the H1 header
        /// </summary>
        protected string EnabledLink { get; set; }

        protected List<ManagedExtension> ExtensionList()
        {
            var extensions = ExtensionManager.Extensions.Where(x => x.Key != "MetaExtension").ToList();

            extensions.Sort(
                (e1, e2) => e1.Value.Priority == e2.Value.Priority ? string.CompareOrdinal(e1.Key, e2.Key) : e1.Value.Priority.CompareTo(e2.Value.Priority));

            List<ManagedExtension> manExtensions = new List<ManagedExtension>();

            foreach (KeyValuePair<string, ManagedExtension> ext in extensions)
            {
                var oExt = ExtensionManager.GetExtension(@ext.Key);
                manExtensions.Add(oExt);
            }
            return manExtensions;
        }
        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init"/> event to initialize the page.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> that contains the event data.</param>
        protected override void OnInit(EventArgs e)
        {
            Security.DemandUserHasRight(Rights.AccessAdminPages, true);

            var extname = Request.QueryString["ext"];

            var enb = Request.QueryString["enb"];
            if (!string.IsNullOrEmpty(enb))
            {
                if (bool.Parse(enb))
                {
                    EnabledLink = "<a class=\"extEnabled\" href=\"SetStatus.cshtml?ext={0}&act=false\">Enabled: click to disable</a>";
                }
                else
                {
                    EnabledLink = "<a class=\"extDisabled\" href=\"SetStatus.cshtml?ext={0}&act=true\">Disabled: click to enable</a>";
                }

                EnabledLink = string.Format(EnabledLink, extname);
            }

            foreach (var setting in from x in ExtensionManager.Extensions
                                    where x.Key == extname
                                    from setting in x.Value.Settings
                                    where !string.IsNullOrEmpty(setting.Name) && !setting.Hidden
                                    select setting)
            {
                var uc = (UserControlSettings)Page.LoadControl("Settings.ascx");
                uc.ID = setting.Name;
                ucPlaceHolder.Controls.Add(uc);
            }

            base.OnInit(e);
        }
    }
}