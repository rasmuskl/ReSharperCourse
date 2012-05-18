namespace Admin.Users
{
    using System;
    using BlogEngine.Core;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Security;
    using System.Web.Services;
    using BlogEngine.Core.Web.Navigation;

    /// <summary>
    /// The Users.
    /// </summary>
    public partial class Users : System.Web.UI.Page
    {
        static int listCount = 0;
        static int pageSize = BlogConfig.GenericPageSize;

        #region Public Methods

        protected void Page_Load(object sender, EventArgs e)
        {
            CheckSecurity();

            phNewUserRoles.Visible = Security.IsAuthorizedTo(BlogEngine.Core.Rights.EditOtherUsersRoles);
        }

        private static void CheckSecurity()
        {
            Security.DemandUserHasRight(AuthorizationCheck.HasAll, true, new[] {
                BlogEngine.Core.Rights.AccessAdminPages,
                BlogEngine.Core.Rights.EditOtherUsers
            });
            
        }

        /// <summary>
        /// Gets the users.
        /// </summary>
        /// <returns>The users.</returns>
        [WebMethod]
        public static List<MembershipUser> GetUsers(int page)
        {
            CheckSecurity();

            int count;
            var userCollection = Membership.Provider.GetAllUsers(0, 999, out count);
            var users = userCollection.Cast<MembershipUser>().ToList();
            users.Sort((u1, u2) => string.Compare(u1.UserName, u2.UserName));

            listCount = users.Count;

            if (users.Count < pageSize)
                return users;

            var skip = page == 1 ? 0 : page * pageSize - pageSize;
            var usersPage = users.Skip(skip).Take(pageSize).ToList();

            return usersPage;
        }

        [WebMethod]
        public static string LoadPager(int page)
        {
            CheckSecurity();

            if (listCount == 0)
                return string.Empty;

            IPager pager = new Pager(page, pageSize, listCount);

            return pager.Render(page, "LoadUsers({1})");
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets RolesList.
        /// </summary>
        protected string RolesList
        {
            get
            {
                var ret = string.Empty;
                const string Ptrn = "<input type=\"checkbox\" id=\"{0}\" class=\"chkRole\" /><span class=\"lbl\">{0}</span>";
                var allRoles = System.Web.Security.Roles.GetAllRoles().Where(r => !r.Equals(BlogConfig.AnonymousRole, StringComparison.OrdinalIgnoreCase));
                return allRoles.Aggregate(ret, (current, r) => current + string.Format(Ptrn, r, string.Empty));
            }
        }

        #endregion
    }
}