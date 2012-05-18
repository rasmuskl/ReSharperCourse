namespace admin.Settings
{
    using System;
    using System.Web.Services;
    using Resources;
    using BlogEngine.Core;
    using BlogEngine.Core.Json;
    using App_Code;

    using Page = System.Web.UI.Page;

    public partial class HeadTrack : Page
    {
        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init"/> event to initialize the page.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> that contains the event data.</param>
        protected override void OnInit(EventArgs e)
        {
            WebUtils.CheckRightsForAdminSettingsPage(false);

            BindSettings();

            Page.MaintainScrollPositionOnPostBack = true;
            Page.Title = labels.settings;
            base.OnInit(e);
        }

        /// <summary>
        /// The bind settings.
        /// </summary>
        private void BindSettings()
        {
            // -----------------------------------------------------------------------
            // HTML header section
            // -----------------------------------------------------------------------
            txtHtmlHeader.Text = BlogSettings.Instance.HtmlHeader;

            // -----------------------------------------------------------------------
            // Visitor tracking settings
            // -----------------------------------------------------------------------
            txtTrackingScript.Text = BlogSettings.Instance.TrackingScript;
        }

        /// <summary>
        /// Save settings
        /// </summary>
        /// <param name="hdr">Header script</param>
        /// <param name="ftr">Tracking script</param>
        /// <returns>Json response</returns>
        [WebMethod]
        public static JsonResponse Save(string hdr, string ftr)
        {
            var response = new JsonResponse {Success = false};

            if (!WebUtils.CheckRightsForAdminSettingsPage(true))
            {
                response.Message = "Not authorized";
                return response;
            }

            try
            {
                BlogSettings.Instance.HtmlHeader = hdr;
                BlogSettings.Instance.TrackingScript = ftr;
                BlogSettings.Instance.Save();
            }
            catch (Exception ex)
            {
                Utils.Log(string.Format("admin.Settings.HeadTrack.Save(): {0}", ex.Message));
                response.Message = string.Format("Could not save settings: {0}", ex.Message);
                return response;
            }

            response.Success = true;
            response.Message = "Settings saved";
            return response;
        }
    }
}