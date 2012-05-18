#region Using

using System;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web.UI;

using BlogEngine.Core;
using BlogEngine.Core.Web.Controls;

using Resources;

using Page = BlogEngine.Core.Page;

#endregion

/// <summary>
/// The page.
/// </summary>
public partial class page : BlogBasePage
{
    /// <summary>
    /// Raises the <see cref="E:System.Web.UI.Control.Init"/> event to initialize the page.
    /// </summary>
    /// <param name="e">An <see cref="T:System.EventArgs"/> that contains the event data.</param>
    protected override void OnInit(EventArgs e)
    {
        var queryString = this.Request.QueryString;
        var qsDeletePage = queryString["deletepage"];
        if (qsDeletePage != null && qsDeletePage.Length == 36)
        {
            this.DeletePage(new Guid(qsDeletePage));
        }

        var qsId = queryString["id"];
        if (qsId != null && qsId.Length == 36)
        {
            this.ServePage(new Guid(qsId));
            this.AddMetaTags();
        }
        else
        {
            this.Response.Redirect(Utils.RelativeWebRoot);
        }

        base.OnInit(e);
    }

    /// <summary>
    /// Serves the page to the containing DIV tag on the page.
    /// </summary>
    /// <param name="id">
    /// The id of the page to serve.
    /// </param>
    private void ServePage(Guid id)
    {
        var pg = BlogEngine.Core.Page.GetPage(id);
        this.Page = pg;

        if (pg == null || (!pg.IsVisible))
        {
            this.Response.Redirect(string.Format("{0}error404.aspx", Utils.RelativeWebRoot), true);
            return; // WLF: ReSharper is stupid and doesn't know that redirect returns this method.... or does it not...?
        }

        this.h1Title.InnerHtml = System.Web.HttpContext.Current.Server.HtmlEncode(pg.Title);

        var arg = new ServingEventArgs(pg.Content, ServingLocation.SinglePage);
        BlogEngine.Core.Page.OnServing(pg, arg);

        if (arg.Cancel)
        {
            this.Response.Redirect("error404.aspx", true);
        }

        if (arg.Body.Contains("[usercontrol", StringComparison.OrdinalIgnoreCase))
        {
            Utils.InjectUserControls(this.divText, arg.Body);
           // this.InjectUserControls(arg.Body);
        }
        else
        {
            this.divText.InnerHtml = arg.Body;
        }
    }

    /// <summary>
    /// Adds the meta tags and title to the HTML header.
    /// </summary>
    private void AddMetaTags()
    {
        if (this.Page == null)
        {
            return;
        }

        this.Title = this.Server.HtmlEncode(this.Page.Title);
        this.AddMetaTag("keywords", this.Server.HtmlEncode(this.Page.Keywords));
        this.AddMetaTag("description", this.Server.HtmlEncode(this.Page.Description));
    }

    /// <summary>
    /// Deletes the page.
    /// </summary>
    /// <param name="id">
    /// The page id.
    /// </param>
    private void DeletePage(Guid id)
    {
        var page = BlogEngine.Core.Page.GetPage(id);
        if (page == null)
        {
            return;
        }
        if (!page.CanUserDelete)
        {
            Response.Redirect(Utils.RelativeWebRoot);
            return;
        }

        page.Delete();
        page.Save();
        this.Response.Redirect(Utils.RelativeWebRoot, true);
    }

 
 
    /// <summary>
    ///     The Page instance to render on the page.
    /// </summary>
    public new Page Page;

    /// <summary>
    ///     Gets the admin links to edit and delete a page.
    /// </summary>
    /// <value>The admin links.</value>
    public string AdminLinks
    {
        get
        {
            if (!Security.IsAuthenticated)
            {
                return string.Empty;
            }

            var sb = new StringBuilder();

            if (this.Page.CanUserEdit)
            {
                if (sb.Length > 0) { sb.Append(" | "); }

                sb.AppendFormat(
                    "<a href=\"{0}admin/Pages/EditPage.aspx?id={1}\">{2}</a>",
                    Utils.RelativeWebRoot,
                    this.Page.Id,
                    labels.edit);
            }

            if (this.Page.CanUserDelete)
            {
                if (sb.Length > 0) { sb.Append(" | "); }

                sb.AppendFormat(
                    String.Concat("<a href=\"javascript:void(0);\" onclick=\"if (confirm('", labels.areYouSureDeletePage, "')) location.href='?deletepage={0}'\">{1}</a>"),
                    this.Page.Id,
                    labels.delete);
            }

            if (sb.Length > 0)
            {
                sb.Insert(0, "<div id=\"admin\">");
                sb.Append("</div>");
            }

            return sb.ToString();
        }
    }

    /// <summary>
    /// Gets PermaLink.
    /// </summary>
    public string PermaLink
    {
        get
        {
            return string.Format("{0}page.aspx?id={1}", Utils.AbsoluteWebRoot, this.Page.Id);
        }
    }
}