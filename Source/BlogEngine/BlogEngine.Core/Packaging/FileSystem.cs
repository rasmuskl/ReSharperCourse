using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Web;
using System.Text.RegularExpressions;
using BlogEngine.Core.Json;
using BlogEngine.Core.Providers;

namespace BlogEngine.Core.Packaging
{
    /// <summary>
    /// Class for packaging IO
    /// </summary>
    public class FileSystem
    {
        private static int fileOrder;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="packages"></param>
        public static void Load(List<JsonPackage> packages)
        {
            try
            {
                var themes = GetThemes();
                var widgets = GetWidgets();
                
                if(themes != null && themes.Count > 0)
                {
                    foreach (var theme in from theme in themes
                        let found = packages.Any(pkg => theme.Id.ToLower() == pkg.Id.ToLower())
                        where !found select theme)
                    {
                        packages.Add(theme);
                    }
                }

                if (widgets != null && widgets.Count > 0)
                {
                    foreach (var wdg in from wdg in widgets
                        let found = packages.Any(pkg => wdg.Id.ToLower() == pkg.Id.ToLower())
                        where !found select wdg)
                    {
                        packages.Add(wdg);
                    }
                }
            }
            catch (Exception ex)
            {
                Utils.Log(System.Reflection.MethodBase.GetCurrentMethod().Name, ex);
            }
        }

        /// <summary>
        /// Copy uncompressed package files
        /// to application directories
        /// </summary>
        /// <param name="pkgId">Package Id</param>
        /// <param name="version">Package Version</param>
        public static List<PackageFile> InstallPackage(string pkgId, string version)
        {
            var packgeFiles = new List<PackageFile>();

            var content = HttpContext.Current.Server.MapPath(Utils.ApplicationRelativeWebRoot +
                string.Format("App_Data/packages/{0}.{1}/content", pkgId, version));

            var lib = HttpContext.Current.Server.MapPath(Utils.ApplicationRelativeWebRoot +
                string.Format("App_Data/packages/{0}.{1}/lib", pkgId, version));

            var root = HttpContext.Current.Server.MapPath(Utils.ApplicationRelativeWebRoot);
            var bin = HttpContext.Current.Server.MapPath(Utils.ApplicationRelativeWebRoot + "bin");

            // copy content files
            var source = new DirectoryInfo(content);
            var target = new DirectoryInfo(root);
            
            fileOrder = 0;
            CopyDirectory(source, target, pkgId, packgeFiles);

            // copy DLLs from lib to bin
            if (Directory.Exists(lib))
            {
                source = new DirectoryInfo(lib);
                target = new DirectoryInfo(bin);

                fileOrder = 0;
                CopyDirectory(source, target, pkgId, packgeFiles);
            }

            return packgeFiles;
        }

        /// <summary>
        /// Remove package files
        /// </summary>
        /// <param name="pkgId">Package Id</param>
        public static void UninstallPackage(string pkgId)
        {
            var installedFiles = BlogService.InstalledFromGalleryPackageFiles(pkgId);
            var pkg = PackageRepository.GetPackage(pkgId);
            
            foreach (var file in installedFiles.OrderByDescending(f => f.FileOrder))
            {
                var fullPath = HttpContext.Current.Server.MapPath(Path.Combine(Utils.RelativeWebRoot, file.FilePath));

                if(file.IsDirectory)
                {
                    var folder = new DirectoryInfo(fullPath);
                    if (folder.Exists)
                    {
                        if(folder.GetFileSystemInfos().Length == 0)
                        {
                            ForceDeleteDirectory(fullPath);
                        }
                        else
                        {
                            Utils.Log(string.Format("Package Uninstaller: can not remove directory if it is not empty ({0})", fullPath));
                        }
                    }

                }
                else if (File.Exists(fullPath))
                {
                    File.Delete(fullPath);
                }
            }

            if (!string.IsNullOrWhiteSpace(pkg.LocalVersion))
            {
                var pkgDir = string.Format("{0}.{1}", pkgId, pkg.LocalVersion);

                // clean up removing installed version
                pkgDir = HttpContext.Current.Server.MapPath(string.Format("{0}App_Data/packages/{1}", Utils.ApplicationRelativeWebRoot, pkgDir));
                if (Directory.Exists(pkgDir))
                {
                    ForceDeleteDirectory(pkgDir);
                }
            }
        }

