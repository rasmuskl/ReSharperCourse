using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Services;
using System.Collections;
using App_Code;
using BlogEngine.Core.Json;
using BlogEngine.Core;
using System.IO;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using BlogEngine.Core.Providers;

public partial class admin_AjaxFileManager : System.Web.UI.Page
{
    [WebMethod]
    public static JsonResponse ChangeImage(string path, int change)
    {
        JsonResponse response = new JsonResponse()
        {
            Data = string.Empty,
            Success = false,
            Message = ""
        };
        //check rights
        if (!Security.IsAuthorizedTo(Rights.CreateNewPosts))
        {
            response.Success = false;
            response.Message = "Access denied";
            return response;
        }
        var itemChange = BlogEngine.Core.FileSystem.Image.ModificationType.FlipHorizontal;
        
        switch (change)
        {
            case 0:
                itemChange = BlogEngine.Core.FileSystem.Image.ModificationType.FlipHorizontal;
                break;
            case 1:
                itemChange = BlogEngine.Core.FileSystem.Image.ModificationType.FlipVertical;
                break;
            case 2:
                itemChange = BlogEngine.Core.FileSystem.Image.ModificationType.RotateLeft;
                break;
            case 3:
                itemChange = BlogEngine.Core.FileSystem.Image.ModificationType.RotateRight;
                break;
        }
        BlogService.GetFile(path).AsImage.ModifyImage(itemChange);
        return new JsonResponse()
        {
            Success = true,
            Message = "",
            Data = ""

        };
    }

    [WebMethod]
    public static JsonResponse ResizeImage(string path, int percent)
    {
        JsonResponse response = new JsonResponse()
        {
            Data = string.Empty,
            Success = false,
            Message = ""
        };
        //check rights
        if (!Security.IsAuthorizedTo(Rights.CreateNewPosts))
        {
            response.Success = false;
            response.Message = "Access denied";
            return response;
        }
        BlogService.GetFile(path).AsImage.ResizeImage((decimal)percent);
        return new JsonResponse()
        {
            Success = true,
            Data = "",
            Message = ""
        };
    }

    [WebMethod]
    public static JsonResponse CropImage(string path, int x, int y, int width, int height)
    {
        JsonResponse response = new JsonResponse()
        {
            Data = string.Empty,
            Success = false,
            Message = ""
        };
        //check rights
        if (!Security.IsAuthorizedTo(Rights.CreateNewPosts))
        {
            response.Success = false;
            response.Message = "Access denied";
            return response;
        }
        BlogService.GetFile(path).AsImage.CropImage(x, y, width, height);
        return new JsonResponse()
        {
            Success = true,
            Data = path
        };
    }


    [WebMethod]
    public static JsonResponse RenameFile(string path, string newname)
    {
        JsonResponse response = new JsonResponse()
        {
            Data = string.Empty,
            Success = false,
            Message = ""
        };
        //check rights
        if (!Security.IsAuthorizedTo(Rights.CreateNewPosts))
        {
            response.Success = false;
            response.Message = "Access denied";
            return response;
        }
        
        try
        {
            var file = BlogService.GetFile(path).Rename(newname);
            return (response = new JsonResponse()
            {
                Data = file.ParentDirectory.FullPath,
                Success = true,
                Message = "File \"" + path + "\" has been renamed to \"" + newname + "\"."
            });
        }
        catch
        {
            return new JsonResponse()
            {
                Success = false,
                Message = "There was an error while renaming your file, please try again or specify a new name."
            };
        }
    }

    [WebMethod]
    public static JsonResponse DeleteFile(string path)
    {
        JsonResponse response = new JsonResponse()
        {
            Data = string.Empty,
            Success = false,
            Message = ""
        };
        //check rights
        if (!Security.IsAuthorizedTo(Rights.CreateNewPosts))
        {
            response.Success = false;
            response.Message = "Access denied";
            return response;
        }

        var file = BlogService.GetFile(path);
        var directory = file.ParentDirectory;
        file.Delete();
        return (response = new JsonResponse()
        {
            Data = directory.FullPath,
            Success = true,
            Message = "File \"" + new FileInfo(path).Name + "\" has been deleted."
        });
    }

