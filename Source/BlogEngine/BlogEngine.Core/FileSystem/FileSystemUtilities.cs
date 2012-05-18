using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using BlogEngine.Core.Providers;
using System.Web.Configuration;
using System.Configuration;

namespace BlogEngine.Core.FileSystem
{
    public class FileSystemUtilities
    {
        /// <summary>
        /// Compresses a directory using Zip compression into a specified directory
        /// </summary>
        /// <param name="ArchiveOutputLocation">the output directory path (including file name)</param>
        /// <param name="ArchiveDirectory">the Directory object to be compressed</param>
        /// <remarks>
        /// is recursive
        /// </remarks>
        public void CompressDirectory(string ArchiveOutputLocation, Directory ArchiveDirectory)
        {
        }

        /// <summary>
        /// Adds a directory to a specific ZipStream
        /// </summary>
        /// <param name="RootDirectory">the root directory</param>
        /// <param name="CurrentDirectory">the directory to push</param>
        /// <param name="zStream">stream to write to</param>
        /// <remarks>
        /// do not call this method directly, this method is designed be called in a recursive manor.
        /// </remarks>
        private void ZipDirectory(Directory RootDirectory, Directory CurrentDirectory, object zStream)
        {
        }

        /// <summary>
        /// Dumps the file store provider to a zip file, clears the old file store, switches and reloads a new providers
        ///     clears the new file storage provider, then loads the new file store provider from the zip.
        /// </summary>
        /// <param name="NewProviderName">The name of the new provider</param>
        /// <param name="ArchiveOutputLocation">The output archive location.</param>
        /// <returns>bool if complete</returns>
        public string DumpProvider(string NewProviderName, string ArchiveOutputLocation)
        {
            string Message = string.Empty;
            CompressDirectory(ArchiveOutputLocation, Blog.CurrentInstance.RootFileStore);
            try
            {
                //file system may throw an error if there is a lock on a file while trying to delete. As this is the 
                //old provider we *will* allow for residue to be left behind. The user may wish to manually clear the provider
                BlogService.ClearFileSystem();
            }
            catch
            {
                Message = "Error while clearing the old file system provider, there may have been a lock on a file. You will have to manually clear the old file system. The update operation has bypassed clearing the old file storage, however will continue to import your data into your new file storage.<br/>";
            }
            var config = WebConfigurationManager.OpenWebConfiguration("~");
            BlogFileSystemProviderSection section = (BlogFileSystemProviderSection)config.GetSection("BlogEngine/blogFileSystemProvider");
            var olderProivder = section.DefaultProvider.Clone().ToString();
            section.DefaultProvider = NewProviderName;
            config.Save();
            ConfigurationManager.RefreshSection("BlogEngine/blogFileSystemProvider");
            config = null;
            config = WebConfigurationManager.OpenWebConfiguration("~");
            try
            {
                BlogService.ReloadFileSystemProvider();
                BlogService.ClearFileSystem();
                ExtractDirectory(ArchiveOutputLocation);
            }
            catch
            {
                section = (BlogFileSystemProviderSection)config.GetSection("BlogEngine/blogFileSystemProvider");
                section.DefaultProvider = olderProivder;
                config.Save();
                ConfigurationManager.RefreshSection("BlogEngine/blogFileSystemProvider");
                config = null;
                config = WebConfigurationManager.OpenWebConfiguration("~");
                BlogService.ReloadFileSystemProvider();
                BlogService.ClearFileSystem();
                ExtractDirectory(ArchiveOutputLocation);
                Message = "Error while copying data to the new file storage provider, your changes were not successful.<br/>";
            }
            if (System.IO.File.Exists(ArchiveOutputLocation))
                System.IO.File.Delete(ArchiveOutputLocation);
            return Message;
        }

        private void ExtractDirectory(string ArchiveOutputLocation)
        {
        }


        public static Providers.BlogProvider CreateProvider(string ProviderName)
        {
            var section = (BlogProviderSection)WebConfigurationManager.GetSection("BlogEngine/blogProvider");
            var _providers = new BlogProviderCollection();
            ProvidersHelper.InstantiateProviders(section.Providers, _providers, typeof(BlogProvider));
            return _providers[ProviderName];
        }

    }        
}
