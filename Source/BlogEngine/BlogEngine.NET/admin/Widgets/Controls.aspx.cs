// --------------------------------------------------------------------------------------------------------------------
// <summary>
//   The admin pages controls.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace admin.Widgets
{
    using System;
    using System.Globalization;
    using BlogEngine.Core;
    using App_Code;
    using Resources;

    /// <summary>
    /// The admin pages controls.
    /// </summary>
    public partial class Controls : System.Web.UI.Page
    {
        #region Methods

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init"/> event to initialize the page.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> that contains the event data.</param>
        protected override void OnInit(EventArgs e)
        {
            WebUtils.CheckRightsForAdminSettingsPage(false);

            BindSettings();

            btnSave.Click += BtnSaveClick;
            btnSave.Text = string.Format("{0} {1}", labels.save, labels.settings);
            btnSave2.Click += BtnSaveClick;
            btnSave2.Text = string.Format("{0} {1}", labels.save, labels.settings);
            Page.Title = labels.controls;

            base.OnInit(e);
        }

        /// <summary>
        /// Binds the settings.
        /// </summary>
        private void BindSettings()
        {
            txtNumberOfPosts.Text = BlogSettings.Instance.NumberOfRecentPosts.ToString();
            cbDisplayComments.Checked = BlogSettings.Instance.DisplayCommentsOnRecentPosts;
            cbDisplayRating.Checked = BlogSettings.Instance.DisplayRatingsOnRecentPosts;

            txtNumberOfComments.Text = BlogSettings.Instance.NumberOfRecentComments.ToString();

            txtSearchButtonText.Text = BlogSettings.Instance.SearchButtonText;
            txtCommentLabelText.Text = BlogSettings.Instance.SearchCommentLabelText;
            txtDefaultSearchText.Text = BlogSettings.Instance.SearchDefaultText;
            cbEnableCommentSearch.Checked = BlogSettings.Instance.EnableCommentSearch;
            cbShowIncludeCommentsOption.Checked = BlogSettings.Instance.ShowIncludeCommentsOption;

            txtThankMessage.Text = BlogSettings.Instance.ContactThankMessage;
            txtFormMessage.Text = BlogSettings.Instance.ContactFormMessage;
            cbEnableAttachments.Checked = BlogSettings.Instance.EnableContactAttachments;
            cbEnableRecaptcha.Checked = BlogSettings.Instance.EnableRecaptchaOnContactForm;
        }

        /// <summary>
        /// Handles the Click event of the btnSave control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void BtnSaveClick(object sender, EventArgs e)
        {
            BlogSettings.Instance.NumberOfRecentPosts = int.Parse(txtNumberOfPosts.Text, CultureInfo.InvariantCulture);
            BlogSettings.Instance.DisplayCommentsOnRecentPosts = cbDisplayComments.Checked;
            BlogSettings.Instance.DisplayRatingsOnRecentPosts = cbDisplayRating.Checked;

            BlogSettings.Instance.NumberOfRecentComments = int.Parse(
                txtNumberOfComments.Text, CultureInfo.InvariantCulture);

            BlogSettings.Instance.SearchButtonText = txtSearchButtonText.Text;
            BlogSettings.Instance.SearchCommentLabelText = txtCommentLabelText.Text;
            BlogSettings.Instance.SearchDefaultText = txtDefaultSearchText.Text;
            BlogSettings.Instance.EnableCommentSearch = cbEnableCommentSearch.Checked;
            BlogSettings.Instance.ShowIncludeCommentsOption = cbShowIncludeCommentsOption.Checked;

            BlogSettings.Instance.ContactFormMessage = txtFormMessage.Text;
            BlogSettings.Instance.ContactThankMessage = txtThankMessage.Text;
            BlogSettings.Instance.EnableContactAttachments = cbEnableAttachments.Checked;
            BlogSettings.Instance.EnableRecaptchaOnContactForm = cbEnableRecaptcha.Checked;

            BlogSettings.Instance.Save();
        }

        #endregion
    }
}