namespace Admin.Posts
{
    using System;
    using App_Code;

    public partial class Tags : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            WebUtils.CheckRightsForAdminPostPages(false);
        }
    }
}