namespace admin.Settings
{
    using System;
    using System.IO;
    using BlogEngine.Core.API.BlogML;
    using App_Code;

    public partial class Import : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            WebUtils.CheckRightsForAdminSettingsPage(false);
        }

        /// <summary>
        /// Handles the Click event of the btnBlogMLImport control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void BtnBlogMlImportClick(object sender, EventArgs e)
        {
            var fileName = txtUploadFile.FileName;

            if (string.IsNullOrEmpty(fileName))
            {
                //this.Master.SetStatus("warning", "File name is required");
            }
            else
            {
                var reader = new BlogReader();

                var stm = txtUploadFile.FileContent;
                var rdr = new StreamReader(stm);
                reader.XmlData = rdr.ReadToEnd();

                string tmpl = "<script language=\"JavaScript\">ShowStatus('{0}', '{1}');</script>";

                if(reader.Import())
                {
                    tmpl = string.Format(tmpl, "success", reader.Message);
                }else
                {
                    tmpl = string.Format(tmpl, "warning", reader.Message.Replace("'", "`").Replace("\"", "`").Replace(Environment.NewLine, " "));
                }

                ClientScript.RegisterStartupScript(this.GetType(), "ImportDone", tmpl);
            }
        }
    }
}