namespace BlogEngine.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Security.Principal;
    using System.Web.Security;

    public class CustomPrincipal : IPrincipal
    {
        private IIdentity _identity;
        public IIdentity Identity
        {
            get { return _identity; }
        }

        public bool IsInRole(string roleName)
        { 
            if (Identity == null || !Identity.IsAuthenticated || string.IsNullOrEmpty(Identity.Name))
                return false;

            // Note: Cannot use "Security.CurrentUser.IsInRole" or anything similar since
            // Security.CurrentUser.IsInRole will look to this IsInRole() method here --
            // resulting in an endless loop.  Need to query the role provider directly.

            return Roles.IsUserInRole(Identity.Name, roleName);
        }

        public CustomPrincipal(IIdentity identity)
        {
            _identity = identity;
        }
    }
}
