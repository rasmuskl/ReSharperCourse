using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web.Script.Serialization;

namespace BlogEngine.Core.Json
{

    /// <summary>
    /// Represents the i18n culture used by blog.js. Used by ResourceHandler.
    /// </summary>
    public sealed class JsonCulture
    {
        #region "Fields"

        private readonly CultureInfo cultureInfo;
        private readonly Dictionary<string, string> translationDict = new Dictionary<string, string>();

        #endregion

        /// <summary>
        /// Creates a new JsonCulture instance from the supplied CultureInfo.
        /// </summary>
        /// <param name="cultureInfo">The CultureInfo needed to get the proper translations for this JsonCulture instance</param>
        /// <param name="resourceType">Type of resource (blog or admin)</param>
        /// <remarks>
        /// 
        /// This class uses a dictionary as its basis for storing/caching its information. This makes it incredibly easy to extend
        /// without having to create/remove properties.
        /// 
        /// </remarks>
        public JsonCulture(CultureInfo cultureInfo, ResourceType resourceType)
        {
            if (cultureInfo == null)
            {
                throw new ArgumentNullException("cultureInfo");
            }
            this.cultureInfo = cultureInfo;

            if(resourceType == ResourceType.Admin)
            {
                AddResource("name");
                AddResource("email");
                AddResource("edit");
                AddResource("delete");
                AddResource("profile");
                AddResource("tools");
                AddResource("youHaveNotAddedAnyUserSoFar");
                AddResource("roles");
                AddResource("rights");
                AddResource("youHaveNotDefinedAnyRolesSoFar");
                AddResource("active");
                AddResource("primary");
                AddResource("textBeforeHostName");
                AddResource("virtualPath");
                AddResource("storageContainerName");
                AddResource("noBlogsWereFound");
                AddResource("approve");
                AddResource("unapprove");
                AddResource("title");
                AddResource("type");
                AddResource("createdOn");
                AddResource("restore");
                AddResource("purge");
                AddResource("restoreSelected");
                AddResource("purgeSelected");
                AddResource("purgeAll");
                AddResource("trashIsEmpty");
                AddResource("tags");
                AddResource("posts");
                AddResource("count");
                AddResource("view");
                AddResource("youHaveNoTaggedPosts");
                AddResource("youHaveNotWrittenAnyPosts");
                AddResource("author");
                AddResource("categories");
                AddResource("published");
                AddResource("parent");
                AddResource("showInList");
                AddResource("youHaveNoWrittenAnyPages");
                AddResource("website");
                AddResource("comment");
                AddResource("save");
                AddResource("or");
                AddResource("cancel");
                AddResource("size");
                AddResource("noFilesFound");
                AddResource("reset");
                AddResource("enabled");
                AddResource("cheked");
                AddResource("spam");
                AddResource("mistakes");
                AddResource("accuracy");
                AddResource("noPackagesToDisplay");
                AddResource("version");
                AddResource("installed");
                AddResource("install");
                AddResource("update");
                AddResource("downloads");
                AddResource("firstName");
                AddResource("lastName");
                AddResource("middleName");
                AddResource("displayName");
                AddResource("birthday");
                AddResource("photoURL");
                AddResource("isPrivate");
                AddResource("personalDetails");
                AddResource("contactDetails");
                AddResource("shortBiography");
                AddResource("phoneFax");
                AddResource("phoneMain");
                AddResource("phoneMobile");
                AddResource("cityTown");
                AddResource("country");
                AddResource("regionState");
                AddResource("ip");
                AddResource("deleteSelected");
                AddResource("approveSelected");
                AddResource("rejectSelected");
                AddResource("deleteAll");
                AddResource("noSpamHere");
                AddResource("noPendingComments");
                AddResource("noApprovedComments");
                AddResource("noPingbacksTrackbacks");
                AddResource("isForSiteAggregation");
            }
            else
            {
                AddResource("hasRated");
                AddResource("savingTheComment");
                AddResource("comments");
                AddResource("commentWasSaved");
                AddResource("commentWaitingModeration");
                AddResource("cancel");
                AddResource("filter");
                AddResource("apmlDescription");
                AddResource("beTheFirstToRate");
                AddResource("currentlyRated");
                AddResource("ratingHasBeenRegistered");
                AddResource("rateThisXStars");
            }
        }

        /// <summary>
        /// Type of language resources
        /// </summary>
        public enum ResourceType
        {
            /// <summary>
            ///     Resources added to Blog
            /// </summary>
            Blog = 0,

            /// <summary>
            ///     Resources added to Admin
            /// </summary>
            Admin = 1
        }

        #region "Methods"

        /// <summary>
        /// Adds a new translatable string resource to this JsonCulture.
        /// </summary>
        /// <param name="scriptKey">The key used to retrieve this value from clientside script.</param>
        /// <param name="resourceLabelKey">The key used to retrieve the translated value from global resource labels.</param>
        /// <returns>The translated string.</returns>
        public string AddResource(string resourceLabelKey)
        {
            var translation = Utils.Translate(resourceLabelKey, null, cultureInfo);
            translationDict.Add(resourceLabelKey, translation);
            return translation;
        }
       
        /// <summary>
        /// Returns a JSON formatted string repressentation of this JsonCulture instance's culture labels.
        /// </summary>
        /// <returns></returns>
        public string ToJsonString()
        {
            return new JavaScriptSerializer().Serialize(this.translationDict);
        }
    
        #endregion

    }
}
