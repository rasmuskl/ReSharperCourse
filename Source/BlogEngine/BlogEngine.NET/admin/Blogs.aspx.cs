using BlogEngine.Core.Json;

namespace Admin
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Collections.Specialized;
    using System.Web.Security;
    using BlogEngine.Core;

    public partial class Blogs : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Security.DemandUserHasRight(Rights.AccessAdminPages, true);
            if (!Blog.CurrentInstance.IsPrimary)
            {
                Security.RedirectForUnauthorizedRequest();
                return;
            }
        }
    }
}