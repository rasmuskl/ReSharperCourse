namespace admin.Comments
{
    using System;
    using System.Data;
    using System.Web.UI.WebControls;
    using BlogEngine.Core;
    using BlogEngine.Core.Web.Extensions;
    using App_Code;

    public partial class Settings : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            WebUtils.CheckRightsForAdminSettingsPage(false);

            if (!IsPostBack)
            {
                BindSettings();
            }

            Page.MaintainScrollPositionOnPostBack = true;
            Page.Title = Resources.labels.comments;

            btnSave.Click += btnSave_Click;
            btnSave.Text = Resources.labels.saveSettings;
            btnSave2.Click += btnSave_Click;
            btnSave2.Text = Resources.labels.saveSettings;
        }

        private void BindSettings()
        {
            //-----------------------------------------------------------------------
            // Bind Comments settings
            //-----------------------------------------------------------------------
            cbEnableComments.Checked = BlogSettings.Instance.IsCommentsEnabled;
            cbEnableCommentNesting.Checked = BlogSettings.Instance.IsCommentNestingEnabled;
            cbEnableCountryInComments.Checked = BlogSettings.Instance.EnableCountryInComments;
            cbEnableWebsiteInComments.Checked = BlogSettings.Instance.EnableWebsiteInComments;
            cbEnableCoComment.Checked = BlogSettings.Instance.IsCoCommentEnabled;
            cbShowLivePreview.Checked = BlogSettings.Instance.ShowLivePreview;
            ddlCloseComments.SelectedValue = BlogSettings.Instance.DaysCommentsAreEnabled.ToString();
            cbEnableCommentsModeration.Checked = BlogSettings.Instance.EnableCommentsModeration;
            rblAvatar.SelectedValue = BlogSettings.Instance.Avatar;
            cbEnablePingBackSend.Checked = BlogSettings.Instance.EnablePingBackSend;
            cbEnablePingBackReceive.Checked = BlogSettings.Instance.EnablePingBackReceive;
            cbEnableTrackBackSend.Checked = BlogSettings.Instance.EnableTrackBackSend;
            cbEnableTrackBackReceive.Checked = BlogSettings.Instance.EnableTrackBackReceive;
            txtThumbProvider.Text = BlogSettings.Instance.ThumbnailServiceApi;
            ddlCommentsPerPage.SelectedValue = BlogSettings.Instance.CommentsPerPage.ToString();

            // disqus
            cbEnableDisqus.Checked = BlogSettings.Instance.ModerationType == BlogSettings.Moderation.Disqus;
            string discusName = "YourDisqusWebsite";
            if (BlogSettings.Instance.DisqusWebsiteName != null)
                discusName = BlogSettings.Instance.DisqusWebsiteName;

            txtDisqusName.Text = discusName;
            cbDisqusDevMode.Checked = BlogSettings.Instance.DisqusDevMode;
            cbDisqusAddToPages.Checked = BlogSettings.Instance.DisqusAddCommentsToPages;
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            //-----------------------------------------------------------------------
            // Set Comments settings
            //-----------------------------------------------------------------------
            BlogSettings.Instance.IsCommentsEnabled = cbEnableComments.Checked;
            BlogSettings.Instance.IsCommentNestingEnabled = cbEnableCommentNesting.Checked;
            BlogSettings.Instance.EnableCountryInComments = cbEnableCountryInComments.Checked;
            BlogSettings.Instance.EnableWebsiteInComments = cbEnableWebsiteInComments.Checked;
            BlogSettings.Instance.IsCoCommentEnabled = cbEnableCoComment.Checked;
            BlogSettings.Instance.ShowLivePreview = cbShowLivePreview.Checked;

            BlogSettings.Instance.DaysCommentsAreEnabled = int.Parse(ddlCloseComments.SelectedValue);
            BlogSettings.Instance.EnableCommentsModeration = cbEnableCommentsModeration.Checked;
            BlogSettings.Instance.Avatar = rblAvatar.SelectedValue;
            BlogSettings.Instance.EnableTrackBackSend = cbEnableTrackBackSend.Checked;
            BlogSettings.Instance.EnableTrackBackReceive = cbEnableTrackBackReceive.Checked;
            BlogSettings.Instance.EnablePingBackSend = cbEnablePingBackSend.Checked;
            BlogSettings.Instance.EnablePingBackReceive = cbEnablePingBackReceive.Checked;
            BlogSettings.Instance.ThumbnailServiceApi = txtThumbProvider.Text;
            BlogSettings.Instance.CommentsPerPage = int.Parse(ddlCommentsPerPage.SelectedValue);

            // disqus 
            BlogSettings.Instance.ModerationType = cbEnableDisqus.Checked ? BlogSettings.Moderation.Disqus : BlogSettings.Moderation.Auto;
            BlogSettings.Instance.DisqusWebsiteName = txtDisqusName.Text.Length > 250 ? txtDisqusName.Text.Substring(0, 250) : txtDisqusName.Text;
            BlogSettings.Instance.DisqusDevMode = cbDisqusDevMode.Checked;
            BlogSettings.Instance.DisqusAddCommentsToPages = cbDisqusAddToPages.Checked;

            //-----------------------------------------------------------------------
            //  Persist settings
            //-----------------------------------------------------------------------
            BlogSettings.Instance.Save();

            Response.Redirect(Request.RawUrl, true);
        }
    }
}