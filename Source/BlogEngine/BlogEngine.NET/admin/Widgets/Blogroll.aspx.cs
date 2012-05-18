namespace admin.Widgets
{
    #region Using

    using System;
    using System.Web.UI.WebControls;
    using BlogEngine.Core;
    using App_Code;

    #endregion

    public partial class Blogroll : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            WebUtils.CheckRightsForAdminSettingsPage(false);

            if (!Page.IsPostBack)
            {
                BindSettings();
                BindBlogroll();
            }

            btnSaveSettings.Text = Resources.labels.save + " " + Resources.labels.settings.ToLowerInvariant();
            btnSave.Click += btnSave_Click;
            btnSaveSettings.Click += btnSaveSettings_Click;

            Page.Title = Resources.labels.blogroll;
            btnSave.Text = Resources.labels.add;
        }

        #region Event handlers

        private void btnSaveSettings_Click(object sender, EventArgs e)
        {
            BlogSettings.Instance.BlogrollMaxLength = int.Parse(txtMaxLength.Text);
            BlogSettings.Instance.BlogrollVisiblePosts = int.Parse(ddlVisiblePosts.SelectedValue);
            BlogSettings.Instance.BlogrollUpdateMinutes = int.Parse(txtUpdateFrequency.Text);
            BlogSettings.Instance.Save();
            Response.Redirect(Request.RawUrl, true);
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            Page.Validate("addNew");  // manually invoke validation to be sure it fires.
            if (!Page.IsValid)
                return;

            AddBlog();
            Response.Redirect(Request.RawUrl, true);
        }

        #endregion

        #region Methods

        private void BindBlogroll()
        {
            grid.DataKeyNames = new string[] { "Id" };
            grid.DataSource = BlogRollItem.BlogRolls;
            grid.DataBind();
        }

        protected void validateWebUrl(object sender, ServerValidateEventArgs args)
        {
            args.IsValid = validateUrl(txtWebUrl.Text.Trim());
        }

        protected void validateFeedUrl(object sender, ServerValidateEventArgs args)
        {
            args.IsValid = validateUrl(txtFeedUrl.Text.Trim());
        }

        private bool validateUrl(string url)
        {
            if (!string.IsNullOrEmpty(url))
            {
                Uri uri;
                return Uri.TryCreate(getUrl(url), UriKind.Absolute, out uri);
            }
            return true;
        }

        private string getUrl(string url)
        {
            if (!string.IsNullOrEmpty(url) && !url.Contains("://"))
                url = "http://" + url;
            return url;
        }

        private void AddBlog()
        {
            BlogRollItem br = new BlogRollItem();
            br.Title = txtTitle.Text.Replace(@"\", "'");
            br.Description = txtDescription.Text;
            br.BlogUrl = new Uri(getUrl(txtWebUrl.Text));
            br.FeedUrl = new Uri(getUrl(txtFeedUrl.Text));
            br.Xfn = string.Empty;

            foreach (ListItem item in cblXfn.Items)
            {
                if (item.Selected)
                    br.Xfn += item.Text + " ";
            }
            if (br.Xfn.Length > 0)
            {
                br.Xfn = br.Xfn.Substring(0, br.Xfn.Length - 1);
            }

            int largestSortIndex = -1;
            foreach (BlogRollItem brExisting in BlogRollItem.BlogRolls)
            {
                if (brExisting.SortIndex > largestSortIndex)
                    largestSortIndex = brExisting.SortIndex;
            }

            br.SortIndex = largestSortIndex + 1;
            br.Save();

        }

        private void BindSettings()
        {
            txtMaxLength.Text = BlogSettings.Instance.BlogrollMaxLength.ToString();
            ddlVisiblePosts.SelectedIndex = BlogSettings.Instance.BlogrollVisiblePosts;
            txtUpdateFrequency.Text = BlogSettings.Instance.BlogrollUpdateMinutes.ToString();
        }

        #endregion

        protected void grid_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            // Don't want to handle Edit and Delete commands.
            if (!(e.CommandName.Equals("moveUp", StringComparison.OrdinalIgnoreCase) ||
                  e.CommandName.Equals("moveDown", StringComparison.OrdinalIgnoreCase)))

                return;

            // If only one item in grid, there's nothing to adjust.
            if (grid.Rows.Count < 2)
                return;

            bool moveUp = e.CommandName.Equals("moveUp", StringComparison.OrdinalIgnoreCase);

            int rowIndex = Convert.ToInt32(e.CommandArgument);

            // If already at the top, can't move any higher.
            if (moveUp && rowIndex == 0)
                return;

            // If already at the bottom, can't move any lower.
            if (!moveUp && rowIndex == (grid.Rows.Count - 1))
                return;

            Guid id = (Guid)grid.DataKeys[rowIndex].Value;
            BlogRollItem brToMove = BlogRollItem.GetBlogRollItem(id);

            Guid swapPositionWithId = (Guid)grid.DataKeys[rowIndex + (moveUp ? -1 : 1)].Value;
            BlogRollItem brToSwapPositionWith = BlogRollItem.GetBlogRollItem(swapPositionWithId);

            if (brToMove != null && brToSwapPositionWith != null)
            {
                int newSortIndex = brToSwapPositionWith.SortIndex;
                brToSwapPositionWith.SortIndex = brToMove.SortIndex;
                brToMove.SortIndex = newSortIndex;

                brToSwapPositionWith.Save();
                brToMove.Save();

                BlogRollItem.BlogRolls.Sort();
                Response.Redirect(Request.RawUrl);
            }
        }

        protected void grid_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            Guid id = (Guid)grid.DataKeys[e.RowIndex].Value;
            BlogRollItem br = BlogRollItem.GetBlogRollItem(id);
            br.Delete();
            br.Save();

            int sortIndex = -1;
            // Re-sort remaining items starting from zero to eliminate any possible gaps.
            // Need to cast BlogRollItem.BlogRolls to an array to
            // prevent errors with modifying a collection while enumerating it.
            foreach (BlogRollItem brItem in BlogRollItem.BlogRolls.ToArray())
            {
                brItem.SortIndex = ++sortIndex;
                brItem.Save();
            }

            Response.Redirect(Request.RawUrl);
        }
    }
}
