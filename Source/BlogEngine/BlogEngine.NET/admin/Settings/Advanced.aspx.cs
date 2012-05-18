namespace admin.Settings
{
    using System;
    using System.Web.Services;
    using Resources;
    using BlogEngine.Core;
    using BlogEngine.Core.Json;
    using System.Web.Security;
    using System.Linq;
    using App_Code;
    using Page = System.Web.UI.Page;
    using BlogEngine.Core.Providers;
    using System.Configuration;
    using System.Web.Configuration;
    using System.IO;

    public partial class Advanced : Page
    {
        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init"/> event to initialize the page.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> that contains the event data.</param>
        protected override void OnInit(EventArgs e)
        {
            WebUtils.CheckRightsForAdminSettingsPage(false);

            BindRoles();
            BindSettings();

            Page.MaintainScrollPositionOnPostBack = true;
            Page.Title = labels.settings;
            base.OnInit(e);
        }

        private void BindRoles()
        {
            ddlSelfRegistrationInitialRole.AppendDataBoundItems = true;
            ddlSelfRegistrationInitialRole.DataSource = Roles.GetAllRoles().Where(r => !r.Equals(BlogConfig.AnonymousRole, StringComparison.OrdinalIgnoreCase));
            ddlSelfRegistrationInitialRole.DataBind();
        }

        /// <summary>
        /// The bind settings.
        /// </summary>
        private void BindSettings()
        {
            // -----------------------------------------------------------------------
            // Bind Advanced settings
            // -----------------------------------------------------------------------

            var settings = BlogSettings.Instance;

            cbEnableCompression.Checked = settings.EnableHttpCompression;
            cbRemoveWhitespaceInStyleSheets.Checked = settings.RemoveWhitespaceInStyleSheets;
            cbCompressWebResource.Checked = settings.CompressWebResource;
            cbEnableOpenSearch.Checked = settings.EnableOpenSearch;
            cbRequireSslForMetaWeblogApi.Checked = settings.RequireSslMetaWeblogApi;
            rblWwwSubdomain.SelectedValue = settings.HandleWwwSubdomain;
            cbEnableErrorLogging.Checked = settings.EnableErrorLogging;
            txtGalleryFeed.Text = settings.GalleryFeedUrl;
            cbAllowRemoteFileDownloads.Checked = settings.AllowServerToDownloadRemoteFiles;
            txtRemoteTimeout.Text = settings.RemoteFileDownloadTimeout.ToString();
            txtRemoteMaxFileSize.Text = settings.RemoteMaxFileSize.ToString();
            cbEnablePasswordReset.Checked = BlogSettings.Instance.EnablePasswordReset;
            cbEnableSelfRegistration.Checked = BlogSettings.Instance.EnableSelfRegistration;
            Utils.SelectListItemByValue(ddlSelfRegistrationInitialRole, BlogSettings.Instance.SelfRegistrationInitialRole);
            if (!Page.IsPostBack)
            {
                ddlProvider.DataSource = BlogService.FileSystemProviders;
                ddlProvider.DataTextField = "Description";
                ddlProvider.DataValueField = "Name";
                ddlProvider.DataBind();
                ddlProvider.SelectedValue = BlogService.FileSystemProvider.Name;
                hdnProvider.Value = BlogService.FileSystemProvider.Name;
            }
        }

        protected void btnChangeProvider_Click(object sender, EventArgs e)
        {
            providerError.Visible = false;
            var zipArchive = Server.MapPath(string.Format("{0}FileSystemBackup-{1}.zip", Blog.CurrentInstance.StorageLocation, DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss")));
            var msg = new BlogEngine.Core.FileSystem.FileSystemUtilities().DumpProvider(ddlProvider.SelectedValue.ToString(), zipArchive);
            if (!string.IsNullOrWhiteSpace(msg))
            {
                providerError.Visible = true;
                providerError.Text = msg;
            }
            else
                hdnProvider.Value = ddlProvider.SelectedValue.ToString();
        }

        protected void btnDownloadArchive_Click(object sender, EventArgs e)
        {
            var zipArchive = Server.MapPath(string.Format("{0}FileSystemBackup-{1}.zip", Blog.CurrentInstance.StorageLocation, DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss")));
            new BlogEngine.Core.FileSystem.FileSystemUtilities().CompressDirectory(zipArchive, Blog.CurrentInstance.RootFileStore);
            var file = new FileInfo(zipArchive);
            byte[] Buffer = null;
            System.IO.FileStream FileStream = new System.IO.FileStream(file.FullName, System.IO.FileMode.Open, System.IO.FileAccess.Read);
            System.IO.BinaryReader BinaryReader = new System.IO.BinaryReader(FileStream);
            long TotalBytes = file.Length;
            Buffer = BinaryReader.ReadBytes((int)TotalBytes);
            FileStream.Close();
            FileStream.Dispose();
            BinaryReader.Close();
            Response.AppendHeader("Content-Disposition", string.Format("attachment; filename=\"{0}\"", file.Name));
            Response.BinaryWrite(Buffer);
        }

        /// <summary>
        /// Save settings
        /// </summary>
        /// <param name="enableCompression"></param>
        /// <param name="removeWhitespaceInStyleSheets"></param>
        /// <param name="compressWebResource"></param>
        /// <param name="enableOpenSearch"></param>
        /// <param name="requireSslForMetaWeblogApi"></param>
        /// <param name="wwwSubdomain"></param>
        /// <param name="enableTrackBackSend"></param>
        /// <param name="enableTrackBackReceive"></param>
        /// <param name="enablePingBackSend"></param>
        /// <param name="enablePingBackReceive"></param>
        /// <param name="enableErrorLogging"></param>
        /// <param name="allowRemoteFileDownloads"></param>
        /// <param name="remoteTimeout"></param>
        /// <param name="remoteMaxFileSize"></param>
        /// <param name="galleryFeedUrl">Online gallery feed URL</param>
        /// <returns></returns>
        [WebMethod]
        public static JsonResponse Save(bool enableCompression, 
			bool removeWhitespaceInStyleSheets,
            bool compressWebResource,
            bool enableOpenSearch,
            bool requireSslForMetaWeblogApi,
			string wwwSubdomain,
            /*bool enableTrackBackSend,
            bool enableTrackBackReceive,
            bool enablePingBackSend,
            bool enablePingBackReceive,*/
            bool enableErrorLogging,
            bool allowRemoteFileDownloads,
            int remoteTimeout,
            int remoteMaxFileSize,
            string galleryFeedUrl,
            string enablePasswordReset,
            string enableSelfRegistration,
            string selfRegistrationInitialRole)
        {
            var response = new JsonResponse { Success = false };
            var settings = BlogSettings.Instance;

            if (!WebUtils.CheckRightsForAdminSettingsPage(true))
            {
                response.Message = "Not authorized";
                return response;
            }

            try
            {

                // Validate values before setting any of them to the BlogSettings instance.
                // Because it's a singleton, we don't want partial data being stored to
                // it if there's any exceptions thrown prior to saving. 

                if (remoteTimeout < 0)
                {
                    throw new ArgumentOutOfRangeException("RemoteFileDownloadTimeout must be greater than or equal to 0 milliseconds.");
                }
                else if (remoteMaxFileSize < 0)
                {
                    throw new ArgumentOutOfRangeException("RemoteMaxFileSize must be greater than or equal to 0 bytes.");
                }  

                settings.EnableHttpCompression = enableCompression;
                settings.RemoveWhitespaceInStyleSheets = removeWhitespaceInStyleSheets;
                settings.CompressWebResource = compressWebResource;
                settings.EnableOpenSearch = enableOpenSearch;
                settings.RequireSslMetaWeblogApi = requireSslForMetaWeblogApi;
                settings.HandleWwwSubdomain = wwwSubdomain;
                settings.EnableErrorLogging = enableErrorLogging;
                settings.GalleryFeedUrl = galleryFeedUrl;

                settings.AllowServerToDownloadRemoteFiles = allowRemoteFileDownloads;
                settings.RemoteFileDownloadTimeout = remoteTimeout;
                settings.RemoteMaxFileSize = remoteMaxFileSize;
                settings.EnablePasswordReset = bool.Parse(enablePasswordReset);
                settings.EnableSelfRegistration = bool.Parse(enableSelfRegistration);
                settings.SelfRegistrationInitialRole = selfRegistrationInitialRole;

                settings.Save();
            }
            catch (Exception ex)
            {
                Utils.Log(string.Format("admin.Settings.Advanced.Save(): {0}", ex.Message));
                response.Message = string.Format("Could not save settings: {0}", ex.Message);
                return response;
            }

            response.Success = true;
            response.Message = "Settings saved";
            return response;
        }
    }
}