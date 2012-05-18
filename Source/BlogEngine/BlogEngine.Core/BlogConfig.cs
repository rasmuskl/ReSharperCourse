using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Configuration;

namespace BlogEngine.Core
{
    public class BlogConfig
    {

        #region FileExtension

        /// <summary>
        ///     The  file extension used for aspx pages
        /// </summary>
        public static string FileExtension
        {
            get
            {
                return WebConfigurationManager.AppSettings["BlogEngine.FileExtension"] ?? ".aspx";
            }
        }

        #endregion

        #region VirtualPath

        public static string VirtualPath
        {
            get
            {
                return WebConfigurationManager.AppSettings["BlogEngine.VirtualPath"] ?? "~/";
            }
        }
        #endregion

        #region MobileServices

        public static string MobileServices
        {
            get
            {
                //return WebConfigurationManager.AppSettings["BlogEngine.MobileDevices"] ?? @"(iemobile|iphone|ipod|android|nokia|sonyericsson|blackberry|samsung|sec\-|windows ce|motorola|mot\-|up.b|midp\-)";
                return WebConfigurationManager.AppSettings["BlogEngine.MobileDevices"];
            }
        }

        #endregion

        #region StorageLocation

        /// <summary>
        /// Storage location on web server
        /// </summary>
        /// <returns>
        /// string with virtual path to storage
        /// </returns>
        public static string StorageLocation
        {
            get
            {
                return string.IsNullOrEmpty(WebConfigurationManager.AppSettings["StorageLocation"])
                           ? @"~/app_data/"
                           : WebConfigurationManager.AppSettings["StorageLocation"];
            }
        }

        #endregion

        #region StorageLocation

        /// <summary>
        /// Gets name of the folder blog instances are stored in.
        /// </summary>
        public static string BlogInstancesFolderName
        {
            get
            {
                return WebConfigurationManager.AppSettings["BlogInstancesFolderName"] ?? "blogs";
            }
        }

        #endregion

        #region AdministratorRole

        /// <summary>
        ///     The role that has administrator persmissions
        /// </summary>
        public static string AdministratorRole
        {
            get
            {
                return WebConfigurationManager.AppSettings["BlogEngine.AdminRole"] ?? "administrators";
            }
        }
        #endregion

        #region AnonymousRole

        /// <summary>
        /// The role that represents all non-authenticated users.
        /// </summary>
        public static string AnonymousRole
        {
            get
            {
                return (WebConfigurationManager.AppSettings["BlogEngine.AnonymousRole"] ?? "Anonymous");
            }
        }

        #endregion

        #region EditorsRole

        /// <summary>
        /// The role that represents all non-authenticated users.
        /// </summary>
        public static string EditorsRole
        {
            get
            {
                return (WebConfigurationManager.AppSettings["BlogEngine.EditorsRole"] ?? "Editors");
            }
        }

        #endregion

        #region IsSystemRole

        /// <summary>
        /// Returns whether a role is a System role.
        /// </summary>
        /// <param name="roleName">The name of the role.</param>
        /// <returns>true if the roleName is a system role, otherwiser false</returns>
        public static bool IsSystemRole(string roleName)
        {
            if (roleName.Equals(AdministratorRole, StringComparison.OrdinalIgnoreCase) ||
                roleName.Equals(AnonymousRole, StringComparison.OrdinalIgnoreCase) ||
                roleName.Equals(EditorsRole, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            return false;
        }

        #endregion

        #region GenericPageSize

        /// <summary>
        /// Default number of items per page in admin data grids.
        /// </summary>
        public static int GenericPageSize
        {
            get
            {
                return string.IsNullOrEmpty(WebConfigurationManager.AppSettings["BlogEngine.GenericPageSize"])
                           ? 20
                           : int.Parse(WebConfigurationManager.AppSettings["BlogEngine.GenericPageSize"]);
            }
        }

        #endregion
    }
}
