using System;
using System.Collections.Generic;
using System.Linq;
using BlogEngine.Core.GalleryServer;
using BlogEngine.Core.Json;

namespace BlogEngine.Core.Packaging
{
    /// <summary>
    /// Online gallery
    /// </summary>
    public static class Gallery
    {
        /// <summary>
        /// Type of sort order
        /// </summary>
        public enum OrderType
        {
            /// <summary>
            /// Most downloaded
            /// </summary>
            Downloads,
            /// <summary>
            /// Newest
            /// </summary>
            Newest,
            /// <summary>
            /// Heighest rated
            /// </summary>
            Rating,
            /// <summary>
            /// Alphabetical
            /// </summary>
            Alphanumeric
        }

        /// <summary>
        /// Gallery pager
        /// </summary>
        public static Pager GalleryPager;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="packages"></param>
        public static void Load(List<JsonPackage> packages)
        {
            try
            {
                foreach (var pkg in GetAllPublishedPackages().ToList())
                {
                    //System.Diagnostics.Debug.WriteLine(string.Format("{0}|{1}|{2}|{3}", pkg.Id, pkg.Version, pkg.IsLatestVersion, pkg.IconUrl));

                    var jp = new JsonPackage
                    {
                        Id = pkg.Id,
                        PackageType = pkg.PackageType,
                        Authors = string.IsNullOrEmpty(pkg.Authors) ? "unknown" : pkg.Authors,
                        Description = pkg.Description.Length > 140 ? string.Format("{0}...", pkg.Description.Substring(0, 140)) : pkg.Description,
                        DownloadCount = pkg.DownloadCount,
                        LastUpdated = pkg.LastUpdated.ToString("dd MMM yyyy", Utils.GetDefaultCulture()),
                        Title = pkg.Title,
                        OnlineVersion = pkg.Version,
                        Website = pkg.ProjectUrl,
                        Tags = pkg.Tags,
                        IconUrl = pkg.IconUrl,
                        Location = "G"
                    };

                    // for themes or widgets, get screenshot instead of icon
                    // also get screenshot if icon is missing for package
                    if(pkg.Screenshots != null && pkg.Screenshots.Count > 0)
                    {
                        if ((pkg.PackageType == Constants.Theme || pkg.PackageType == Constants.Widget) || string.IsNullOrEmpty(pkg.IconUrl))
                        {
                            jp.IconUrl = pkg.Screenshots[0].ScreenshotUri;
                        }
                    }

                    // if both icon and screenshot missing, get default image for package type
                    if (string.IsNullOrEmpty(jp.IconUrl))
                        jp.IconUrl = DefaultThumbnail(pkg.PackageType);

                    if (!string.IsNullOrEmpty(jp.IconUrl) && !jp.IconUrl.StartsWith("http:"))
                        jp.IconUrl = Constants.GalleryUrl + jp.IconUrl;

                    if (!string.IsNullOrWhiteSpace(pkg.GalleryDetailsUrl))
                        jp.PackageUrl = PackageUrl(pkg.PackageType, pkg.Id);

                    //System.Diagnostics.Debug.WriteLine(string.Format("{0}|{1}|{2}|{3}", jp.Id, jp.OnlineVersion, jp.PackageType, jp.IconUrl));

                    packages.Add(jp);
                }
            }
            catch (Exception ex)
            {
                Utils.Log("BlogEngine.Core.Packaging.Load", ex);
            }   
        }

        /// <summary>
        /// Convert version from string to int for comparison
        /// </summary>
        /// <param name="version">string version</param>
        /// <returns>int version</returns>
        public static int ConvertVersion(string version)
        {
            if (string.IsNullOrEmpty(version))
                return 0;

            int numVersion;
            Int32.TryParse(version.Replace(".", ""), out numVersion);
            return numVersion;
        }

        /// <summary>
        /// Package URL
        /// </summary>
        /// <param name="pkgType">Package Type</param>
        /// <param name="pkgId">Package ID</param>
        /// <returns></returns>
        public static string PackageUrl(string pkgType, string pkgId)
        {
            switch (pkgType)
            {
                case "Theme":
                    return string.Format("{0}/List/Themes/{1}", Constants.GalleryAppUrl, pkgId);
                case "Extension":
                    return string.Format("{0}/List/Extensions/{1}", Constants.GalleryAppUrl, pkgId);
                case "Widget":
                    return string.Format("{0}/List/Widgets/{1}", Constants.GalleryAppUrl, pkgId);
            }
            return string.Empty;
        }

        static IEnumerable<PublishedPackage> GetAllPublishedPackages()
        {
            var packagingSource = new PackagingSource { FeedUrl = BlogSettings.Instance.GalleryFeedUrl };
            var allPacks = new List<PublishedPackage>();

            // gallery has a limit 100 records per call
            // keep calling till any records returned
            int cnt;
            var skip = 0;
            do
            {
                var s = skip;
                var pkgs = (new[] { packagingSource })
                .SelectMany(
                    source =>
                    {
                        var galleryFeedContext = new GalleryFeedContext(new Uri(BlogSettings.Instance.GalleryFeedUrl)) { IgnoreMissingProperties = true };
                        return galleryFeedContext.Packages.Expand("Screenshots").OrderBy(p => p.Id).Where(p => p.IsLatestVersion).Skip(s).Take(100);
                    }
                );
                cnt = pkgs.Count();
                skip = skip + 100;
                allPacks.AddRange(pkgs);
            } while (cnt > 0);

            return allPacks;
        }

        static string DefaultThumbnail(string packageType)
        {
            switch (packageType)
            {
                case "Theme":
                    return string.Format("{0}/Themes/OrchardGallery/Content/Images/themeDefaultIcon.png", Constants.GalleryAppUrl);
                case "Extension":
                    return string.Format("{0}/Themes/OrchardGallery/Content/Images/extensionDefaultIcon.png", Constants.GalleryAppUrl);
                case "Widget":
                    return string.Format("{0}/Themes/OrchardGallery/Content/Images/widgetDefaultIcon.png", Constants.GalleryAppUrl);
            }
            return string.Empty;
        }
   
    }
}