    [WebMethod]
    public static JsonResponse AppendFile(string path)
    {
        JsonResponse response = new JsonResponse()
        {
            Data = string.Empty,
            Success = false,
            Message = ""
        };
        //check rights
        if (!Security.IsAuthorizedTo(Rights.CreateNewPosts))
        {
            response.Success = false;
            response.Message = "Access denied";
            return response;
        }

        var file = BlogService.GetFile(path);
        if (file != null)
        {
            if (file.IsImage)
            {
                var imagePath = string.Format("<img src=\"{0}\" />", file.AsImage.ImageUrl);
                response.Success = true;
                response.Data = imagePath;
            }
            else
            {
                var filePath = string.Format("<p><a href=\"{1}\" >{0}</a></p>", file.FileDescription, file.FileDownloadPath);
                response.Success = true;
                response.Data = filePath;
            }
        }
        return response;
    }


    [WebMethod]
    public static JsonResponse<FileResponse> GetFiles(string path)
    {
        JsonResponse<FileResponse> response = new JsonResponse<FileResponse>()
        {
            Data = new FileResponse(),
            Success = false,
            Message = ""
        };
        //check rights
        if (!Security.IsAuthorizedTo(Rights.CreateNewPosts)) {
            response.Success = false;
            response.Message = "Access denied";
            return response;
        }
        var rwr = Utils.RelativeWebRoot;
        List<FileInstance> list = new List<FileInstance>();
        var directory = BlogService.GetDirectory(path);
        if (!directory.IsRoot)
        {
            list.Add(new FileInstance()
            {
                FileSize = "",
                FileType = FileType.Directory,
                Created = "",
                FullPath = "",
                Image = rwr + "admin/filemanager/images/up-directory.png",
                Name = "..."
            });
            response.Data.Path = "root" + directory.FullPath;
        }
        else
            response.Data.Path = "root";

        foreach (var dir in directory.Directories)
            list.Add(new FileInstance()
            {
                FileSize = "",
                FileType = FileType.Directory,
                Created = dir.DateCreated.ToString(),
                FullPath = dir.FullPath,
                Image = rwr + "admin/filemanager/images/folder.png",
                Name = dir.Name.Replace("/","")
            });


        foreach (var file in directory.Files)
            list.Add(new FileInstance()
            {
                FileSize = file.FileSizeFormat,
                Created = file.DateCreated.ToString(),
                FileType = FileType.File,
                Image = !file.IsImage ? rwr + "admin/filemanager/images/page_white_world.png" : rwr + "admin/filemanager/images/page_white_picture.png",
                FullPath = file.FilePath,
                Name = file.Name
            });


        var pathPieces = response.Data.Path.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
        StringBuilder pathString = new StringBuilder();
        pathString.Append("<a href=\"javascript:;\" class=\"fmdPathPiece\" data-path=\"\">root</a>");
        var hPath = "";
        foreach (var p in pathPieces.Skip(1))
        {
            hPath = string.Format("{0}/{1}", hPath, p);
            pathString.AppendFormat("/<a href=\"javascript:;\" class=\"fmdPathPiece\" data-path=\"{0}\">{1}</a>", hPath, p);
        }
        response.Data.Path = pathString.ToString();
        response.Success = true;
        response.Message = string.Empty;
        response.Data.Files = list;
        return response;
    }

    public class FileResponse
    {
        public FileResponse()
        {
            Files = new List<FileInstance>();
            Path = string.Empty;
        }
        public IEnumerable<FileInstance> Files { get; set; }
        public string Path { get; set; }
    }

    public class FileInstance
    {
        public string Image { get; set; }
        public string Created { get; set; }
        public string Name { get; set; }
        public string FileSize { get; set; }
        public FileType FileType { get; set; }
        public string FullPath { get; set; }
    }

    public enum FileType
    {
        Directory,
        File,
        Image,
        None
    }
}