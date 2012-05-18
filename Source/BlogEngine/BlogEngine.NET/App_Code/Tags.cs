using System;
using System.Linq;
using BlogEngine.Core;
using BlogEngine.Core.Json;

namespace App_Code
{
    using System.Web.Services;

    /// <summary>
    /// Summary description for Tags
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.Web.Script.Services.ScriptService]
    public class Tags : WebService
    {

        /// <summary>
        /// Edits a tag.
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
            if (!WebUtils.CheckRightsForAdminPostPages(true))
            {
                return new JsonResponse { Success = false, Message = Resources.labels.notAuthorized };
            }
            if (Utils.StringIsNullOrWhitespace(id))
            {
                return new JsonResponse { Message = Resources.labels.idArgumentNull };
            }
            if (vals == null)
            {
                return new JsonResponse { Message = Resources.labels.valsArgumentNull };
            }
            if (vals.Length == 0 || Utils.StringIsNullOrWhitespace(vals[0]))
            {
                return new JsonResponse { Message = Resources.labels.tagIsRequired };
            }

            var response = new JsonResponse();
            try
            {
                foreach (var p in Post.Posts.ToArray())
                {
                    var tg = p.Tags.FirstOrDefault(tag => tag == id);
                    if(tg != null)
                    {
                        p.Tags.Remove(tg);
                        p.Tags.Add(vals[0]);
                        p.DateModified = DateTime.Now;
                        p.Save();
                    }
                }
                response.Success = true;
                response.Message = string.Format(Resources.labels.tagChangedFromTo, id, vals[0]);
            }
            catch (Exception ex)
            {
                Utils.Log(string.Format("Tags.Update: {0}", ex.Message));
                response.Message = string.Format(Resources.labels.couldNotUpdateTag, vals[0]);
            }

            return response;
        }

        /// <summary>
        /// Delete tag in all posts
        /// </summary>
        /// <param name="id">Tag</param>
        /// <returns>Response object</returns>
        [WebMethod]
        public JsonResponse Delete(string id)
        {
            if (!WebUtils.CheckRightsForAdminPostPages(true))
            {
                return new JsonResponse { Success = false, Message = Resources.labels.notAuthorized };
            }
            if (Utils.StringIsNullOrWhitespace(id))
            {
                return new JsonResponse { Message = "Tag is required field" };
            }

            try
            {
                foreach (var p in Post.Posts.ToArray())
                {
                    var tg = p.Tags.FirstOrDefault(tag => tag == id);
                    if (tg != null)
                    {
                        p.Tags.Remove(tg);
                        p.DateModified = DateTime.Now;
                        p.Save();
                    }
                }
                return new JsonResponse { Success = true, Message = string.Format(Resources.labels.tagHasBeenDeleted, id) };
            }
            catch (Exception ex)
            {
                Utils.Log(string.Format("Tags.Delete: {0}", ex.Message));
                return new JsonResponse { Message = string.Format(Resources.labels.couldNotDeleteTag, id) };
            }
        }

    }
}