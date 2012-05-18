using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Security;
using System.Diagnostics;
using System.Security;
using System.Security.Principal;

namespace BlogEngine.Core
{
    /// <summary>
    /// Class to provide a unified area of authentication/authorization checking.
    /// </summary>
    public partial class Security : IHttpModule
    {
        static Security()
        {

        }

        /// <summary>
        /// Disposes of the resources (other than memory) used by the module that implements <see cref="T:System.Web.IHttpModule"/>.
        /// </summary>
        public void Dispose()
        {
            // Nothing to dispose
        }

        /// <summary>
        /// Initializes a module and prepares it to handle requests.
        /// </summary>
        /// <param name="context">An <see cref="T:System.Web.HttpApplication"/> that provides access to the methods, properties, and events common to all application objects within an ASP.NET application</param>
        public void Init(HttpApplication context)
        {
            context.AuthenticateRequest += ContextAuthenticateRequest;
        }

        /// <summary>
        /// Handles the AuthenticateRequest event of the context control.
        /// </summary>
        /// <param name="sender">
        /// The source of the event.
        /// </param>
        /// <param name="e">
        /// The <see cref="System.EventArgs"/> instance containing the event data.
        /// </param>
        private static void ContextAuthenticateRequest(object sender, EventArgs e)
        {
            var context = ((HttpApplication)sender).Context;

            // FormsAuthCookieName is a custom cookie name based on the current instance.
            HttpCookie authCookie = context.Request.Cookies[FormsAuthCookieName];
            if (authCookie != null)
            {
                Blog blog = Blog.CurrentInstance;

                FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);

                // for extra security, make sure the UserData matches the current blog instance.
                // this would prevent a cookie name change for a forms auth cookie encrypted in
                // the same application (different blog) as being valid for this blog instance.
                if (authTicket != null && !string.IsNullOrWhiteSpace(authTicket.UserData) && authTicket.UserData.Equals(Blog.CurrentInstance.Id.ToString(), StringComparison.OrdinalIgnoreCase))
                {
                    CustomIdentity identity = new CustomIdentity(authTicket.Name, true);
                    CustomPrincipal principal = new CustomPrincipal(identity);

                    context.User = principal;
                    return;
                }
            }

            // need to create an empty/unauthenticated user to assign to context.User.
            CustomIdentity unauthIdentity = new CustomIdentity(string.Empty, false);
            CustomPrincipal unauthPrincipal = new CustomPrincipal(unauthIdentity);
            context.User = unauthPrincipal;
        }

        /// <summary>
        /// Name of the Forms authentication cookie for the current blog instance.
        /// </summary>
        public static string FormsAuthCookieName
        {
            get
            {
                return FormsAuthentication.FormsCookieName + "-" + Blog.CurrentInstance.Id.ToString();
            }
        }

        public static void SignOut()
        {
            // using a custom cookie name based on the current blog instance.
            HttpCookie cookie = new HttpCookie(FormsAuthCookieName, string.Empty);
            cookie.Expires = DateTime.Now.AddYears(-3);
            HttpContext.Current.Response.Cookies.Add(cookie);
        }

        public static bool AuthenticateUser(string username, string password, bool rememberMe)
        {
            string un = (username ?? string.Empty).Trim();
            string pw = (password ?? string.Empty).Trim();

            if (!string.IsNullOrWhiteSpace(un) && !string.IsNullOrWhiteSpace(pw))
            {
                bool isValidated = Membership.ValidateUser(un, pw);

                if (isValidated)
                {
                    HttpContext context = HttpContext.Current;
                    DateTime expirationDate = DateTime.Now.Add(FormsAuthentication.Timeout);

                    FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(
                        1,
                        un,
                        DateTime.Now,
                        expirationDate,
                        rememberMe,
                        Blog.CurrentInstance.Id.ToString(),
                        FormsAuthentication.FormsCookiePath
                    );

                    string encryptedTicket = FormsAuthentication.Encrypt(ticket);

                    // setting a custom cookie name based on the current blog instance.
                    // if !rememberMe, set expires to DateTime.MinValue which makes the
                    // cookie a browser-session cookie expiring when the browser is closed.
                    HttpCookie cookie = new HttpCookie(FormsAuthCookieName, encryptedTicket);
                    cookie.Expires = rememberMe ? expirationDate : DateTime.MinValue;
                    context.Response.Cookies.Set(cookie);

                    string returnUrl = context.Request.QueryString["returnUrl"];

                    // ignore Return URLs not beginning with a forward slash, such as remote sites.
                    if (string.IsNullOrWhiteSpace(returnUrl) || !returnUrl.StartsWith("/"))
                        returnUrl = null;

                    if (!string.IsNullOrWhiteSpace(returnUrl))
                    {
                        context.Response.Redirect(returnUrl);
                    }
                    else
                    {
                        context.Response.Redirect(Utils.RelativeWebRoot);
                    }

                    return true;
                }
            }

            return false;
        }