        #region Private methods

        static List<JsonPackage> GetThemes()
        {
            var installedThemes = new List<JsonPackage>();
            var path = HttpContext.Current.Server.MapPath(string.Format("{0}themes/", Utils.ApplicationRelativeWebRoot));

            foreach (var p in from d in Directory.GetDirectories(path)
                let index = d.LastIndexOf(Path.DirectorySeparatorChar) + 1
                select d.Substring(index)
                into themeId select GetPackageManifest(themeId, Constants.Theme) ?? 
                new JsonPackage {Id = themeId, PackageType = Constants.Theme, Location = "L"}
                into p where p.Id != "RazorHost" select p)
            {
                if (string.IsNullOrEmpty(p.IconUrl))
                    p.IconUrl = DefaultIconUrl(p);

                installedThemes.Add(p);
            }
            return installedThemes;
        }

        static List<JsonPackage> GetWidgets()
        {
            var installedWidgets = new List<JsonPackage>();
            var path = HttpContext.Current.Server.MapPath(string.Format("{0}widgets/", Utils.ApplicationRelativeWebRoot));

            foreach (var p in from d in Directory.GetDirectories(path)
                let index = d.LastIndexOf(Path.DirectorySeparatorChar) + 1
                select d.Substring(index)
                into widgetId select GetPackageManifest(widgetId, Constants.Widget) ?? 
                new JsonPackage {Id = widgetId, PackageType = Constants.Widget, Location = "L"})
            {
                if (string.IsNullOrEmpty(p.IconUrl))
                    p.IconUrl = DefaultIconUrl(p);

                installedWidgets.Add(p);
            }
            return installedWidgets;
        }

        static JsonPackage GetPackageManifest(string id, string pkgType)
        {
            var jp = new JsonPackage { Id = id, PackageType = pkgType };

            var pkgUrl = pkgType == "Theme" ?
                string.Format("{0}themes/{1}/theme.xml", Utils.ApplicationRelativeWebRoot, id) :
                string.Format("{0}widgets/{1}/widget.xml", Utils.ApplicationRelativeWebRoot, id);

            var pkgPath = HttpContext.Current.Server.MapPath(pkgUrl);
            try
            {
                if (File.Exists(pkgPath))
                {
                    using (var textReader = new XmlTextReader(pkgPath))
                    {
                        textReader.Read();

                        while (textReader.Read())
                        {
                            textReader.MoveToElement();

                            if (textReader.Name == "description")
                                jp.Description = textReader.ReadString();

                            if (textReader.Name == "authors")
                                jp.Authors = textReader.ReadString();

                            if (textReader.Name == "website")
                                jp.Website = textReader.ReadString();

                            if (textReader.Name == "version")
                                jp.LocalVersion = textReader.ReadString();

                            if (textReader.Name == "iconurl")
                                jp.IconUrl = textReader.ReadString();
                        }
                        textReader.Close();
                    }
                    return jp;
                }
            }
            catch (Exception ex)
            {
                Utils.Log("Packaging.FileSystem.GetPackageManifest", ex);
            }
            return null;
        }

