namespace Admin.Users
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Services;
    using BlogEngine.Core;
    using BlogEngine.Core.Json;

    using Page = System.Web.UI.Page;

    /// <summary>
    /// The admin account roles.
    /// </summary>
    public partial class Roles : Page
    {
        #region Public Methods

        protected void Page_Load(object sender, EventArgs e)
        {
            Security.DemandUserHasRight(AuthorizationCheck.HasAll, true, new[] {
                BlogEngine.Core.Rights.AccessAdminPages,
                BlogEngine.Core.Rights.ViewRoles });

        }

        /// <summary>
        /// Gets the roles.
        /// </summary>
        /// <returns>The roles.</returns>
        [WebMethod]
        public static List<JsonRole> GetRoles()
        {
            if (!Security.IsAuthorizedTo(BlogEngine.Core.Rights.ViewRoles))
                return new List<JsonRole>();

            var roles = new List<JsonRole>();
            roles.AddRange(System.Web.Security.Roles.GetAllRoles().Select(r => new JsonRole { RoleName = r, IsSystemRole = Security.IsSystemRole(r) }));
            roles.Sort((r1, r2) => string.Compare(r1.RoleName, r2.RoleName));

            return roles;
        }

        #endregion

        #region Methods


        #endregion
    }
}