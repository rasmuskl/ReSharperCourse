namespace App_Code
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.Services;
    using System.Web.Script.Services;

    using BlogEngine.Core;
    using BlogEngine.Core.Json;

    /// <summary>
    /// Blogs service.
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [ScriptService]
    public class Blogs : WebService
    {
        /// <summary>
        /// Adds a new blog
        /// </summary>
        /// <returns>JSON Response.</returns>
        [WebMethod]
        public JsonResponse AddBlog()
        {
            Security.DemandUserHasRight(Rights.AccessAdminPages, true);
            if (!Blog.CurrentInstance.IsPrimary)
            {
                return new JsonResponse() { Message = Resources.labels.notAuthorized };
            }



            return new JsonResponse() { Success = true, Message = string.Format(Resources.labels.blogAdded, ".....") };
        }

        [WebMethod]
        public JsonResponse DeleteBlog(string id, bool deleteStorageContainer)
        {
            Security.DemandUserHasRight(Rights.AccessAdminPages, true);
            if (!Blog.CurrentInstance.IsPrimary)
            {
                return new JsonResponse() { Message = Resources.labels.notAuthorized };
            }

            if (Utils.StringIsNullOrWhitespace(id))
            {
                return new JsonResponse() { Message = Resources.labels.invalidBlogId };
            }

            var blog = Blog.GetBlog(new Guid(id));
            if (blog == null)
            {
                return new JsonResponse() { Message = Resources.labels.invalidBlogId };
            }

            if (blog.IsPrimary)
            {
                return new JsonResponse() { Message = "The primary blog cannot be deleted." };
            }

            if (!blog.CanUserDelete)
            {
                return new JsonResponse() { Message = Resources.labels.notAuthorized };
            }
            else
            {
                try
                {   
                    blog.Delete();
                    blog.DeleteStorageContainer = deleteStorageContainer;
                    blog.Save();
                    return new JsonResponse() { Success = true, Message = string.Format(Resources.labels.blogDeleted, blog.Name) };

                }
                catch (Exception ex)
                {
                    Utils.Log(string.Format("Api.Blogs.DeleteBlog: {0}", ex.Message));
                    return new JsonResponse() { Message = string.Format(Resources.labels.couldNotDeleteBlog, ex.Message) };
                }
            }
        }

        [WebMethod]
        public JsonResponse SaveBlog(
            string blogId,
            string copyFromExistingBlogId,
            string blogName,
            string hostname,
            bool isAnyTextBeforeHostnameAccepted,
            string storageContainerName,
            string virtualPath,
            bool isActive,
            bool isSiteAggregation)
        {
            Security.DemandUserHasRight(Rights.AccessAdminPages, true);
            if (!Blog.CurrentInstance.IsPrimary)
            {
                return new JsonResponse() { Message = Resources.labels.notAuthorized };
            }

            Guid existingId = Guid.Empty;
            Blog blog = null;
            if (!string.IsNullOrWhiteSpace(blogId) && blogId.Length == 36)
            {
                existingId = new Guid(blogId);
                blog = Blog.GetBlog(existingId);

                if (blog == null)
                {
                    return new JsonResponse() { Message = Resources.labels.invalidBlogId };
                }
            }

            string message;
            if (!Blog.ValidateProperties(blog == null, blog, blogName, hostname, isAnyTextBeforeHostnameAccepted, storageContainerName, virtualPath, isSiteAggregation, out message))
            {
                if (string.IsNullOrWhiteSpace(message)) { message = "Validation for new blog failed."; }
                return new JsonResponse() { Message = message };
            }

            if (blog == null)
            {
                // new blog

                blog = Blog.CreateNewBlog(copyFromExistingBlogId, blogName, hostname, isAnyTextBeforeHostnameAccepted, storageContainerName, virtualPath, isActive, isSiteAggregation, out message);

                if (blog == null || !string.IsNullOrWhiteSpace(message))
                {
                    return new JsonResponse() { Message = (message ?? "Failed to create the new blog.") };
                }

                return new JsonResponse() { Success = true, Message = "New blog was created." };
            }
            else
            { 
                // update to an existing blog.

                blog.Name = blogName;

                blog.VirtualPath = virtualPath;
                blog.IsActive = isActive;
                blog.Hostname = hostname;
                blog.IsAnyTextBeforeHostnameAccepted = isAnyTextBeforeHostnameAccepted;
                blog.IsSiteAggregation = isSiteAggregation;

                // intentionally not updating StorageContainerName for an update.  this would
                // involve renaming the folder on disk, or DB changes (DB changes are not
                // likely).  if requested, this can be done, but should probably be done as
                // a separate "Rename" process/action.
                

                blog.Save();
                
                return new JsonResponse() { Success = true, Message = "Blog was updated." };
            }
        }
    }
}