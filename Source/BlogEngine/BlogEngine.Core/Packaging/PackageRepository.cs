using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Caching;
using BlogEngine.Core.Json;
using System.Globalization;

namespace BlogEngine.Core.Packaging
{
    /// <summary>
    /// Package repository
    /// </summary>
    public class PackageRepository
    {
        /// <summary>
        /// Returns list of packages from online gallery for a single page
        /// </summary>
        /// <param name="pkgType">Theme, Widget or Extension</param>
        /// <param name="page">Current page</param>
        /// <param name="sortOrder">Order - newest, most downloaded etc.</param>
        /// <param name="searchVal">Search term if it is search request</param>
        /// <returns>List of packages</returns>
        public static List<JsonPackage> FromGallery(string pkgType, int page = 0, Gallery.OrderType sortOrder = Gallery.OrderType.Newest, string searchVal = "")
        {
            var packages = new List<JsonPackage>();
            var gallery = CachedPackages.Where(p => p.Location == "G" || p.Location == "I");

            if (pkgType != "all")
                gallery = gallery.Where(p => p.PackageType == pkgType).ToList();

            foreach (var pkg in gallery)
            {
                if (string.IsNullOrEmpty(searchVal))
                {
                    packages.Add(pkg);
                }
                else
                {
                    if (pkg.Title.IndexOf(searchVal, StringComparison.OrdinalIgnoreCase) != -1
                        || pkg.Description.IndexOf(searchVal, StringComparison.OrdinalIgnoreCase) != -1
                        ||
                        (!string.IsNullOrWhiteSpace(pkg.Tags) &&
                         pkg.Tags.IndexOf(searchVal, StringComparison.OrdinalIgnoreCase) != -1))
                    {
                        packages.Add(pkg);
                    }
                }
            }

            Gallery.GalleryPager = new Pager(page, packages.Count, pkgType);
            CultureInfo culture;
            try
            {
                culture = new CultureInfo(BlogSettings.Instance.Culture);
            }
            catch (Exception)
            {
                culture = Utils.GetDefaultCulture();
            }

            if (packages.Count > 0)
            {
                switch (sortOrder)
                {
                    case Gallery.OrderType.Downloads:
                        packages = packages.OrderByDescending(p => p.DownloadCount).ThenBy(p => p.Title).ToList();
                        break;
                    case Gallery.OrderType.Rating:
                        packages = packages.OrderByDescending(p => p.Rating).ThenBy(p => p.Title).ToList();
                        break;
                    case Gallery.OrderType.Newest:
                        packages = packages.OrderByDescending(p => Convert.ToDateTime(p.LastUpdated, CultureInfo.InvariantCulture)).ThenBy(p => p.Title).ToList();
                        break;
                    case Gallery.OrderType.Alphanumeric:
                        packages = packages.OrderBy(p => p.Title).ToList();
                        break;
                }
            }
            return packages.Skip(page * Constants.PageSize).Take(Constants.PageSize).ToList();
        }

        /// <summary>
        /// Returns list of local packages from repository
        /// For reference:
        /// G - package exists only in the online gallery
        /// I - installed from gallery, exists both in gallery and locally
        /// L - local, only exists locally
        /// </summary>
        /// <param name="pkgType">Package type</param>
        /// <returns>List of packages</returns>
        public static List<JsonPackage> LocalPackages(string pkgType)
        {
            var packages = new List<JsonPackage>();

            foreach (var pkg in CachedPackages)
            {
                if(pkgType != "all")
                {
                    if (pkg.PackageType != pkgType) continue;
                }

                if(pkg.Location == "G")
                    continue;

                packages.Add(pkg);
            }
            return packages;
        }

        /// <summary>
        /// Package by ID
        /// </summary>
        /// <param name="pkgId">Package ID</param>
        /// <returns>Package</returns>
        public static JsonPackage GetPackage(string pkgId)
        {
            return CachedPackages.FirstOrDefault(pkg => pkg.Id == pkgId);
        }

        /// <summary>
        /// Local "packages" - list of extensions,
        /// themes and widgets folders
        /// </summary>
        static IEnumerable<JsonPackage> CachedPackages
        {
            get
            {
                if (Blog.CurrentInstance.Cache[Constants.CacheKey] == null)
                {
                    Blog.CurrentInstance.Cache.Add(
                        Constants.CacheKey,
                        LoadPackages(),
                        null,
                        Cache.NoAbsoluteExpiration,
                        new TimeSpan(0, 15, 0),
                        CacheItemPriority.Low,
                        null);
                }
                return (IEnumerable<JsonPackage>)Blog.CurrentInstance.Cache[Constants.CacheKey];

                //return (IEnumerable<JsonPackage>)LoadPackages();
            }
        }

        static List<JsonPackage> LoadPackages()
        {
            var packages = new List<JsonPackage>();

            Gallery.Load(packages);
            //Trace("01: ", packages);
            FileSystem.Load(packages);
            //Trace("02: ", packages);
            Installer.MarkAsInstalled(packages);
            //Trace("03: ", packages);

            return packages;
        }

        static void Trace(string msg, List<JsonPackage> packages)
        {
            string s = "{0}|{1}|{2}|{3}|{4}|{5}";
            foreach (var p in packages)
            {
                System.Diagnostics.Debug.WriteLine(string.Format(s, msg, p.PackageType, p.Id, p.Location, p.LocalVersion, p.OnlineVersion));
            }
        }
    }
}
