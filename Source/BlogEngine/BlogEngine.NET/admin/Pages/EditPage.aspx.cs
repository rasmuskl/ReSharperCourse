// --------------------------------------------------------------------------------------------------------------------
// <summary>
//   The page editor.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Admin.Pages
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Web.UI;
    using System.Web.UI.HtmlControls;
    using System.Web.UI.WebControls;

    using BlogEngine.Core;

    using Resources;

    using Page = System.Web.UI.Page;
    using App_Code;
    using BlogEngine.Core.Providers;

    public partial class EditPage : Page, ICallbackEventHandler
    {
        protected string PageUrl
        {
            get 
            {
                if (!String.IsNullOrEmpty(this.Request.QueryString["id"]) && this.Request.QueryString["id"].Length == 36)
                {
                    var id = new Guid(this.Request.QueryString["id"]);
                    BlogEngine.Core.Page pg = BlogEngine.Core.Page.GetPage(id);
                    return pg.RelativeLink;
                }
                return string.Empty;
            }
        }
        #region Constants and Fields

        /// <summary>
        /// The callback.
        /// </summary>
        private string callback;

        #endregion

        #region Implemented Interfaces

        #region ICallbackEventHandler

        /// <summary>
        /// Returns the results of a callback event that targets a control.
        /// </summary>
        /// <returns>The result of the callback.</returns>
        public string GetCallbackResult()
        {
            return this.callback;
        }

        /// <summary>
        /// Processes a callback event that targets a control.
        /// </summary>
        /// <param name="eventArgument">A string that represents an event argument to pass to the event handler.</param>
        public void RaiseCallbackEvent(string eventArgument)
        {
            this.callback = Utils.RemoveIllegalCharacters(eventArgument.Trim());
        }

        #endregion

        #endregion

        #region Methods

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init"/> event to initialize the page.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> that contains the event data.</param>
        protected override void OnInit(EventArgs e)
        {
            WebUtils.CheckRightsForAdminPagesPages(false);
            this.MaintainScrollPositionOnPostBack = true;

            if (!String.IsNullOrEmpty(this.Request.QueryString["id"]) && this.Request.QueryString["id"].Length == 36)
            {
                var id = new Guid(this.Request.QueryString["id"]);
                this.BindPage(id);
                this.BindParents(id);
            }
            else if (!String.IsNullOrEmpty(this.Request.QueryString["delete"]) &&
                     this.Request.QueryString["delete"].Length == 36)
            {
                var id = new Guid(this.Request.QueryString["delete"]);
                this.DeletePage(id);
            }
            else
            {
                if (!Security.IsAuthorizedTo(Rights.CreateNewPages))
                {
                    Response.Redirect(Utils.RelativeWebRoot);
                    return;
                }

                this.BindParents(Guid.Empty);
                this.cbPublished.Checked = Security.IsAuthorizedTo(Rights.PublishOwnPages);
            }

            this.btnUploadFile.Click += this.BtnUploadFileClick;
            this.btnUploadImage.Click += this.BtnUploadImageClick;
            this.btnUploadVideo.Click += this.BtnUploadVideoClick;
            this.Page.Title = labels.pages;

            base.OnInit(e);
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Load"/> event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.EventArgs"/> object that contains the event data.</param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!this.Page.IsPostBack && !this.Page.IsCallback)
            {
                this.Page.ClientScript.GetCallbackEventReference(this, "title", "ApplyCallback", "slug");
            }
        }

        /// <summary>
        /// The bind page.
        /// </summary>
        /// <param name="pageId">
        /// The page id.
        /// </param>
        private void BindPage(Guid pageId)
        {
            var page = BlogEngine.Core.Page.GetPage(pageId);

            if (page == null || !page.CanUserEdit)
            {
                Response.Redirect(Request.Path);
                return;
            }

            this.txtTitle.Text = page.Title;
            this.txtContent.Text = page.Content;
            this.txtDescription.Text = page.Description;
            this.txtKeyword.Text = page.Keywords;
            this.txtSlug.Text = page.Slug;
            this.cbFrontPage.Checked = page.IsFrontPage;
            this.cbShowInList.Checked = page.ShowInList;
            this.cbPublished.Checked = page.IsPublished;
        }

        /// <summary>
        /// The bind parents.
        /// </summary>
        /// <param name="pageId">
        /// The page id.
        /// </param>
        private void BindParents(Guid pageId)
        {
            foreach (var page in BlogEngine.Core.Page.Pages.Where(page => pageId != page.Id))
            {
                this.ddlParent.Items.Add(new ListItem(page.Title, page.Id.ToString()));
            }

            this.ddlParent.Items.Insert(0, string.Format("-- {0} --", labels.noParent));
            if (pageId == Guid.Empty)
            {
                return;
            }

            var parent = BlogEngine.Core.Page.GetPage(pageId);
            if (parent != null)
            {
                this.ddlParent.SelectedValue = parent.Parent.ToString();
            }
        }

        /// <summary>
        /// Builds the child page list.
        /// </summary>
        /// <param name="page">The page to make a child list for.</param>
        /// <returns>The page list.</returns>
        private HtmlGenericControl BuildChildPageList(BlogEngine.Core.Page page)
        {
            var ul = new HtmlGenericControl("ul");
            foreach (var childPage in BlogEngine.Core.Page.Pages.FindAll(p => p.Parent == page.Id))
            {
                var cLi = new HtmlGenericControl("li");
                cLi.Attributes.CssStyle.Add("font-weight", "normal");
                var cA = new HtmlAnchor { HRef = string.Format("?id={0}", childPage.Id), InnerHtml = childPage.Title };

                var childText = new LiteralControl(string.Format(" ({0}) ", childPage.DateCreated.ToString("yyyy-dd-MM HH:mm")));

                const string DeleteText = "Are you sure you want to delete the page?";
                var delete = new HtmlAnchor { InnerText = labels.delete };
                delete.Attributes["onclick"] = string.Format("if (confirm('{0}')){{location.href='?delete={1}'}}", DeleteText, childPage.Id);
                delete.HRef = "javascript:void(0);";
                delete.Style.Add(HtmlTextWriterStyle.FontWeight, "normal");

                cLi.Controls.Add(cA);
                cLi.Controls.Add(childText);
                cLi.Controls.Add(delete);

                if (childPage.HasChildPages)
                {
                    cLi.Attributes.CssStyle.Remove("font-weight");
                    cLi.Attributes.CssStyle.Add("font-weight", "bold");
                    cLi.Controls.Add(this.BuildChildPageList(childPage));
                }

                ul.Controls.Add(cLi);
            }

            return ul;
        }

        /// <summary>
        /// The delete page.
        /// </summary>
        /// <param name="pageId">
        /// The page id.
        /// </param>
        private void DeletePage(Guid pageId)
        {
            var page = BlogEngine.Core.Page.GetPage(pageId);
            if (page == null)
            {
                return;
            }
            if (!page.CanUserDelete)
            {
                Response.Redirect(Utils.RelativeWebRoot);
                return;
            }

            this.ResetParentPage(page);
            page.Delete();
            page.Save();
            this.Response.Redirect("pages.aspx");
        }

        /// <summary>
        /// Resets the parent page.
        /// </summary>
        /// <param name="page">The page to reset.</param>
        private void ResetParentPage(BlogEngine.Core.Page page)
        {
            foreach (var child in BlogEngine.Core.Page.Pages.Where(child => page.Id == child.Parent))
            {
                child.Parent = Guid.Empty;
                child.Save();
                this.ResetParentPage(child);
            }
        }

        /// <summary>
        /// Formats the size.
        /// </summary>
        /// <param name="size">The size to format.</param>
        /// <param name="formatString">The format string.</param>
        /// <returns>The formatted string.</returns>
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

        /// <summary>
        /// Uploads the specified virtual folder.
        /// </summary>
        /// <param name="virtualFolder">The virtual folder.</param>
        /// <param name="control">The control.</param>
        /// <param name="fileName">Name of the file.</param>
        private void Upload(string virtualFolder, FileUpload control, string fileName)
        {
            var folder = this.Server.MapPath(virtualFolder);
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            control.PostedFile.SaveAs(folder + fileName);
        }

        /// <summary>
        /// Handles the Click event of the btnSave control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void BtnSaveClick(object sender, EventArgs e)
        {
            if (!this.Page.IsValid)
            {
                throw new InvalidOperationException("One or more validators are invalid.");
            }

            var page = this.Request.QueryString["id"] != null ? BlogEngine.Core.Page.GetPage(new Guid(this.Request.QueryString["id"])) : new BlogEngine.Core.Page();

            if (string.IsNullOrEmpty(this.txtContent.Text))
            {
                this.txtContent.Text = "[No text]";
            }

            page.Title = this.txtTitle.Text;
            page.Content = this.txtContent.Text;
            page.Description = this.txtDescription.Text;
            page.Keywords = this.txtKeyword.Text;

            if (this.cbFrontPage.Checked)
            {
                foreach (var otherPage in BlogEngine.Core.Page.Pages.Where(otherPage => otherPage.IsFrontPage))
                {
                    otherPage.IsFrontPage = false;
                    otherPage.Save();
                }
            }

            page.IsFrontPage = this.cbFrontPage.Checked;
            page.ShowInList = this.cbShowInList.Checked;
            page.IsPublished = this.cbPublished.Checked;

            if (!string.IsNullOrEmpty(this.txtSlug.Text))
            {
                page.Slug = Utils.RemoveIllegalCharacters(this.txtSlug.Text.Trim());
            }

            page.Parent = this.ddlParent.SelectedIndex != 0 ? new Guid(this.ddlParent.SelectedValue) : Guid.Empty;

            page.Save();

            this.Response.Redirect(page.RelativeLink);
        }

        /// <summary>
        /// Handles the Click event of the btnUploadFile control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void BtnUploadFileClick(object sender, EventArgs e)
        {
            var dirName = string.Format("/{0}/{1}", DateTime.Now.ToString("yyyy"), DateTime.Now.ToString("MM"));
            var dir = BlogService.GetDirectory(dirName);
            var file = BlogService.UploadFile(txtUploadFile.PostedFile.InputStream, txtUploadFile.PostedFile.FileName, dir, true);
            txtContent.Text += string.Format("<p><a href=\"{0}\">{1}</a></p>", file.FileDownloadPath, file.FileDescription);
        }

        /// <summary>
        /// Handles the Click event of the btnUploadImage control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void BtnUploadImageClick(object sender, EventArgs e)
        {
            var dirName = string.Format("/{0}/{1}", DateTime.Now.ToString("yyyy"), DateTime.Now.ToString("MM"));
            var dir = BlogService.GetDirectory(dirName);
            var file = BlogService.UploadFile(txtUploadImage.PostedFile.InputStream, txtUploadImage.PostedFile.FileName, dir, true);
            txtContent.Text += string.Format("<img src=\"{0}\" />", file.AsImage.ImageUrl);
        }

        /// <summary>
        /// Handles the Click event of the btnUploadVideo control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void BtnUploadVideoClick(object sender, EventArgs e)
        {

            // default media folder
            var mediaFolder = "media";

            // get the mediaplayer extension and use it's folder
            var mediaPlayerExtension = BlogEngine.Core.Web.Extensions.ExtensionManager.GetExtension("MediaElementPlayer");
            mediaFolder = mediaPlayerExtension.Settings[0].GetSingleValue("folder");

            var folder = "~/" + mediaFolder + "/";
            var fileName = txtUploadVideo.FileName;

            Upload(folder, txtUploadVideo, fileName);

            var shortCode = "[video src=\"" + fileName + "\"]";

            txtContent.Text += shortCode;
        }

        #endregion
    }
}