// --------------------------------------------------------------------------------------------------------------------
// <summary>
//   Avatar support.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace BlogEngine.Core
{
    using System;
    using System.Globalization;
    using System.Web;
    using System.Web.Security;

    /// <summary>
    /// Avatar support.
    /// </summary>
    public class Avatar
    {
        #region Constants and Fields

        /// <summary>
        ///     The avatar image.
        /// </summary>
        private const string AvatarImage = "<img class=\"photo\" src=\"{0}\" alt=\"{1}\" width=\"{2}\" height=\"{3}\" />";

        /// <summary>
        ///     Gets or sets the URL to the Avatar image.
        /// </summary>
        public Uri Url { get; set; }

        /// <summary>
        ///     Gets or sets the image tag for the Avatar image.
        /// </summary>
        public string ImageTag { get; set; }

        /// <summary>
        ///    Gets or sets a value indicating whether there is not a specific image available.
        /// </summary>
        public bool HasNoImage { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Returns the avatar/gravatar that matches the specified email, website or avatar Url.
        /// </summary>
        /// <param name="size">
        /// The image size.
        /// </param>
        /// <param name="email">
        /// Email address.
        /// </param>
        /// <param name="website">
        /// The website URL.
        /// </param>
        /// <param name="avatarUrl">
        /// An optional avatar URL to use instead of the default.
        /// </param>
        /// <param name="description">
        /// Description used for the Alt/Title attributes.
        /// </param>
        /// <returns>
        /// The avatar/gravatar image.
        /// </returns>
        public static Avatar GetAvatar(int size, string email, Uri website, string avatarUrl, string description)
        {
            return GetAvatar(email, website, avatarUrl, description, size, size);
        }

        /// <summary>
        /// Returns the avatar/gravatar that matches the specified email, website or avatar Url.
        /// </summary>
        /// <param name="email">
        /// Email address.
        /// </param>
        /// <param name="website">
        /// The website URL.
        /// </param>
        /// <param name="avatarUrl">
        /// An optional avatar URL to use instead of the default.
        /// </param>
        /// <param name="description">
        /// Description used for the Alt/Title attributes.
        /// </param>
        /// <param name="width">
        /// The image width.
        /// </param>
        /// <param name="height">
        /// The image height.
        /// </param>
        /// <returns>
        /// The avatar/gravatar image.
        /// </returns>
        public static Avatar GetAvatar(string email, Uri website = null, string avatarUrl = "", string description = "", int width = 80, int height = 80)
        {
            if (width < 1 || width > 400) width = 80;
            if (height < 1 || height > 400) height = 80;

            if (BlogSettings.Instance.Avatar == "none" && email != "pingback" && email != "trackback")
                return DefaultImage(description, width, height);

            string imageTag;
            Uri url;

            // custom avatar URL (different than email)
            if (!string.IsNullOrEmpty(avatarUrl) && Uri.TryCreate(avatarUrl, UriKind.RelativeOrAbsolute, out url))
                return CustomAvatar(url, description, width, height);

            // email is required and must have "@" to be valid
            // the only exceptions are pingbacks and trackbacks
            if (string.IsNullOrEmpty(email) || !email.Contains("@"))
            {
                if(ValidRemoteSite(website))
                {
                    return SiteThumb(email, website, description, width, height);
                }
                else
                {
                    // invalid - fall back to default avatar image
                    return DefaultImage(description, width, height);
                }
            }

            return Gravatar(email, description, width, height);
        }

        /// <summary>
        /// Returns the avatar/gravatar image tag that matches the specified email, website or avatar Url.
        /// </summary>
        /// <param name="size">
        /// The image size.
        /// </param>
        /// <param name="email">
        /// Email address.
        /// </param>
        /// <param name="website">
        /// The website URL.
        /// </param>
        /// <param name="avatarUrl">
        /// An optional avatar URL to use instead of the default.
        /// </param>
        /// <param name="description">
        /// Description used for the Alt/Title attributes.
        /// </param>
        /// <returns>
        /// The avatar/gravatar image.
        /// </returns>
        public static string GetAvatarImageTag(int size, string email, Uri website, string avatarUrl, string description)
        {
            return GetAvatarImageTag(email, website, avatarUrl, description, size, size);
        }

        /// <summary>
        /// Returns the avatar/gravatar image tag that matches the specified email, website or avatar Url.
        /// </summary>
        /// <param name="email">Email address</param>
        /// <param name="website">Website URL</param>
        /// <param name="avatarUrl">An optional avatar URL to use instead of the default.</param>
        /// <param name="description">Description used for the Alt/Title attributes.</param>
        /// <param name="width">Image width</param>
        /// <param name="height">Image height</param>
        /// <returns>The avatar/gravatar image.</returns>
        public static string GetAvatarImageTag(string email, Uri website = null, string avatarUrl = "", string description = "", int width = 80, int height = 80)
        {
            var avatar = GetAvatar(email, website, avatarUrl, description, width, height);
            return avatar == null || string.IsNullOrEmpty(avatar.ImageTag) ? string.Empty : avatar.ImageTag;
        }

        #endregion

        #region Methods

        static Avatar Gravatar(string email, string description, int width, int height)
        {
            var hash = FormsAuthentication.HashPasswordForStoringInConfigFile(email.ToLowerInvariant().Trim(), "MD5");
            if (hash != null) hash = hash.ToLowerInvariant();

            var gravatar = string.Format("http://www.gravatar.com/avatar/{0}.jpg?s={1}&amp;d=", hash, width);

            string link;
            switch (BlogSettings.Instance.Avatar)
            {
                case "identicon":
                    link = string.Format("{0}identicon", gravatar);
                    break;

                case "wavatar":
                    link = string.Format("{0}wavatar", gravatar);
                    break;

                default:
                    link = string.Format("{0}monsterid", gravatar);
                    break;
            }

            var imageTag = string.Format(CultureInfo.InvariantCulture,
                AvatarImage, link, HttpUtility.HtmlEncode(description), width, height);

            return new Avatar { Url = new Uri(link), ImageTag = imageTag };
        }

        static Avatar CustomAvatar(Uri url, string description, int width, int height)
        {
            var imageTag = string.Format(CultureInfo.InvariantCulture, 
                AvatarImage, url, HttpUtility.HtmlEncode(description), width, height);

            return new Avatar { Url = url, ImageTag = imageTag };
        }

        static Avatar SiteThumb(string email, Uri website, string description, int width, int height)
        {
            // http://www.robothumb.com/src/?url={0}
            // http://api.thumbalizr.com/?url={0}

            var api = BlogSettings.Instance.ThumbnailServiceApi;

            if(string.IsNullOrEmpty(api) || !api.Contains("{0}"))
                return DefaultImage(description, width, height);

            var url = new Uri(string.Format(api, HttpUtility.UrlEncode(website.ToString())));

            var imageTag = string.Format(CultureInfo.InvariantCulture,
                "<img class=\"thumb\" src=\"{0}\" alt=\"{1}\" width=\"{2}\" height=\"{3}\" />",
                url, email, width, height);

            return new Avatar { Url = url, ImageTag = imageTag };
        }

        static Avatar DefaultImage(string description, int width, int height)
        {
            var themeAvatar = HttpContext.Current.Server.MapPath(
                string.Format("{0}themes/{1}/noavatar.jpg", Utils.RelativeWebRoot, BlogSettings.Instance.Theme));

            var uri = new Uri(
                System.IO.File.Exists(themeAvatar) ?
                string.Format("{0}themes/{1}/noavatar.jpg", Utils.AbsoluteWebRoot, BlogSettings.Instance.Theme) :
                string.Format("{0}pics/noavatar.jpg", Utils.AbsoluteWebRoot)
            );

            var imageTag = string.Format(
                "<img src=\"{0}\" alt=\"{1}\" width=\"{2}\" height=\"{3}\" />", 
                uri, description, width, height);

            return new Avatar { Url = uri, ImageTag = imageTag, HasNoImage = true };
        }

        static bool ValidRemoteSite(Uri website)
        {
            if (website != null && website.ToString().Length > 0 && 
                (website.ToString().StartsWith("http://") || website.ToString().StartsWith("https://")))
            {
                return true;
            }
            return false;
        }

        #endregion
    }
}