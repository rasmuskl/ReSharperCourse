namespace App_Code
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Script.Services;
    using System.Web.Services;

    using BlogEngine.Core;
    using BlogEngine.Core.Json;

    /// <summary>
    /// The comments.
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [ScriptService]
    public class Posts : WebService
    {
        [Obsolete]
        [WebMethod]
        public JsonResponse DeletePost(string id)
        {
            if (Utils.StringIsNullOrWhitespace(id))
            {
                return new JsonResponse() { Message = Resources.labels.invalidPostId };
            }

            var post = Post.GetPost(new Guid(id));
            if (post == null)
            {
                return new JsonResponse() { Message = Resources.labels.invalidPostId };
            }

            if (!post.CanUserDelete)
            {
                return new JsonResponse() { Message = Resources.labels.notAuthorized };
            }
            else
            {
                try
                {
                    post.Delete();
                    post.Save();
                    return new JsonResponse() { Success = true, Message = Resources.labels.postDeleted };

                }
                catch (Exception ex)
                {
                    Utils.Log(string.Format("Api.Posts.DeletePost: {0}", ex.Message));
                    return new JsonResponse() { Message = string.Format(Resources.labels.couldNotDeletePost, ex.Message) };
                }

            }
        }

        [WebMethod]
        public JsonResponse DeletePage(string id)
        {
            JsonResponse response = new JsonResponse();
            response.Success = false;

            if (string.IsNullOrEmpty(id))
            {
                response.Message = Resources.labels.pageIdRequired;
                return response;
            }

            var page = Page.GetPage(new Guid(id));
            if (page == null)
            {
                return new JsonResponse() { Message = Resources.labels.invalidPageId };
            }

            if (!page.CanUserDelete)
            {
                return new JsonResponse() { Message = Resources.labels.notAuthorized };
            }

            try
            {
                page.Delete();
                page.Save();
            }
            catch (Exception ex)
            {
                Utils.Log(string.Format("Api.Posts.DeletePage: {0}", ex.Message));
                response.Message = string.Format(Resources.labels.couldNotDeletePage, ex.Message);
                return response;
            }

            response.Success = true;
            response.Message = Resources.labels.pageDeleted;
            return response;
        }


    }

}
