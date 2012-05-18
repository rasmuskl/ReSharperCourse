namespace Admin.Users
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Reflection;
    using BlogEngine.Core;

    public partial class Rights : System.Web.UI.Page
    {

        private string roleName;
        protected string RoleName
        {
            get { return this.roleName; }
        }

        protected override void OnInit(System.EventArgs e)
        {
            base.OnInit(e);

            Security.DemandUserHasRight(AuthorizationCheck.HasAll, true,
                BlogEngine.Core.Rights.AccessAdminPages,
                BlogEngine.Core.Rights.EditRoles);

            this.roleName = this.Request.QueryString["role"];

            // If an invalid role is requested, send the user back to the Roles page since it has a list of all the roles.
            if (Utils.StringIsNullOrWhitespace(this.roleName) || !System.Web.Security.Roles.RoleExists(this.roleName)) {
                this.Response.Redirect("~/admin/Users/Roles.aspx");
            }

        }

        protected string RolesForLoading
        {
            get
            {
                var ret = string.Empty;
                const string Ptrn = "<li class=\"role\"><a href=\"#\" onclick=\"return setRoleRights('{0}',this)\">{0} role</a></li>";
                var allRoles = System.Web.Security.Roles.GetAllRoles().Where(r => !r.Equals(this.roleName, StringComparison.OrdinalIgnoreCase));
                return allRoles.Aggregate(ret, (current, r) => current + string.Format(Ptrn, r, string.Empty));
            }
        }

        protected string GetRightsJson()
        {
            var role = this.roleName;

            if (Utils.StringIsNullOrWhitespace(role))
            {
                return "null";
            }
            else
            {
                // outer key is Category (RightCategory)
                // inner key is the Right name
                var jsonDict = new Dictionary<string, Dictionary<string, bool>>();

                // store the category for each Rights.
                var rightCategories = new Dictionary<BlogEngine.Core.Rights, string>();

                foreach (FieldInfo fi in typeof(BlogEngine.Core.Rights).GetFields(BindingFlags.Static | BindingFlags.GetField | BindingFlags.Public))
                {
                    BlogEngine.Core.Rights right = (BlogEngine.Core.Rights)fi.GetValue(null);
                    if (right != BlogEngine.Core.Rights.None)
                    {
                        RightDetailsAttribute rightDetails = null;

                        foreach (Attribute attrib in fi.GetCustomAttributes(true))
                        {
                            if (attrib is RightDetailsAttribute)
                            {
                                rightDetails = (RightDetailsAttribute)attrib;
                                break;
                            }
                        }

                        RightCategory category = rightDetails == null ? RightCategory.General : rightDetails.Category;
                        rightCategories.Add(right, category.ToString());
                    }
                }

                foreach (var right in BlogEngine.Core.Right.GetAllRights())
                {
                    // The None flag isn't meant to be set specifically, so 
                    // don't render it out.
                    if (right.Flag != BlogEngine.Core.Rights.None)
                    {
                        if (rightCategories.ContainsKey(right.Flag))
                        {
                            string categoryName = rightCategories[right.Flag];

                            if (!jsonDict.ContainsKey(categoryName))
                            {
                                jsonDict.Add(categoryName, new Dictionary<string, bool>());
                            }

                            jsonDict[categoryName].Add(right.DisplayName, right.Roles.Contains(role));
                        }
                    }
                }

                return Utils.ConvertToJson(jsonDict);
            }
        }

    }
}