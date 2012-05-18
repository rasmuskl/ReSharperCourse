namespace App_Code
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Script.Services;
    using System.Web.Security;
    using System.Web.Services;

    using BlogEngine.Core;
    using BlogEngine.Core.Json;

    /// <summary>
    /// Membership service to support AJAX calls
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [ScriptService]
    public sealed class RoleService : WebService
    {
        #region Constants and Fields

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="RoleService"/> class.
        /// </summary>
        public RoleService()
        {
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Adds a role.
        /// </summary>
        /// <param name="roleName">
        /// The role name.
        /// </param>
        /// <returns>
        /// JSON Response
        /// </returns>
        [WebMethod]
        public JsonResponse Add(string roleName)
        {
            if (!Security.IsAuthorizedTo(Rights.CreateNewRoles))
            {
                return GetNotAuthorized();
            }
            else if (Utils.StringIsNullOrWhitespace(roleName))
            {
                return new JsonResponse() { Message = Resources.labels.roleNameIsRequired };
            }
            else if (Roles.RoleExists(roleName))
            {
                return new JsonResponse() { Message = string.Format(Resources.labels.roleAlreadyExists, roleName) };
            }
            else
            {
                var response = new JsonResponse();

                try
                {
                    Roles.CreateRole(roleName);
                    response.Success = true;
                    response.Message = string.Format(Resources.labels.roleHasBeenCreated, roleName);

                }
                catch (Exception ex)
                {
                    Utils.Log(string.Format("Roles.AddRole: {0}", ex.Message));
                    response.Success = false;
                    response.Message = string.Format(Resources.labels.couldNotCreateRole, roleName);
                }

                return response;
            }
        }

        /// <summary>
        /// The delete.
        /// </summary>
        /// <param name="id">
        /// The role id.
        /// </param>
        /// <returns>
        /// JSON Response.
        /// </returns>
        [WebMethod]
        public JsonResponse Delete(string id)
        {
            if (!Security.IsAuthorizedTo(Rights.DeleteRoles))
            {
                return GetNotAuthorized();
            }
            else if (Utils.StringIsNullOrWhitespace(id))
            {
                return new JsonResponse() { Message = Resources.labels.roleNameIsRequired };
            }

            try
            {
                Right.OnRoleDeleting(id);
                Roles.DeleteRole(id);
                return new JsonResponse() { Success = true, Message = string.Format(Resources.labels.roleHasBeenDeleted, id) };
            }
            catch (Exception ex)
            {
                Utils.Log(string.Format("Roles.DeleteRole: {0}", ex.Message));
                return new JsonResponse() { Message = string.Format(Resources.labels.couldNotDeleteRole, id) };
            }
        }

        /// <summary>
        /// Saves the rights for a specific Role. 
        /// </summary>
        /// <param name="roleName">The name of the role whose rights are being updated.</param>
        /// <param name="rightsCollection">A dictionary of rights that a role is allowed to have.</param>
        /// <returns></returns>
        [WebMethod]
        public JsonResponse SaveRights(string roleName, Dictionary<string, bool> rightsCollection)
        {
            if (!Security.IsAuthorizedTo(Rights.EditRoles))
            {
                return new JsonResponse() { Message = Resources.labels.notAuthorized };
            }
            else if (Utils.StringIsNullOrWhitespace(roleName) || !Roles.RoleExists(roleName))
            {
                return new JsonResponse() { Message = Resources.labels.invalidRoleName };
            }
            else if (rightsCollection == null)
            {
                return new JsonResponse() { Message = Resources.labels.rightsCanNotBeNull };
            }
            else
            {

                // The rights collection can be empty, just not null. An empty array would indicate that a role is
                // being updated to include no rights at all.
                // Remove spaces from each key (i.e. so "Edit Own User" becomes EditOwnUser).
                rightsCollection = rightsCollection.ToDictionary(r => r.Key.Replace(" ", string.Empty), r => r.Value, StringComparer.OrdinalIgnoreCase);

                // Validate the dictionary before doing any altering to Rights.
                foreach (var right in rightsCollection)
                {
                    if (!Right.RightExists(right.Key))
                    {
                        return new JsonResponse() { Success = false, Message = String.Format(Resources.labels.noRightExists, right.Key) };
                    }
                    else if (right.Value == false)
                    {
                        return new JsonResponse() { Success = false, Message = Resources.labels.doNotPassRightsWithFalseValue };
                    }
                }

                foreach (var right in Right.GetAllRights())
                {
                    if (right.Flag != Rights.None)
                    {
                        if (rightsCollection.ContainsKey(right.Name))
                        {
                            right.AddRole(roleName);
                        }
                        else
                        {
                            right.RemoveRole(roleName);
                        }
                    }
                }

                BlogEngine.Core.Providers.BlogService.SaveRights();
                return new JsonResponse() { Success = true, Message = string.Format(Resources.labels.rightsUpdatedForRole, roleName) };
            }

        }

        /// <summary>
        /// Edits a role.
        /// </summary>
        /// <param name="id">
        /// The row id.
        /// </param>
        /// <param name="bg">
        /// The background.
        /// </param>
        /// <param name="vals">
        /// The values.
        /// </param>
        /// <returns>
        /// JSON Response.
        /// </returns>
        [WebMethod]
        public JsonResponse Edit(string id, string bg, string[] vals)
        {
            if (!Security.IsAuthorizedTo(Rights.EditRoles))
            {
                return GetNotAuthorized();
            }
            else if (Utils.StringIsNullOrWhitespace(id))
            {
                return new JsonResponse() { Message = Resources.labels.idArgumentNull };
            }
            else if (vals == null)
            {
                return new JsonResponse() { Message = Resources.labels.valsArgumentNull };
            }
            else if (vals.Length == 0 || Utils.StringIsNullOrWhitespace(vals[0]))
            {
                return new JsonResponse() { Message = Resources.labels.roleNameIsRequired };
            }

            var response = new JsonResponse();

            try
            {
                Right.OnRenamingRole(id, vals[0]);

                string[] usersInRole = Roles.GetUsersInRole(id);
                if (usersInRole.Length > 0)
                {
                    Roles.RemoveUsersFromRoles(usersInRole, new string[] { id });
                }

                Roles.DeleteRole(id);
                Roles.CreateRole(vals[0]);

                if (usersInRole.Length > 0)
                {
                    Roles.AddUsersToRoles(usersInRole, new string[] { vals[0] });
                }

                Right.RefreshAllRights();
               
                response.Success = true;
                response.Message = string.Format(Resources.labels.roleUpdatedFromTo, id, vals[0]);
            }
            catch (Exception ex)
            {
                Utils.Log(string.Format("Roles.UpdateRole: {0}", ex.Message));
                response.Message = string.Format(Resources.labels.couldNotUpdateRole, vals[0]);
            }

            return response;
        }

        /// <summary>
        /// Returns the default rights for the role.
        /// </summary>
        /// <param name="roleName">The roleName.</param>
        /// <returns>
        /// JSON Response containing delimited default rights.
        /// </returns>
        [WebMethod]
        public JsonResponse GetDefaultRoleRights(string roleName)
        {
            if (!Security.IsAuthorizedTo(Rights.EditRoles))
            {
                return GetNotAuthorized();
            }
            else if (Utils.StringIsNullOrWhitespace(roleName))
            {
                return new JsonResponse() { Message = Resources.labels.roleNameArgumentNull };
            }

            List<Rights> defaultRights = Right.GetDefaultRights(roleName);

            var response = new JsonResponse()
            {
                Success = true,
                Data = string.Join("|", defaultRights.Select(r => Utils.FormatIdentifierForDisplay(r.ToString())).ToArray())
            };

            return response;
        }

        /// <summary>
        /// Returns the rights for the role.
        /// </summary>
        /// <param name="roleName">The roleName.</param>
        /// <returns>
        /// JSON Response containing delimited rights.
        /// </returns>
        [WebMethod]
        public JsonResponse GetRoleRights(string roleName)
        {
            if (!Security.IsAuthorizedTo(Rights.EditRoles))
            {
                return GetNotAuthorized();
            }
            else if (Utils.StringIsNullOrWhitespace(roleName))
            {
                return new JsonResponse() { Message = Resources.labels.roleNameArgumentNull };
            }

            IEnumerable<Right> roleRights = Right.GetRights(roleName);

            var response = new JsonResponse()
            {
                Success = true,
                Data = string.Join("|", roleRights.Select(r => r.DisplayName).ToArray())
            };

            return response;
        }

        #endregion

        #region Methods

        private static JsonResponse GetNotAuthorized()
        {
            return new JsonResponse() { Success = false, Message = Resources.labels.notAuthorized };
        }

        #endregion

    }
}