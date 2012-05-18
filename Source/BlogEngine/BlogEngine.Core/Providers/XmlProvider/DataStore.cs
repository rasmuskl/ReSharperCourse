// --------------------------------------------------------------------------------------------------------------------
// <summary>
//   A storage provider for BlogEngine that uses XML files.
//   <remarks>
//   To build another provider, you can just copy and modify
//   this one. Then add it to the web.config's BlogEngine section.
//   </remarks>
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Linq;

namespace BlogEngine.Core.Providers
{
    using System.IO;
    using System.Web.Hosting;
    using System.Xml.Serialization;

    using BlogEngine.Core.DataStore;

    /// <summary>
    /// A storage provider for BlogEngine that uses XML files.
    ///     <remarks>
    /// To build another provider, you can just copy and modify
    ///         this one. Then add it to the web.config's BlogEngine section.
    ///     </remarks>
    /// </summary>
    public partial class XmlBlogProvider : BlogProvider
    {
        #region Public Methods

        /// <summary>
        /// Loads settings from generic data store
        /// </summary>
        /// <param name="extensionType">
        /// Extension Type
        /// </param>
        /// <param name="extensionId">
        /// Extension ID
        /// </param>
        /// <returns>
        /// Stream Settings
        /// </returns>
        public override object LoadFromDataStore(ExtensionType extensionType, string extensionId)
        {
            var fileName = string.Format("{0}{1}.xml", StorageLocation(extensionType), extensionId);
            Stream str = null;
            if (!Directory.Exists(StorageLocation(extensionType)))
            {
                Directory.CreateDirectory(StorageLocation(extensionType));
            }

            if (File.Exists(fileName))
            {
                var reader = new StreamReader(fileName);
                str = reader.BaseStream;
            }

            return str;
        }

        /// <summary>
        /// Removes settings from data store
        /// </summary>
        /// <param name="extensionType">
        /// Extension Type
        /// </param>
        /// <param name="extensionId">
        /// Extension Id
        /// </param>
        public override void RemoveFromDataStore(ExtensionType extensionType, string extensionId)
        {
            var fileName = string.Format("{0}{1}.xml", StorageLocation(extensionType), extensionId);
            File.Delete(fileName);
        }

        /// <summary>
        /// Save settings to generic data store
        /// </summary>
        /// <param name="extensionType">
        /// Type of extension
        /// </param>
        /// <param name="extensionId">
        /// Extension ID
        /// </param>
        /// <param name="settings">
        /// Stream Settings
        /// </param>
        public override void SaveToDataStore(ExtensionType extensionType, string extensionId, object settings)
        {
            var fileName = string.Format("{0}{1}.xml", StorageLocation(extensionType), extensionId);
            if (!Directory.Exists(StorageLocation(extensionType)))
            {
                Directory.CreateDirectory(StorageLocation(extensionType));
            }

            using (TextWriter writer = new StreamWriter(fileName))
            {
                var x = new XmlSerializer(settings.GetType());
                x.Serialize(writer, settings);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Data Store Location
        /// </summary>
        /// <param name="extensionType">
        /// Type of extension
        /// </param>
        /// <returns>
        /// Path to storage directory
        /// </returns>
        private static string StorageLocation(ExtensionType extensionType)
        {
            switch (extensionType)
            {
                case ExtensionType.Extension:
                    Blog blog = Blog.Blogs.FirstOrDefault(b => b.IsPrimary);
                    return
                        HostingEnvironment.MapPath(
                            Path.Combine(blog.StorageLocation, @"datastore\extensions\"));
                case ExtensionType.Widget:
                    return
                        HostingEnvironment.MapPath(
                            Path.Combine(Blog.CurrentInstance.StorageLocation, @"datastore\widgets\"));
                case ExtensionType.Theme:
                    return
                        HostingEnvironment.MapPath(
                            Path.Combine(Blog.CurrentInstance.StorageLocation, @"datastore\themes\"));
            }

            return string.Empty;
        }

        #endregion
    }
}