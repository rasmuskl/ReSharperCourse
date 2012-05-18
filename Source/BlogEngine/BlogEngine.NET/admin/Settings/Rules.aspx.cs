namespace admin.Settings
{
    using System;
    using System.Collections.Generic;
    using System.Web.UI.WebControls;
    using BlogEngine.Core;
    using BlogEngine.Core.Web.Extensions;
    using System.Web.Services;
    using BlogEngine.Core.Json;
    using App_Code;

    public partial class Rules : System.Web.UI.Page
    {
        static protected ExtensionSettings Filters;

        /// <summary>
        /// Usually filter implemented as extension and can be turned
        /// on and off. If it is not extension, defaulted to enabled.
        /// </summary>
        /// <param name="filter">Filter (extension) name</param>
        /// <returns>True if enabled</returns>
        public bool CustomFilterEnabled(string filter)
        {
            var ext = ExtensionManager.GetExtension(filter);
            return ext == null ? true : ext.Enabled;
        }

        [WebMethod]
        public static List<JsonCustomFilter> GetCustomFilters()
        {
            if (!Security.IsAuthorizedTo(Rights.AccessAdminSettingsPages))
                return new List<JsonCustomFilter>();

            var customFilters = JsonCustomFilterList.GetCustomFilters();

            customFilters.Sort((f1, f2) => string.Compare(f1.Name, f2.Name));

            return customFilters;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            WebUtils.CheckRightsForAdminSettingsPage(false);

            Filters = ExtensionManager.GetSettings("MetaExtension", "BeCommentFilters");

            if (!IsPostBack)
            {
                BindFilters();
            }

            Page.MaintainScrollPositionOnPostBack = true;
            Page.Title = Resources.labels.comments;

            btnSave.Click += btnSave_Click;
            btnSave.Text = Resources.labels.saveSettings;
            btnSave2.Click += btnSave_Click;
            btnSave2.Text = Resources.labels.saveSettings;
        }

        protected void BindFilters()
        {
            gridFilters.DataKeyNames = new string[] { Filters.KeyField };
            gridFilters.DataSource = Filters.GetDataTable();
            gridFilters.DataBind();

            // rules
            cbTrustAuthenticated.Checked = BlogSettings.Instance.TrustAuthenticatedUsers;
            ddWhiteListCount.SelectedValue = BlogSettings.Instance.CommentWhiteListCount.ToString();
            ddBlackListCount.SelectedValue = BlogSettings.Instance.CommentBlackListCount.ToString();
            cbReportMistakes.Checked = BlogSettings.Instance.CommentReportMistakes;
            cbBlockOnDelete.Checked = BlogSettings.Instance.BlockAuthorOnCommentDelete;
            cbAddIpToWhitelistFilterOnApproval.Checked = BlogSettings.Instance.AddIpToWhitelistFilterOnApproval;
            cbAddIpToBlacklistFilterOnRejection.Checked = BlogSettings.Instance.AddIpToBlacklistFilterOnRejection;
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            // rules
            BlogSettings.Instance.TrustAuthenticatedUsers = cbTrustAuthenticated.Checked;
            BlogSettings.Instance.CommentWhiteListCount = int.Parse(ddWhiteListCount.SelectedValue);
            BlogSettings.Instance.CommentBlackListCount = int.Parse(ddBlackListCount.SelectedValue);

            BlogSettings.Instance.CommentReportMistakes = cbReportMistakes.Checked;
            BlogSettings.Instance.BlockAuthorOnCommentDelete = cbBlockOnDelete.Checked;
            BlogSettings.Instance.AddIpToWhitelistFilterOnApproval = cbAddIpToWhitelistFilterOnApproval.Checked;
            BlogSettings.Instance.AddIpToBlacklistFilterOnRejection = cbAddIpToBlacklistFilterOnRejection.Checked;

            //-----------------------------------------------------------------------
            //  Persist settings
            //-----------------------------------------------------------------------
            BlogSettings.Instance.Save();

            Response.Redirect(Request.RawUrl, true);
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            ImageButton btn = (ImageButton)sender;
            GridViewRow grdRow = (GridViewRow)btn.Parent.Parent;

            int pageIdx = gridFilters.PageIndex;
            int pageSize = gridFilters.PageSize;
            int rowIndex = grdRow.RowIndex;

            if (pageIdx > 0) rowIndex = pageIdx * pageSize + rowIndex;


            foreach (ExtensionParameter par in Filters.Parameters)
            {
                par.DeleteValue(rowIndex);
            }

            ExtensionManager.SaveSettings("MetaExtension", Filters);
            Response.Redirect(Request.RawUrl);
        }

        protected void gridView_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gridFilters.PageIndex = e.NewPageIndex;
            BindFilters();
        }

        protected void btnAddFilter_Click(object sender, EventArgs e)
        {
            if (ValidateForm())
            {
                string id = Guid.NewGuid().ToString();
                string[] f = new string[] { id, 
                    ddAction.SelectedValue, 
                    ddSubject.SelectedValue, 
                    ddOperator.SelectedValue, 
                    txtFilter.Text };

                Filters.AddValues(f);
                ExtensionManager.SaveSettings("MetaExtension", Filters);
                Response.Redirect(Request.RawUrl);
            }
        }

        protected bool ValidateForm()
        {
            if (string.IsNullOrEmpty(txtFilter.Text))
            {
                FilterValidation.InnerHtml = "Filter is a required field";
                return false;
            }

            return true;
        }

        public static string ApprovedCnt(object total, object cought)
        {
            try
            {
                int t = int.Parse(total.ToString());
                int c = int.Parse(cought.ToString());

                int a = t - c;

                return a.ToString();
            }
            catch (Exception)
            {
                return "";
            }

        }

        protected void gridCustomFilters_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            //string filterName = e.CommandArgument.ToString();

            //if (!string.IsNullOrEmpty(filterName))
            //{
            //    // reset statistics for this filter
            //    for (int i = 0; i < _customFilters.Parameters[0].Values.Count; i++)
            //    {
            //        if (_customFilters.Parameters[0].Values[i] == filterName)
            //        {
            //            _customFilters.Parameters[2].Values[i] = "0";
            //            _customFilters.Parameters[3].Values[i] = "0";
            //            _customFilters.Parameters[4].Values[i] = "0";
            //        }
            //    }

            //    ExtensionManager.SaveSettings("MetaExtension", _customFilters);
            //    Response.Redirect(Request.RawUrl);
            //}
        }
    }
}