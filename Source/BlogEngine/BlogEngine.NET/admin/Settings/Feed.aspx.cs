namespace admin.Settings
{
    using System;
    using System.Globalization;
    using System.Web.Services;
    using Resources;
    using BlogEngine.Core;
    using BlogEngine.Core.Json;
    using App_Code;
    using Page = System.Web.UI.Page;

    public partial class Feed : Page
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
            // Bind Syndication settings
            // -----------------------------------------------------------------------
            ddlSyndicationFormat.SelectedValue = BlogSettings.Instance.SyndicationFormat;
            txtPostsPerFeed.Text = BlogSettings.Instance.PostsPerFeed.ToString();
            txtDublinCoreCreator.Text = BlogSettings.Instance.AuthorName;
            txtEmail.Text = BlogSettings.Instance.FeedAuthor;
            txtDublinCoreLanguage.Text = BlogSettings.Instance.Language;

            txtGeocodingLatitude.Text = BlogSettings.Instance.GeocodingLatitude != Single.MinValue
                   ? BlogSettings.Instance.GeocodingLatitude.ToString(CultureInfo.InvariantCulture)
                   : String.Empty;
            txtGeocodingLongitude.Text = BlogSettings.Instance.GeocodingLongitude != Single.MinValue
                   ? BlogSettings.Instance.GeocodingLongitude.ToString(CultureInfo.InvariantCulture)
                   : String.Empty;

            txtBlogChannelBLink.Text = BlogSettings.Instance.Endorsement;
            txtAlternateFeedUrl.Text = BlogSettings.Instance.AlternateFeedUrl;
            cbEnableEnclosures.Checked = BlogSettings.Instance.EnableEnclosures;
        }

        /// <summary>
        /// Save settings
        /// </summary>
        /// <param name="syndicationFormat"></param>
        /// <param name="postsPerFeed"></param>
        /// <param name="dublinCoreCreator"></param>
        /// <param name="feedemail"></param>
        /// <param name="dublinCoreLanguage"></param>
        /// <param name="geocodingLatitude"></param>
        /// <param name="geocodingLongitude"></param>
        /// <param name="blogChannelBLink"></param>
        /// <param name="alternateFeedUrl"></param>
        /// <param name="enableEnclosures"></param>
        /// <returns></returns>
        [WebMethod]
        public static JsonResponse Save(
			string syndicationFormat, 
			string postsPerFeed,
			string dublinCoreCreator,
            string feedemail,
			string dublinCoreLanguage,
			string geocodingLatitude,
			string geocodingLongitude,
			string blogChannelBLink,
			string alternateFeedUrl,
            string enableEnclosures)
        {
            var response = new JsonResponse {Success = false};

            if (!WebUtils.CheckRightsForAdminSettingsPage(true))
            {
                response.Message = "Not authorized";
                return response;
            }

            try
            {
				BlogSettings.Instance.SyndicationFormat = syndicationFormat;
				BlogSettings.Instance.PostsPerFeed = int.Parse(postsPerFeed, CultureInfo.InvariantCulture);
				BlogSettings.Instance.AuthorName = dublinCoreCreator;
                BlogSettings.Instance.FeedAuthor = feedemail;
				BlogSettings.Instance.Language = dublinCoreLanguage;

				float latitude;
				BlogSettings.Instance.GeocodingLatitude = Single.TryParse(
					geocodingLatitude.Replace(",", "."),
					NumberStyles.Any,
					CultureInfo.InvariantCulture,
					out latitude) ? latitude : Single.MinValue;

				float longitude;
				BlogSettings.Instance.GeocodingLongitude = Single.TryParse(
					geocodingLongitude.Replace(",", "."),
					NumberStyles.Any,
					CultureInfo.InvariantCulture,
					out longitude) ? longitude : Single.MinValue;

				BlogSettings.Instance.Endorsement = blogChannelBLink;

				if (alternateFeedUrl.Trim().Length > 0 && !alternateFeedUrl.Contains("://"))
				{
					alternateFeedUrl = string.Format("http://{0}", alternateFeedUrl);
				}

				BlogSettings.Instance.AlternateFeedUrl = alternateFeedUrl;
                BlogSettings.Instance.EnableEnclosures = bool.Parse(enableEnclosures);

                BlogSettings.Instance.Save();
            }
            catch (Exception ex)
            {
                Utils.Log(string.Format("admin.Settings.Feed.Save(): {0}", ex.Message));
                response.Message = string.Format("Could not save settings: {0}", ex.Message);
                return response;
            }

            response.Success = true;
            response.Message = "Settings saved";
            return response;
        }
    }
}