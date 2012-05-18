<%@ WebHandler Language="C#" Class="FileUpload" %>

using System;
using System.Web;
using System.Web.Hosting;
using System.Linq;
using System.IO;
using System.Web.SessionState;
using BlogEngine.Core;
using BlogEngine.Core.Providers;
public class FileUpload : IHttpHandler, IRequiresSessionState {

    public enum FileType
    {
        Image,
        File
    }
    
    public void ProcessRequest(HttpContext context)
    {
        var files = context.Request.Files;
        if (files.Count == 0)
        {
            context.Response.Write("0:Unknown file upload");
            context.Response.End();
        }
        
        var file = files[0];
        var dirName = string.Format("/{0}/{1}", DateTime.Now.ToString("yyyy"), DateTime.Now.ToString("MM"));
        var dir = BlogService.GetDirectory(dirName);  
        
        var uFile = BlogService.UploadFile(file.InputStream, file.FileName, dir, true);
        if (uFile.IsImage)
        {
            var imgString = string.Format("<img src=\"{0}\" />", uFile.AsImage.ImageUrl);
            context.Response.Write(string.Format("1:{0}:{1}:{2}", imgString, uFile.Name, uFile.ParentDirectory.FullPath));
            context.Response.End();
        }
        else
        {
            var fileString = string.Format("<p><a href=\"{0}\" >{1}</a></p>", uFile.FileDownloadPath, uFile.FileDescription);
            context.Response.Write(string.Format("1:{0}:{1}:{2}", fileString, uFile.Name, uFile.ParentDirectory.FullPath));
            context.Response.End();
        }
        
        context.Response.Write("0:Upload failed..");
    }
 
    public bool IsReusable {
        get {
            return false;
        }
    }


    private FileType ExtractType(string Extension)
    {
        if (Extension == ".png" ||
            Extension == ".jpg" ||
            Extension == ".jpeg" ||
            Extension == ".bmp" ||
            Extension == ".gif")
            return FileType.Image;
        else
            return FileType.File;
    }
    
    /// <summary>
    /// Sizes the format.
    /// </summary>
    /// <param name="size">
    /// The string size.
    /// </param>
    /// <param name="formatString">
    /// The format string.
    /// </param>
    /// <returns>
    /// The string.
    /// </returns>
    private static string SizeFormat(float size, string formatString)
    {
        if (size < 1024)
        {
            return string.Format("{0} bytes", size.ToString(formatString));
        }

        if (size < Math.Pow(1024, 2))
        {
            return string.Format("{0} kb", (size / 1024).ToString(formatString));
        }

        if (size < Math.Pow(1024, 3))
        {
            return string.Format("{0} mb", (size / Math.Pow(1024, 2)).ToString(formatString));
        }

        if (size < Math.Pow(1024, 4))
        {
            return string.Format("{0} gb", (size / Math.Pow(1024, 3)).ToString(formatString));
        }

        return size.ToString(formatString);
    }

}