namespace App_Code
{
    using System;
    using System.Linq;
    using System.Web.Script.Services;
    using System.Web.Security;
    using System.Web.Services;

    using BlogEngine.Core;
    using BlogEngine.Core.Json;

    /// <summary>
    /// The user service.
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [ScriptService]
    public class UserService : WebService
    {
        #region Constants and Fields

        /// <summary>
        /// The response.
        /// </summary>
        private readonly JsonResponse response;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="UserService"/> class.
        /// </summary>
        public UserService()
        {
            this.response = new JsonResponse();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Adds the specified user.
        /// </summary>
        /// <param name="user">The user to add.</param>
        /// <param name="pwd">The password to add.</param>
        /// <param name="email">The email to add.</param>
        /// <param name="roles">Roles for new user</param>
        /// <returns>JSON Response.</returns>
        [WebMethod]
        public JsonResponse Add(string user, string pwd, string email, string[] roles)
        {
            if (!Security.IsAuthorizedTo(Rights.CreateNewUsers))
            {
                return new JsonResponse() { Message = Resources.labels.notAuthorized };
            }
            else if (Utils.StringIsNullOrWhitespace(user))
            {
                return new JsonResponse() { Message = Resources.labels.userArgumentInvalid };
            }
            else if (Utils.StringIsNullOrWhitespace(pwd))
            {
                return new JsonResponse() { Message = Resources.labels.passwordArgumentInvalid };
            }
            else if (Utils.StringIsNullOrWhitespace(email) || !Utils.IsEmailValid(email))
            {
                return new JsonResponse() { Message = Resources.labels.emailArgumentInvalid };
            }

            user = user.Trim();
            email = email.Trim();
            pwd = pwd.Trim();

            if (Membership.GetUser(user) != null)
            {
                return new JsonResponse() { Message = string.Format(Resources.labels.userAlreadyExists, user) };
            }

            try
            {
                Membership.CreateUser(user, pwd, email);

                if (Security.IsAuthorizedTo(Rights.EditOtherUsersRoles))
                {
                    if (roles.GetLength(0) > 0)
                    {
                        Roles.AddUsersToRoles(new string[] { user }, roles);
                    }
                }

                return new JsonResponse() { Success = true, Message = string.Format(Resources.labels.userHasBeenCreated, user) };
            }
            catch (Exception ex)
            {
                Utils.Log("UserService.Add: ", ex);
                return new JsonResponse() { Message = string.Format(Resources.labels.couldNotCreateUser, user, ex.Message) };
            }

        }

        /// <summary>
        /// Deletes the specified id.
        /// </summary>
        /// <param name="id">The username.</param>
        /// <returns>JSON Response</returns>
        [WebMethod]
        public JsonResponse Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                this.response.Success = false;
                this.response.Message = Resources.labels.userNameIsRequired;
                return this.response;
            }

            bool isSelf = id.Equals(Security.CurrentUser.Identity.Name, StringComparison.OrdinalIgnoreCase);

            if (isSelf && !Security.IsAuthorizedTo(Rights.DeleteUserSelf))
            {
                return new JsonResponse() { Message = Resources.labels.notAuthorized };
            }
            else if (!isSelf && !Security.IsAuthorizedTo(Rights.DeleteUsersOtherThanSelf))
            {
                return new JsonResponse() { Message = Resources.labels.notAuthorized };
            }

            // Last check - it should not be possible to remove the last use who has the right to Add and/or Edit other user accounts. If only one of such a 
            // user remains, that user must be the current user, and can not be deleted, as it would lock the user out of the BE environment, left to fix
            // it in XML or SQL files / commands. See issue 11990
            bool adminsExist = false;
            MembershipUserCollection users = Membership.GetAllUsers();
            foreach (MembershipUser user in users)
            {
                string[] roles = Roles.GetRolesForUser(user.UserName);

                // look for admins other than 'id' 
                if (!id.Equals(user.UserName, StringComparison.OrdinalIgnoreCase) && (Right.HasRight(Rights.EditOtherUsers, roles) || Right.HasRight(Rights.CreateNewUsers, roles)))
                {
                    adminsExist = true;
                    break;
                }
            }

            if (!adminsExist)
            {
                return new JsonResponse() { Message = Resources.labels.cannotDeleteLastAdmin };
            }

            string[] userRoles = Roles.GetRolesForUser(id);

            try
            {
                if (userRoles.Length > 0)
                {
                    Roles.RemoveUsersFromRoles(new string[] { id }, userRoles);
                }
                
                Membership.DeleteUser(id);
            }
            catch (Exception ex)
            {
                Utils.Log(string.Format("Users.Delete : {0}", ex.Message));
                this.response.Success = false;
                this.response.Message = string.Format(Resources.labels.couldNotDeleteUser, id);
                return this.response;
            }

            this.response.Success = true;
            this.response.Message = string.Format(Resources.labels.userHasBeenDeleted, id);
            return this.response;
        }

        /// <summary>
        /// Edits the specified id.
        /// </summary>
        /// <param name="id">The user id.</param>
        /// <param name="bg">The background.</param>
        /// <param name="vals">The values.</param>
        /// <returns>JSON Response</returns>
        [WebMethod]
        public JsonResponse Edit(string id, string bg, string[] vals)
        {
            try
            {
                this.response.Success = false;

                bool isSelf = id.Equals(Security.CurrentUser.Identity.Name, StringComparison.OrdinalIgnoreCase);

                if (string.IsNullOrEmpty(vals[0]))
                {
                    this.response.Message = Resources.labels.emailIsRequired;
                    return this.response;
                }

                if (
                    Membership.GetAllUsers().Cast<MembershipUser>().Any(
                        u => u.Email.ToLowerInvariant() == vals[0].ToLowerInvariant()))
                {
                    this.response.Message = Resources.labels.userWithEmailExists;
                    return this.response;
                }

                if (isSelf && !Security.IsAuthorizedTo(Rights.EditOwnUser))
                {
                    this.response.Message = Resources.labels.notAuthorized;
                    return this.response;
                }
                else if (!isSelf && !Security.IsAuthorizedTo(Rights.EditOtherUsers))
                {
                    this.response.Message = Resources.labels.notAuthorized;
                    return this.response;
                }

                var usr = Membership.GetUser(id);
                if (usr != null)
                {
                    usr.Email = vals[0];
                    Membership.UpdateUser(usr);
                }

                this.response.Success = true;
                this.response.Message = string.Format(Resources.labels.userUpdated, id);
                return this.response;
            }
            catch (Exception ex)
            {
                Utils.Log(string.Format("UserService.Update: {0}", ex.Message));
                this.response.Message = string.Format(Resources.labels.couldNotUpdateUser, id);
                return this.response;
            }
        }

        #endregion
    }
}