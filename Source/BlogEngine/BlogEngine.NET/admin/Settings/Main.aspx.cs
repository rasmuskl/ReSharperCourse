namespace admin.Settings
{
    using System;
    using System.IO;
    using System.Web;
    using System.Web.UI.WebControls;
    using System.Globalization;
    using System.Web.Services;
    using Resources;
    using BlogEngine.Core;
    using BlogEngine.Core.Json;
    using System.Web.Security;
    using System.Linq;
    using App_Code;
    using Page = System.Web.UI.Page;

    public partial class Main : Page
    {
        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init"/> event to initialize the page.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> that contains the event data.</param>
        protected override void OnInit(EventArgs e)
        {
            WebUtils.CheckRightsForAdminSettingsPage(false);

            BindCultures();
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
            // Bind Basic settings
            // -----------------------------------------------------------------------
            txtName.Text = BlogSettings.Instance.Name;
            txtDescription.Text = BlogSettings.Instance.Description;
            txtPostsPerPage.Text = BlogSettings.Instance.PostsPerPage.ToString();
            cbShowRelatedPosts.Checked = BlogSettings.Instance.EnableRelatedPosts;
            txtThemeCookieName.Text = BlogSettings.Instance.ThemeCookieName;
            cbUseBlogNameInPageTitles.Checked = BlogSettings.Instance.UseBlogNameInPageTitles;
            cbEnableRating.Checked = BlogSettings.Instance.EnableRating;
            cbShowDescriptionInPostList.Checked = BlogSettings.Instance.ShowDescriptionInPostList;
            txtDescriptionCharacters.Text = BlogSettings.Instance.DescriptionCharacters.ToString();
            cbShowDescriptionInPostListForPostsByTagOrCategory.Checked =
                BlogSettings.Instance.ShowDescriptionInPostListForPostsByTagOrCategory;
            txtDescriptionCharactersForPostsByTagOrCategory.Text =
                BlogSettings.Instance.DescriptionCharactersForPostsByTagOrCategory.ToString();
            cbTimeStampPostLinks.Checked = BlogSettings.Instance.TimeStampPostLinks;
            ddlCulture.SelectedValue = BlogSettings.Instance.Culture;
            txtTimeZone.Text = BlogSettings.Instance.Timezone.ToString();
            cbShowPostNavigation.Checked = BlogSettings.Instance.ShowPostNavigation;
            cbEnableQuickNotes.Checked = BlogSettings.Instance.EnableQuickNotes;
        }

        /// <summary>
        /// The bind cultures.
        /// </summary>
        private void BindCultures()
        {
            if (File.Exists(Path.Combine(HttpRuntime.AppDomainAppPath, "PrecompiledApp.config")))
            {
                var precompiledDir = HttpRuntime.BinDirectory;
                var translations = Directory.GetFiles(
                    precompiledDir, "App_GlobalResources.resources.dll", SearchOption.AllDirectories);
                foreach (var translation in translations)
                {
                    var path = Path.GetDirectoryName(translation);
                    if (path == null)
                    {
                        continue;
                    }

                    var resourceDir = path.Remove(0, precompiledDir.Length);
                    if (String.IsNullOrEmpty(resourceDir))
                    {
                        continue;
                    }

                    var info = CultureInfo.GetCultureInfoByIetfLanguageTag(resourceDir);
                    ddlCulture.Items.Add(new ListItem(info.NativeName, resourceDir));
                }
            }
            else
            {
                var path = Server.MapPath(string.Format("{0}App_GlobalResources/", Utils.ApplicationRelativeWebRoot));
                foreach (var file in Directory.GetFiles(path, "labels.*.resx"))
                {
                    var index = file.LastIndexOf(Path.DirectorySeparatorChar) + 1;
                    var filename = file.Substring(index);
                    filename = filename.Replace("labels.", string.Empty).Replace(".resx", string.Empty);
                    var info = CultureInfo.GetCultureInfoByIetfLanguageTag(filename);
                    ddlCulture.Items.Add(new ListItem(info.NativeName, filename));
                }
            }
        }

        /// <summary>
        /// Save settings
        /// </summary>
        /// <param name="name">Blog name</param>
        /// <param name="desc">Description</param>
        /// <param name="postsPerPage">Number of posts per page</param>
        /// <param name="theme">Active theme</param>
        /// <param name="mobileTheme">Mobile theme</param>
        /// <param name="themeCookieName">Cookie name to persist theme within session</param>
        /// <param name="useBlogNameInPageTitles">Whether use blog name in page titles</param>
        /// <param name="enableRelatedPosts">Enable related posts</param>
        /// <param name="enableRating">Enable rating</param>
        /// <param name="showDescriptionInPostList">Show description in post list</param>
        /// <param name="descriptionCharacters">Number of characters in description</param>
        /// <param name="showDescriptionInPostListForPostsByTagOrCategory">Shwo descripton for posts by tag or category</param>
        /// <param name="descriptionCharactersForPostsByTagOrCategory">Description characters</param>
        /// <param name="timeStampPostLinks">Time stamp post links</param>
        /// <param name="showPostNavigation">Show post navigation</param>
        /// <param name="culture">Culture</param>
        /// <param name="timezone">Time zone</param>
        /// <param name="enablePasswordReset">Enable password resets</param>
        /// <param name="enableSelfRegistration">Enable self registration</param>
        /// <param name="selfRegistrationInitialRole">Self registration initial role</param>
        /// <returns></returns>
        [WebMethod]
        public static JsonResponse Save(string name, 
			string desc,
			string postsPerPage,
			string themeCookieName,
			string useBlogNameInPageTitles,
			string enableRelatedPosts,
			string enableRating,
			string showDescriptionInPostList,
			string descriptionCharacters,
			string showDescriptionInPostListForPostsByTagOrCategory,
			string descriptionCharactersForPostsByTagOrCategory,
			string timeStampPostLinks,
			string showPostNavigation,
			string culture,
			string timezone,
            string enableQuickNotes)
        {
            var response = new JsonResponse {Success = false};

            if (!WebUtils.CheckRightsForAdminSettingsPage(true))
            {
                response.Message = "Not authorized";
                return response;
            }

            try
            {
                BlogSettings.Instance.Name = name;
                BlogSettings.Instance.Description = desc;
				BlogSettings.Instance.PostsPerPage = int.Parse(postsPerPage);
				BlogSettings.Instance.ThemeCookieName = themeCookieName;
				BlogSettings.Instance.UseBlogNameInPageTitles = bool.Parse(useBlogNameInPageTitles);
				BlogSettings.Instance.EnableRelatedPosts = bool.Parse(enableRelatedPosts);
				BlogSettings.Instance.EnableRating = bool.Parse(enableRating);
				BlogSettings.Instance.ShowDescriptionInPostList = bool.Parse(showDescriptionInPostList);
				BlogSettings.Instance.DescriptionCharacters = int.Parse(descriptionCharacters);
				BlogSettings.Instance.ShowDescriptionInPostListForPostsByTagOrCategory =
					bool.Parse(showDescriptionInPostListForPostsByTagOrCategory);
				BlogSettings.Instance.DescriptionCharactersForPostsByTagOrCategory =
					int.Parse(descriptionCharactersForPostsByTagOrCategory);
				BlogSettings.Instance.TimeStampPostLinks = bool.Parse(timeStampPostLinks);
				BlogSettings.Instance.ShowPostNavigation = bool.Parse(showPostNavigation);
				BlogSettings.Instance.Culture = culture;
				BlogSettings.Instance.Timezone = double.Parse(timezone);
                BlogSettings.Instance.EnableQuickNotes = bool.Parse(enableQuickNotes);
                BlogSettings.Instance.Save();
            }
            catch (Exception ex)
            {
                Utils.Log(string.Format("admin.Settings.Main.Save(): {0}", ex.Message));
                response.Message = string.Format("Could not save settings: {0}", ex.Message);
                return response;
            }

            response.Success = true;
            response.Message = "Settings saved";
            return response;
        }
    }
}