        #region "Properties"

        /// <summary>
        /// If the current user is authenticated, returns the current MembershipUser. If not, returns null. This is just a shortcut to Membership.GetUser().
        /// </summary>
        public static MembershipUser CurrentMembershipUser
        {
            get
            {
                return Membership.GetUser();
            }
        }

        /// <summary>
        /// Gets the current user for the current HttpContext.
        /// </summary>
        /// <remarks>
        /// This should always return HttpContext.Current.User. That value and Thread.CurrentPrincipal can't be
        /// guaranteed to always be the same value, as they can be set independently from one another. Looking
        /// through the .Net source, the System.Web.Security.Roles class also returns the HttpContext's User.
        /// </remarks>
        public static System.Security.Principal.IPrincipal CurrentUser
        {
            get
            {
                return HttpContext.Current.User;
            }
        }

        /// <summary>
        /// Gets whether the current user is logged in.
        /// </summary>
        public static bool IsAuthenticated
        {
            get
            {
                return Security.CurrentUser.Identity.IsAuthenticated;
            }
        }

        /// <summary>
        /// Gets whether the currently logged in user is in the administrator role.
        /// </summary>
        public static bool IsAdministrator
        {
            get
            {
                return (Security.IsAuthenticated && Security.CurrentUser.IsInRole(BlogConfig.AdministratorRole));
            }
        }

        /// <summary>
        /// Returns an IEnumerable of Rights that belong to the ecurrent user.
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<Right> CurrentUserRights()
        {
            return Right.GetRights(Security.GetCurrentUserRoles());
        }

        #endregion

        #region "Public Methods"

        /// <summary>
        /// If the current user does not have the requested right, either redirects to the login page,
        /// or throws a SecurityException.
        /// </summary>
        /// <param name="right"></param>
        /// <param name="redirectToLoginPage">
        /// If true and user does not have rights, redirects to the login page.
        /// If false and user does not have rights, throws a security exception.
        /// </param>
        public static void DemandUserHasRight(Rights right, bool redirectToLoginPage)
        {
            DemandUserHasRight(AuthorizationCheck.HasAny, redirectToLoginPage, new[] { right });
        }

        /// <summary>
        /// If the current user does not have the requested rights, either redirects to the login page,
        /// or throws a SecurityException.
        /// </summary>
        /// <param name="authCheck"></param>
        /// <param name="redirectIfUnauthorized">
        /// If true and user does not have rights, redirects to the login page or homepage.
        /// If false and user does not have rights, throws a security exception.
        /// </param>
        /// <param name="rights"></param>
        public static void DemandUserHasRight(AuthorizationCheck authCheck, bool redirectIfUnauthorized, params Rights[] rights)
        {
            if (!IsAuthorizedTo(authCheck, rights))
            {
                if (redirectIfUnauthorized)
                {
                    RedirectForUnauthorizedRequest();
                }
                else
                {
                    throw new SecurityException("User doesn't have the right to perform this");
                }
            }
        }

        public static void RedirectForUnauthorizedRequest()
        {
            HttpContext context = HttpContext.Current;
            Uri referrer = context.Request.UrlReferrer;
            bool isFromLoginPage = referrer != null && referrer.LocalPath.IndexOf("/Account/login.aspx", StringComparison.OrdinalIgnoreCase) != -1;

            // If the user was just redirected from the login page to the current page,
            // we will then redirect them to the homepage, rather than back to the
            // login page to prevent confusion.
            if (isFromLoginPage)
            {
                context.Response.Redirect(Utils.RelativeWebRoot);
            }
            else
            {
                context.Response.Redirect(string.Format("{0}Account/login.aspx?ReturnURL={1}", Utils.RelativeWebRoot, HttpUtility.UrlPathEncode(context.Request.RawUrl)));
            }
        }

