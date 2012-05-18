<%@ WebService Language="C#" Class="Qnotes" %>
using System;
using System.Collections.Generic;
using System.Web.Script.Services;
using System.Web.Services;
using BlogEngine.Core.Json;
using BlogEngine.Core;
using BlogEngine.Core.Notes;
using System.Linq;
using System.Web;
using App_Code;

[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
[ScriptService]
public class Qnotes  : WebService {

    private QuickNotes UserNotes { get; set; }
    
    public Qnotes()
    {
        UserNotes = new QuickNotes(Security.CurrentUser.Identity.Name);
    }
    
    [WebMethod]
    public QuickSettings GetSettings() {
        return UserNotes.Settings;
    }

    [WebMethod]
    public QuickNotes GetQuickNotes()
    {
        return UserNotes;
    }
    
    [WebMethod]
    public IEnumerable<QuickNote> GetNotes()
    {       
        return UserNotes.Notes;
    }

    [WebMethod]
    public QuickNote GetNote(string id)
    {
        if (string.IsNullOrEmpty(id))
            return null;
        else
            return UserNotes.Notes.Where(n => n.Id.ToString() == id).FirstOrDefault();
    }

    [WebMethod]
    public QuickNote SaveNote(string id, string note)
    {
        note = HttpUtility.HtmlEncode(note);
        return UserNotes.SaveNote(id, note);
    }
    
    [WebMethod]
    public JsonResponse SaveSettings(string category, string tags)
    {
        UserNotes.SaveSettings(category, tags);
        return new JsonResponse { Success = true };
    }

    [WebMethod]
    public JsonResponse DeleteNote(string id)
    {
        UserNotes.Delete(id);
        return new JsonResponse { Success = true };
    }

    [WebMethod]
    public JsonResponse SaveQuickPost(string content)
    {
        if (!WebUtils.CheckRightsForAdminPostPages(false)) { return null; }

        var response = new JsonResponse { Success = false };
        var settings = BlogSettings.Instance;

        if (!Security.IsAuthorizedTo(Rights.CreateNewPosts))
        {
            response.Message = "Not authorized to create new Posts.";
            return response;
        }

        try
        {
            var post = new BlogEngine.Core.Post();
            var quickSettings = new QuickSettings(Security.CurrentUser.Identity.Name);
            var tags = "";
            var cats = "";

            foreach (var s in quickSettings.Settings)
            {
                if (s.SettingName == "tags")
                    tags = s.SettingValue;

                if (s.SettingName == "category")
                    cats = s.SettingValue;
            }

            content = HttpUtility.HtmlEncode(content).Replace("\n", "<br />");
            var title = content;

            int prdIdx = content.IndexOf(".");
            int brkIdx = content.IndexOf("<br />");

            // single sentence with ending period.
            if (prdIdx +1 == content.Length)
                prdIdx = -1;

            if (prdIdx > 0 && (brkIdx < 0 || brkIdx > prdIdx))
            {
                title = content.Substring(0, prdIdx);
                content = content.Substring(prdIdx + 1).Trim();
            }

            if (brkIdx > 0 && (prdIdx < 0 || prdIdx > brkIdx))
            {
                title = content.Substring(0, brkIdx);
                content = content.Substring(brkIdx + 6).Trim();
                if(content.StartsWith("<br />"))
                    content = content.Substring(6);
            }

            post.Author = Security.CurrentUser.Identity.Name;
            post.Title = title;
            post.Content = content;
            post.Description = title;
            post.Slug = title;

            var d = string.Format("{0}-{1}-{2} {3}:{4}", DateTime.Now.Year, DFormat(DateTime.Now.Month), DFormat(DateTime.Now.Day), DFormat(DateTime.Now.Hour), DFormat(DateTime.Now.Minute));
            post.DateCreated = DateTime.ParseExact(d, "yyyy-MM-dd HH:mm", null).AddHours(-BlogSettings.Instance.Timezone);

            post.IsPublished = true;
            post.HasCommentsEnabled = true;

            if (tags.Trim().Length > 0)
            {
                var vtags = tags.Trim().Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var tag in
                    vtags.Where(tag => string.IsNullOrEmpty(post.Tags.Find(t => t.Equals(tag.Trim(), StringComparison.OrdinalIgnoreCase)))))
                {
                    post.Tags.Add(tag.Trim());
                }
            }

            if (cats.Trim().Length > 0)
            {
                var vcats = cats.Trim().Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var cat in vcats)
                {
                    post.Categories.Add(Category.GetCategory(new Guid(cat)));
                }
            }
            
            post.Save();

            Utils.Log(string.Format("QuickNotes.Qnotes.SaveQuickPost(): {0} : {1}", post.Id, post.Description));

            response.Data = Utils.AbsoluteWebRoot.ToString();
        }
        catch (Exception ex)
        {
            Utils.Log(string.Format("QuickNotes.Qnotes.SaveQuickPost(): {0}", ex.Message));
            response.Message = string.Format("Could not save quick post: {0}", ex.Message);
            return response;
        }

        response.Success = true;
        response.Message = "Post saved";

        return response;
    }

    string DFormat(int iDate)
    {
        string s = iDate.ToString();
        return s.Length > 1 ? s : "0" + s;
    }
    
}