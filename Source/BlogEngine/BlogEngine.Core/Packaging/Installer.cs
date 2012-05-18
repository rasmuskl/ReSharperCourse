using System;
using System.Linq;
using System.Web;
using System.Collections.Generic;
using BlogEngine.Core.Json;
using BlogEngine.Core.Providers;
using NuGet;

namespace BlogEngine.Core.Packaging
{
    /// <summary>
    /// Responsible for install/uninstall operations
    /// </summary>
    public static class Installer
    {
        private static IPackageRepository _repository
        {
            get
            {
                return PackageRepositoryFactory.Default.CreateRepository(
                    BlogSettings.Instance.GalleryFeedUrl);
            }
        }

        /// <summary>
        /// Install package
        /// </summary>
        /// <param name="pkgId"></param>
        public static JsonResponse InstallPackage(string pkgId)
        {
            try
            {
                if(BlogService.InstalledFromGalleryPackages() != null && 
                    BlogService.InstalledFromGalleryPackages().Find(p => p.PackageId == pkgId) != null)
                    UninstallPackage(pkgId);

                var packageManager = new PackageManager(
                    _repository,
                    new DefaultPackagePathResolver(BlogSettings.Instance.GalleryFeedUrl),
                    new PhysicalFileSystem(HttpContext.Current.Server.MapPath(Utils.ApplicationRelativeWebRoot + "App_Data/packages"))
                );

                var package = _repository.FindPackage(pkgId);

                var iPkg = new InstalledPackage { PackageId = package.Id, Version = package.Version.ToString() };
                BlogService.InsertPackage(iPkg);

                var packageFiles = FileSystem.InstallPackage(package.Id, package.Version.ToString());
                BlogService.InsertPackageFiles(packageFiles);

                Blog.CurrentInstance.Cache.Remove(Constants.CacheKey);

                Utils.Log(string.Format("Installed package {0} by {1}", pkgId, Security.CurrentUser.Identity.Name));
            }
            catch (Exception ex)
            {
                Utils.Log("BlogEngine.Core.Packaging.Installer.InstallPackage(" + pkgId + ")", ex);
                try
                {
                    UninstallPackage(pkgId);
                }
                catch (Exception)
                {
                    // just trying to clean up if package did not installed properly
                }
                return new JsonResponse { Success = false, Message = "Error installing package, see logs for details" };
            }

            return new JsonResponse { Success = true, Message = "Package successfully installed" };
        }

        /// <summary>
        /// Uninstall package
        /// </summary>
        /// <param name="pkgId"></param>
        /// <returns></returns>
        public static JsonResponse UninstallPackage(string pkgId)
        {
            try
            {
                var packageManager = new PackageManager(
                    _repository,
                    new DefaultPackagePathResolver(BlogSettings.Instance.GalleryFeedUrl),
                    new PhysicalFileSystem(HttpContext.Current.Server.MapPath(Utils.ApplicationRelativeWebRoot + "App_Data/packages"))
                );

                var package = _repository.FindPackage(pkgId);

                if (package == null)
                    return new JsonResponse { Success = false, Message = "Package " + pkgId + " not found" };

                packageManager.UninstallPackage(package, true);

                FileSystem.UninstallPackage(package.Id);

                BlogService.DeletePackage(pkgId);

                // reset cache
                Blog.CurrentInstance.Cache.Remove(Constants.CacheKey);

                Utils.Log(string.Format("Uninstalled package {0} by {1}", pkgId, Security.CurrentUser.Identity.Name));
            }
            catch (Exception ex)
            {
                Utils.Log("BlogEngine.Core.Packaging.Installer.UninstallPackage(" + pkgId + ")", ex);
                return new JsonResponse { Success = false, Message = "Error uninstalling package, see logs for details" };
            }

            return new JsonResponse { Success = true, Message = "Package successfully uninstalled" };
        }

        /// <summary>
        /// Load installed packages
        /// </summary>
        /// <param name="packages"></param>
        public static void MarkAsInstalled(List<JsonPackage> packages)
        {
            var installed = BlogService.InstalledFromGalleryPackages();

            foreach (var pkg in packages)
            {
                if (pkg == null) continue;
                var p = pkg;
                if(installed != null && installed.Count > 0)
                {
                    foreach (var inst in installed.Where(inst => p.Id.ToLower() == inst.PackageId.ToLower()))
                    {
                        pkg.Location = "I";
                        pkg.LocalVersion = inst.Version;
                    }
                }
            }
        }
    }
}