        /// <summary>
        /// Returns whether or not the current user has the passed in Right.
        /// </summary>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool IsAuthorizedTo(Rights right)
        {
            return Right.HasRight(right, Security.GetCurrentUserRoles());
        }

        /// <summary>
        /// Returns whether the current user passes authorization on the rights based on the given AuthorizationCheck.
        /// </summary>
        /// <param name="authCheck"></param>
        /// <param name="rights"></param>
        /// <returns></returns>
        public static bool IsAuthorizedTo(AuthorizationCheck authCheck, IEnumerable<Rights> rights)
        {
            if (rights.Count() == 0)
            {
                // Always return false for this. If there's a mistake where authorization
                // is being checked for on an empty collection, we don't want to return 
                // true.
                return false;
            }
            else
            {
                var roles = Security.GetCurrentUserRoles();

                if (authCheck == AuthorizationCheck.HasAny)
                {
                    foreach (var right in rights)
                    {
                        if (Right.HasRight(right, roles))
                        {
                            return true;
                        }
                    }

                    return false;
                }
                else if (authCheck == AuthorizationCheck.HasAll)
                {
                    bool authCheckPassed = true;

                    foreach (var right in rights)
                    {
                        if (!Right.HasRight(right, roles))
                        {
                            authCheckPassed = false;
                            break;
                        }
                    }
                    return authCheckPassed;
                }
                else
                {
                    throw new NotSupportedException();
                }

            }
        }

        /// <summary>
        /// Returns whether a role is a System role.
        /// </summary>
        /// <param name="roleName">The name of the role.</param>
        /// <returns>true if the roleName is a system role, otherwiser false</returns>
        public static bool IsSystemRole(string roleName)
        {
            if (roleName.Equals(BlogConfig.AdministratorRole, StringComparison.OrdinalIgnoreCase) ||
                roleName.Equals(BlogConfig.AnonymousRole, StringComparison.OrdinalIgnoreCase) ||
                roleName.Equals(BlogConfig.EditorsRole, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Returns whether the current user passes authorization on the rights based on the given AuthorizationCheck.
        /// </summary>
        /// <param name="authCheck"></param>
        /// <param name="rights"></param>
        /// <returns></returns>
        public static bool IsAuthorizedTo(AuthorizationCheck authCheck, params Rights[] rights)
        {
            return IsAuthorizedTo(authCheck, rights.ToList());
        }

        #endregion

        #region "Methods"

        /// <summary>
        /// Helper method that returns the correct roles based on authentication.
        /// </summary>
        /// <returns></returns>
        public static string[] GetCurrentUserRoles()
        {
            if (!IsAuthenticated)
            {
                // This needs to be recreated each time, because it's possible 
                // that the array can fall into the wrong hands and then someone
                // could alter it. 
                return new[] { BlogConfig.AnonymousRole };
            }
            else
            {
                return Roles.GetRolesForUser();
            }
        }

        /// <summary>
        /// Impersonates a user for the duration of the HTTP request.
        /// </summary>
        /// <param name="username">The username</param>
        /// <param name="password">The password</param>
        /// <returns>True if the credentials are correct and impersonation succeeds</returns>
        public static bool ImpersonateUser(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                return false;

            CustomIdentity identity = new CustomIdentity(username, password);
            if (!identity.IsAuthenticated) { return false; }

            CustomPrincipal principal = new CustomPrincipal(identity);

            // Make the custom principal be the user for the rest of this request.
            HttpContext.Current.User = principal;

            return true;
        }

        #endregion
    }


    /// <summary>
    /// Enum for setting how rights should be checked for.
    /// </summary>
    public enum AuthorizationCheck
    {
        /// <summary>
        /// A user will be considered authorized if they have any of the given Rights.
        /// </summary>
        HasAny,

        /// <summary>
        /// A user will be considered authorized if they have all of the given Rights.
        /// </summary>
        HasAll
    }

}
