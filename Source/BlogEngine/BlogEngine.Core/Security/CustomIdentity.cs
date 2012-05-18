namespace BlogEngine.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Web.Security;
    using System.Security.Principal;
    using System.Runtime.Serialization;
    using System.Reflection;
    using System.Security;

    // Need to inherit from MarshalByRefObject to prevent runtime errors with Cassini
    // when using a custom identity.
    public class CustomIdentity : MarshalByRefObject, IIdentity
    {
        public string AuthenticationType
        {
            get { return "BlogEngine.NET Custom Identity"; }
        }

        private bool _isAuthenticated;
        public bool IsAuthenticated
        {
            get { return _isAuthenticated; }
        }

        private string _name;
        public string Name
        {
            get { return _name; }
        }

        public CustomIdentity(string username, bool isAuthenticated)
        {
            _name = username;
            _isAuthenticated = isAuthenticated;
        }

        public CustomIdentity(string username, string password)
        {
            if (Utils.StringIsNullOrWhitespace(username))
                throw new ArgumentNullException("username");

            if (Utils.StringIsNullOrWhitespace(password))
                throw new ArgumentNullException("password");

            if (!Membership.ValidateUser(username, password)) { return; }

            _isAuthenticated = true;
            _name = username;
        }
    }
}