        static void CopyDirectory(DirectoryInfo source, DirectoryInfo target, string pkgId, List<PackageFile> installedFiles)
        {
            var rootPath = HttpContext.Current.Server.MapPath(Utils.RelativeWebRoot);

            foreach (var dir in source.GetDirectories())
            {
                var filePath = Path.Combine(target.FullName, dir.Name);
                var relPath = filePath.Replace(rootPath, "");

                // save directory if it is created by package
                // so we can remove it on package uninstall
                if (!Directory.Exists(filePath))
                {
                    fileOrder++;
                    var fileToCopy = new PackageFile
                    {
                        FilePath = relPath,
                        PackageId = pkgId,
                        FileOrder = fileOrder,
                        IsDirectory = true
                    };
                    installedFiles.Add(fileToCopy);
                }                   
                CopyDirectory(dir, target.CreateSubdirectory(dir.Name), pkgId, installedFiles);
            }
             
            foreach (var file in source.GetFiles())
            {
                var filePath = Path.Combine(target.FullName, file.Name);
                var relPath = filePath.Replace(rootPath, "");

                file.CopyTo(filePath);

                fileOrder++;
                var fileToCopy = new PackageFile
                {
                    FileOrder = fileOrder,
                    IsDirectory = false,
                    FilePath = relPath,
                    PackageId = pkgId
                };

                // fix known interface changes
                if (filePath.ToLower().EndsWith(".cs") ||
                    filePath.ToLower().EndsWith(".aspx") ||
                    filePath.ToLower().EndsWith(".ascx") ||
                    filePath.ToLower().EndsWith(".master"))
                {
                    ReplaceInFile(filePath, "BlogSettings.Instance.StorageLocation", "Blog.CurrentInstance.StorageLocation");
                    ReplaceInFile(filePath, "BlogSettings.Instance.FileExtension", "BlogConfig.FileExtension");
                    ReplaceInFile(filePath, "\"login.aspx", "\"account/login.aspx");
                }

                installedFiles.Add(fileToCopy);
            }   
        }

        static void ForceDeleteDirectory(string path)
        {
            DirectoryInfo fol;
            var fols = new Stack<DirectoryInfo>();
            var root = new DirectoryInfo(path);
            fols.Push(root);
            while (fols.Count > 0)
            {
                fol = fols.Pop();
                fol.Attributes = fol.Attributes & ~(FileAttributes.Archive | FileAttributes.ReadOnly | FileAttributes.Hidden);
                foreach (DirectoryInfo d in fol.GetDirectories())
                {
                    fols.Push(d);
                }
                foreach (FileInfo f in fol.GetFiles())
                {
                    f.Attributes = f.Attributes & ~(FileAttributes.Archive | FileAttributes.ReadOnly | FileAttributes.Hidden);
                    f.Delete();
                }
            }
            root.Delete(true);
        }

        static string DefaultIconUrl(JsonPackage pkg)
        {
            var validImages = new List<string> {"screenshot.jpg", "screenshot.png", "theme.jpg", "theme.png"};
            var pkgDir = pkg.PackageType == "Widget" ? "widgets" : "themes";

            foreach (var img in validImages)
            {
                var url = string.Format("{0}{1}/{2}/{3}",
                Utils.ApplicationRelativeWebRoot, pkgDir, pkg.Id, img);

                var path = HttpContext.Current.Server.MapPath(url);

                if (File.Exists(path)) return url;
            }

            if (pkg.PackageType == "Widget") 
                return Utils.ApplicationRelativeWebRoot + "pics/Widget.png";

            return Utils.ApplicationRelativeWebRoot + "pics/Theme.png";
        }

        static void ReplaceInFile(string filePath, string searchText, string replaceText)
        {
            var cnt = 0;
            StreamReader reader = new StreamReader(filePath);
            string content = reader.ReadToEnd();
            cnt = content.Length;
            reader.Close();

            content = Regex.Replace(content, searchText, replaceText);

            if (cnt > 0 && cnt != content.Length)
            {
                Utils.Log(string.Format("Package Installer: replacing in {0} from {1} to {2}", filePath, searchText, replaceText));
            }

            StreamWriter writer = new StreamWriter(filePath);
            writer.Write(content);
            writer.Close();
        }

        #endregion
    }
}
