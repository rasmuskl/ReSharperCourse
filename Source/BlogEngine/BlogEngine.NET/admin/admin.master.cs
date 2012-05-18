// --------------------------------------------------------------------------------------------------------------------
// <summary>
//   The AdminMasterPage.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Admin
{
    using System;
    using System.Threading;
    using System.Web.UI;

    using BlogEngine.Core;

    /// <summary>
    /// The AdminMasterPage.
    /// </summary>
    public partial class AdminMasterPage : MasterPage
    {
        #region Public Methods

        /// <summary>
        /// Sets the status.
        /// </summary>
        /// <param name="status">
        /// The status.
        /// </param>
        /// <param name="msg">
        /// The message.
        /// </param>
        public void SetStatus(string status, string msg)
        {
            this.AdminStatus.Attributes.Clear();
            this.AdminStatus.Attributes.Add("class", status);
            this.AdminStatus.InnerHtml =
                string.Format(
                    "{0}<a href=\"javascript:HideStatus()\" style=\"width:20px;float:right\">X</a>", 
                    this.Server.HtmlEncode(msg));
        }

        #endregion

        #region Methods

        protected string AdminJs { get { return Utils.ApplicationRelativeWebRoot + "admin/js"; } }

        /// <summary>
        /// Gets the current user's photo.
        /// </summary>
        /// <returns>
        /// The user photo.
        /// </returns>
        protected string UserPhoto()
        {
            var src = string.Format("{0}admin/images/no_avatar.png", Utils.AbsoluteWebRoot);
            var email = (string)null;
            var userName = string.Empty;
            var ap = this.UserProfile();

            if (ap != null)
            {
                userName = ap.DisplayName;
                if (string.IsNullOrEmpty(ap.PhotoUrl))
                {
                    if (!string.IsNullOrEmpty(ap.EmailAddress) && BlogSettings.Instance.Avatar != "none")
                    {
                        email = ap.EmailAddress;
                        src = null;
                    }
                }
                else
                {
                    src = ap.PhotoUrl;
                }
            }

            return Avatar.GetAvatarImageTag(28, email, null, src, userName);
        }

        /// <summary>
        /// Gets the current user's profile.
        /// </summary>
        /// <returns>
        /// An Author Profile.
        /// </returns>
        protected AuthorProfile UserProfile()
        {
            try
            {
                return AuthorProfile.GetProfile(Security.CurrentUser.Identity.Name);
            }
            catch (Exception e)
            {
                Utils.Log(e.Message);
                return null;
            }
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> object that contains the event data.</param>
        protected override void OnInit(EventArgs e)
        {
            if (!Security.IsAuthenticated)
            {
                Security.RedirectForUnauthorizedRequest();
                return;
            }

            if (Security.IsAuthenticated)
            {
                aLogin.InnerText = Resources.labels.logoff;
                aLogin.HRef = Utils.RelativeWebRoot + "Account/login.aspx?logoff";
            }
            else
            {
                aLogin.HRef = Utils.RelativeWebRoot + "Account/login.aspx";
                aLogin.InnerText = Resources.labels.login;
            }

            Page.Header.DataBind();

            phRecycleBin.Visible = Security.IsAuthorizedTo(Rights.AccessAdminPages);
            base.OnInit(e);
        }

        protected string RecycleClass()
        {
            if (BlogEngine.Core.Json.JsonTrashList.IsTrashEmpty())
                return "empty";
            return "full";
        }

        #endregion
    }
}