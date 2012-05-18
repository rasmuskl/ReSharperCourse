// --------------------------------------------------------------------------------------------------------------------
// <summary>
//   The admin pages referrers.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Admin.Tracking
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Web.UI.WebControls;
    using System.Web.Services;
    using System.Threading;
    using App_Code;
    using BlogEngine.Core;
    using BlogEngine.Core.Json;

    using Resources;

    using Page = System.Web.UI.Page;

    /// <summary>
    /// The admin pages referrers.
    /// </summary>
    public partial class Referrers : Page
    {
        #region Methods

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init"/> event to initialize the page.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> that contains the event data.</param>
        protected override void OnInit(EventArgs e)
        {
            WebUtils.CheckRightsForAdminSettingsPage(false);

            infoPanel.Visible = false;
            if (BlogSettings.Instance.EnableReferrerTracking)
            {
                this.BindDays();
                this.BindReferrers();
            }
            else
            {
                this.ddlDays.Enabled = false;
                infoPanel.InnerText = "Referrers disabled. If you want to enable referrer tracking open settings from the link in the top right corner of the page.";
                infoPanel.Visible = true;
                referrersPanel.Visible = false;
                possibleSmapPanel.Visible = false;
            }

            this.txtNumberOfDays.Text = BlogSettings.Instance.NumberOfReferrerDays.ToString();
            this.cbEnableReferrers.Checked = BlogSettings.Instance.EnableReferrerTracking;

            this.ddlDays.SelectedIndexChanged += this.DdlDaysSelectedIndexChanged;
            this.Page.Title = labels.referrers;

            base.OnInit(e);
        }

        /// <summary>
        /// The bind days.
        /// </summary>
        private void BindDays()
        {
            var sortedDates = new List<DateTime>(Referrer.ReferrersByDay.Keys);

            sortedDates.Sort((firstPair, nextPair) => firstPair.CompareTo(nextPair) * -1);

            this.ddlDays.DataSource = sortedDates;
            this.ddlDays.DataBind();
            foreach (var item in
                this.ddlDays.Items.Cast<ListItem>().Where(item => item.Text == DateTime.Today.ToShortDateString()))
            {
                item.Selected = true;
            }
        }

        /// <summary>
        /// The bind referrers.
        /// </summary>
        private void BindReferrers()
        {
            if (!(this.ddlDays.SelectedIndex >= 0 & Referrer.Referrers.Count > 0))
            {
                infoPanel.InnerText = "There are no referrers.";
                infoPanel.Visible = true;
                referrersPanel.Visible = false;
                possibleSmapPanel.Visible = false;
                return;
            }

            var referrers = Referrer.ReferrersByDay[DateTime.Parse(this.ddlDays.SelectedItem.Text)];
            if (referrers == null)
            {
                return;
            }

            var table = new DataTable();
            table.Columns.Add("url", typeof(string));
            table.Columns.Add("shortUrl", typeof(string));
            table.Columns.Add("target", typeof(string));
            table.Columns.Add("shortTarget", typeof(string));
            table.Columns.Add("hits", typeof(int));

            var spamTable = table.Clone();

            foreach (var refer in referrers)
            {
                DataRow tableRow;
                if (refer.PossibleSpam)
                {
                    tableRow = spamTable.NewRow();
                    this.PopulateRow(tableRow, refer);
                    spamTable.Rows.Add(tableRow);
                }
                else
                {
                    tableRow = table.NewRow();
                    this.PopulateRow(tableRow, refer);
                    table.Rows.Add(tableRow);
                }
            }

            BindTable(table, this.grid);
            BindTable(spamTable, this.spamGrid);
        }

        /// <summary>
        /// Binds the table.
        /// </summary>
        /// <param name="table">
        /// The table.
        /// </param>
        /// <param name="gridview">
        /// The gridview.
        /// </param>
        private static void BindTable(DataTable table, GridView gridview)
        {
            var total = table.Compute("sum(hits)", null).ToString();

            var view = new DataView(table) { Sort = "hits desc" };

            gridview.DataSource = view;
            gridview.DataBind();

            if (gridview.Rows.Count > 0)
            {
                gridview.FooterRow.Cells[0].Text = labels.Referrers_BindTable_Total;
                gridview.FooterRow.Cells[gridview.FooterRow.Cells.Count - 1].Text = total;
            }
            else
            {
            }

            PaintRows(gridview, 3);
        }

        /// <summary>
        /// Makes the short URL.
        /// </summary>
        /// <param name="url">The text to shorten.</param>
        /// <returns>The shortened text.</returns>
        private string MakeShortUrl(string url)
        {
            return url.Length > 150
                       ? string.Format("{0}...", url.Substring(0, 150))
                       : this.Server.HtmlEncode(url.Replace("http://", string.Empty).Replace("www.", string.Empty));
        }

        /// <summary>
        /// Paints the background color of the alternate rows
        ///     in the gridview.
        /// </summary>
        /// <param name="gridview">
        /// The gridview.
        /// </param>
        /// <param name="alternateRows">
        /// The alternate Rows.
        /// </param>
        private static void PaintRows(GridView gridview, int alternateRows)
        {
            if (gridview.Rows.Count == 0)
            {
                return;
            }

            var count = 0;
            for (var i = 0; i < gridview.Controls[0].Controls.Count - 1; i++)
            {
                if (count > alternateRows)
                {
                    ((WebControl)gridview.Controls[0].Controls[i]).CssClass = "alt";
                }

                count++;

                if (count == alternateRows + alternateRows + 1)
                {
                    count = 1;
                }
            }
        }

        /// <summary>
        /// Populates the row.
        /// </summary>
        /// <param name="tableRow">
        /// The table row.
        /// </param>
        /// <param name="refer">
        /// The refer.
        /// </param>
        private void PopulateRow(DataRow tableRow, Referrer refer)
        {
            tableRow["url"] = this.Server.HtmlEncode(refer.ReferrerUrl.ToString());
            tableRow["shortUrl"] = this.MakeShortUrl(refer.ReferrerUrl.ToString());
            tableRow["target"] = this.Server.HtmlEncode(refer.Url.ToString());
            tableRow["shortTarget"] = this.MakeShortUrl(refer.Url.ToString());
            tableRow["hits"] = refer.Count;
        }


        [WebMethod]
        public static JsonResponse Save(string enable, string days)
        {
            var response = new JsonResponse { Success = false };

            if (!WebUtils.CheckRightsForAdminSettingsPage(true))
            {
                response.Message = "Not authorized";
                return response;
            }

            try
            {
                BlogSettings.Instance.EnableReferrerTracking = bool.Parse(enable);
                BlogSettings.Instance.NumberOfReferrerDays = int.Parse(days);
                BlogSettings.Instance.Save();
                response.Success = true;
                response.Message = string.Format("Settings saved");
                return response;
            }
            catch (Exception ex)
            {
                Utils.Log(string.Format("admin.Tracking.referrers.Save(): {0}", ex.Message));
                response.Message = string.Format("Could not save settings: {0}", ex.Message);
                return response;
            }
        }

        /// <summary>
        /// Handles the CheckedChanged event of the cbEnableReferrers control.
        /// </summary>
        /// <param name="sender">
        /// The source of the event.
        /// </param>
        /// <param name="e">
        /// The <see cref="System.EventArgs"/> instance containing the event data.
        /// </param>
        private void CbEnableReferrersCheckedChanged(object sender, EventArgs e)
        {
            if (this.cbEnableReferrers.Checked)
            {
                this.BindDays();
                this.BindReferrers();
            }
            else
            {
                this.ddlDays.Enabled = false;
            }

            BlogSettings.Instance.EnableReferrerTracking = this.cbEnableReferrers.Checked;
            BlogSettings.Instance.Save();
        }

        /// <summary>
        /// Handles the SelectedIndexChanged event of the ddlDays control.
        /// </summary>
        /// <param name="sender">
        /// The source of the event.
        /// </param>
        /// <param name="e">
        /// The <see cref="System.EventArgs"/> instance containing the event data.
        /// </param>
        private void DdlDaysSelectedIndexChanged(object sender, EventArgs e)
        {
            this.BindReferrers();
        }

        #endregion
    }
}
