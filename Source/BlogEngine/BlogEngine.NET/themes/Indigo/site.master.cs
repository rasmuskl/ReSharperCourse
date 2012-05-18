using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using BlogEngine.Core;


public partial class IndigoSite : System.Web.UI.MasterPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        bool isAuthenticated = Security.IsAuthenticated;
        hypLoginStatus.Text = isAuthenticated ? "Sign Out" : "Sign In";
        hypLoginStatus.NavigateUrl = string.Format("{0}Account/login.aspx{1}", Utils.RelativeWebRoot, isAuthenticated ? "?logoff" : string.Empty);
    }
}
