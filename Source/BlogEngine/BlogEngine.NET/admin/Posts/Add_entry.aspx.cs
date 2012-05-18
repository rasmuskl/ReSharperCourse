namespace Admin.Posts
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Web;
    using System.Web.Security;
    using System.Web.UI;
    using System.Web.UI.HtmlControls;
    using System.Web.UI.WebControls;

    using BlogEngine.Core;

    using Resources;

    using Page = System.Web.UI.Page;
    using App_Code;
    using BlogEngine.Core.Providers;

    /// <summary>
    /// The AddEntry.
    /// </summary>
    public partial class AddEntry : Page, ICallbackEventHandler
    {
        #region Constants and Fields

        /// <summary>
        /// The raw editor cookie.
        /// </summary>
        private const string RawEditorCookie = "useraweditor";

        /// <summary>
        /// The callback.
        /// </summary>
        private string callback;

        /// <summary>
        /// URL of the current post
        /// </summary>
        protected string PostUrl
        {
            get
            {
                if (!String.IsNullOrEmpty(Request.QueryString["id"]) && Request.QueryString["id"].Length == 36)
                {
                    var id = new Guid(Request.QueryString["id"]);
                    var p = Post.GetPost(id);
                    return p.RelativeLink;
                }
                return string.Empty;
            }
        }

        #endregion

        #region Implemented Interfaces

        #region ICallbackEventHandler

        /// <summary>
        /// Returns the results of a callback event that targets a control.
        /// </summary>
        /// <returns>
        /// The result of the callback.
        /// </returns>
        public string GetCallbackResult()
        {
            return callback;
        }

        /// <summary>
        /// Processes a callback event that targets a control.
        /// </summary>
        /// <param name="eventArgument">
        /// A string that represents an event argument to pass to the event handler.
        /// </param>
        public void RaiseCallbackEvent(string eventArgument)
        {
            if (eventArgument.StartsWith("_autosave"))
            {
                var fields = eventArgument.Replace("_autosave", string.Empty).Split(
                    new[] { ";|;" }, StringSplitOptions.None);
                Session["content"] = fields[0];
                Session["title"] = fields[1];
                Session["description"] = fields[2];
                Session["slug"] = fields[3];
                Session["tags"] = fields[4];
            }
            else
            {
                callback = Utils.RemoveIllegalCharacters(eventArgument.Trim());
            }
        }

        #endregion

        #endregion

        #region Methods

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Load"/> event.
        /// </summary>
        /// <param name="e">
        /// The <see cref="T:System.EventArgs"/> object that contains the event data.
        /// </param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            txtTitle.Focus();
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init"/> event to initialize the page.
        /// </summary>
        /// <param name="e">
        /// An <see cref="T:System.EventArgs"/> that contains the event data.
        /// </param>
        protected override void OnInit(EventArgs e)
        {
            WebUtils.CheckRightsForAdminPostPages(false);
            MaintainScrollPositionOnPostBack = true;

            BindTags();
            BindUsers();

            Page.Title = labels.add_Entry;
            Page.ClientScript.GetCallbackEventReference(this, "title", "ApplyCallback", "slug");

            if (!String.IsNullOrEmpty(Request.QueryString["id"]) && Request.QueryString["id"].Length == 36)
            {
                var id = new Guid(Request.QueryString["id"]);
                Page.Title = string.Format("{0} {1}", labels.edit, labels.post);
                BindPost(id);
                BindCategories(id);
            }
            else
            {
                BindCategories(Guid.Empty);
                PreSelectAuthor(Page.User.Identity.Name);
                txtDate.Text = DateTime.Now.AddHours(BlogSettings.Instance.Timezone).ToString("yyyy-MM-dd");
                txtTime.Text = DateTime.Now.AddHours(BlogSettings.Instance.Timezone).ToString("HH\\:mm");
                cbEnableComments.Checked = BlogSettings.Instance.IsCommentsEnabled;
                cbPublish.Checked = Security.IsAuthorizedTo(Rights.PublishOwnPosts);
                if (Session["content"] != null)
                {
                    txtContent.Text = Session["content"].ToString();
                    txtRawContent.Text = txtContent.Text;
                    txtTitle.Text = Session["title"].ToString();
                    txtDescription.Text = Session["description"].ToString();
                    txtSlug.Text = Session["slug"].ToString();
                    txtTags.Text = Session["tags"].ToString();
                }

                BindBookmarklet();
            }

            if (!Security.IsAuthorizedTo(Rights.EditOtherUsersPosts))
            {
                ddlAuthor.Enabled = false;
            }

            cbEnableComments.Enabled = BlogSettings.Instance.IsCommentsEnabled;

            if (Request.Cookies[RawEditorCookie] != null)
            {
                txtRawContent.Visible = true;
                txtContent.Visible = false;
                cbUseRaw.Checked = true;
            }

            btnCategory.Click += BtnCategoryClick;
            btnUploadFile.Click += BtnUploadFileClick;
            btnUploadImage.Click += BtnUploadImageClick;
			btnUploadVideo.Click += BtnUploadVideoClick;
            valExist.ServerValidate += ValExistServerValidate;
            cbUseRaw.CheckedChanged += CbUseRawCheckedChanged;

            base.OnInit(e);
        }

        /// <summary>
        /// The bind bookmarklet.
        /// </summary>
        private void BindBookmarklet()
        {
            if (Request.QueryString["title"] == null || Request.QueryString["url"] == null)
            {
                return;
            }

            var title = Request.QueryString["title"];
            var url = Request.QueryString["url"];

            txtTitle.Text = title;
            txtContent.Text = string.Format("<p><a href=\"{0}\" title=\"{1}\">{1}</a></p>", url, title);
        }

        /// <summary>
        /// The bind categories.
        /// </summary>
        private void BindCategories(Guid postId)
        {
            string catHtml = "";
            var post = postId == Guid.Empty ? null : Post.GetPost(postId);

            foreach (var cat in Category.Categories)
            {
                string chk = "";
                if(post != null && post.Categories.Contains(cat))
                    chk = "checked=\"checked\"";

                catHtml += string.Format("<input type=\"checkbox\" {0} id=\"{1}\">", chk, cat.Id);
                catHtml += string.Format("<label>{0}</label><br/>", Server.HtmlEncode(cat.Title));
            }
            cblCategories.InnerHtml = catHtml;
        }

        /// <summary>
        /// The bind post.
        /// </summary>
        /// <param name="postId">
        /// The post id.
        /// </param>
        private void BindPost(Guid postId)
        {
            var post = Post.GetPost(postId);

            if (post == null || !post.CanUserEdit)
            {
                Response.Redirect(Request.Path);
            }

            if (post != null)
            {
                txtTitle.Text = post.Title;
                txtContent.Text = post.Content;
                txtRawContent.Text = post.Content;
                txtDescription.Text = post.Description;
                txtDate.Text = post.DateCreated.ToString("yyyy-MM-dd");
                txtTime.Text = post.DateCreated.ToString("HH\\:mm");
                cbEnableComments.Checked = post.HasCommentsEnabled;
                cbPublish.Checked = post.IsPublished;
                txtSlug.Text = Utils.RemoveIllegalCharacters(post.Slug);
                PreSelectAuthor(post.Author);

                var tags = new string[post.Tags.Count];
                for (var i = 0; i < post.Tags.Count; i++)
                {
                    tags[i] = post.Tags[i];
                }

                txtTags.Text = string.Join(",", tags);
            }
        }

        /// <summary>
        /// The bind tags.
        /// </summary>
        private void BindTags()
        {
            var col = new List<string>();
            foreach (var tag in from post in Post.Posts from tag in post.Tags where !col.Contains(tag) select tag)
            {
                col.Add(tag);
            }

            col.Sort(String.Compare);

            foreach (var a in col.Select(tag => new HtmlAnchor { HRef = "javascript:void(0)", InnerText = tag }))
            {
                a.Attributes.Add("onclick", "AddTag(this)");
                phTags.Controls.Add(a);
            }
        }

        /// <summary>
        /// The bind users.
        /// </summary>
        private void BindUsers()
        {
            foreach (MembershipUser user in Membership.GetAllUsers())
            {
                ddlAuthor.Items.Add(user.UserName);
            }
        }

        /// <summary>
        /// The pre select author.
        /// </summary>
        /// <param name="author">
        /// The author.
        /// </param>
        private void PreSelectAuthor(string author)
        {
            ddlAuthor.ClearSelection();
            foreach (ListItem item in
                ddlAuthor.Items.Cast<ListItem>().Where(item => item.Text.Equals(author, StringComparison.OrdinalIgnoreCase)))
            {
                item.Selected = true;
                break;
            }
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

        /// <summary>
        /// Uploads the specified virtual folder.
        /// </summary>
        /// <param name="virtualFolder">The virtual folder.</param>
        /// <param name="control">The control.</param>
        /// <param name="fileName">Name of the file.</param>
        private void Upload(string virtualFolder, FileUpload control, string fileName)
        {
            var folder = Server.MapPath(virtualFolder);
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            control.PostedFile.SaveAs(folder + fileName);
        }

        /// <summary>
        /// Handles the Click event of the btnCategory control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void BtnCategoryClick(object sender, EventArgs e)
        {
            if (!Page.IsValid)
            {
                return;
            }

            var cat = new Category(txtCategory.Text, string.Empty);
            cat.Save();
            var item = new ListItem(Server.HtmlEncode(txtCategory.Text), cat.Id.ToString())
                {
                    Selected = true
                };
            string catHtml = string.Format("<input type=\"checkbox\" id=\"{0}\">", cat.Id);
            catHtml += string.Format("<label>{0}</label><br/>", Server.HtmlEncode(cat.Title));
            cblCategories.InnerHtml += catHtml;

            string postId = Request.QueryString["id"];
            Post post = null;


            // Security Rights validation

            if (postId == null)
            {
                Security.DemandUserHasRight(Rights.CreateNewPosts, true);
                post = new Post();
            }
            else
            {
                post = Post.GetPost(new Guid(postId));

                if (post.CurrentUserOwns)
                {
                    Security.DemandUserHasRight(Rights.EditOwnPosts, true);
                }
                else
                {
                    Security.DemandUserHasRight(Rights.EditOtherUsersPosts, true);
                }
            }
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
            txtRawContent.Text += string.Format("<p><a href=\"{0}\">{1}</a></p>", file.FileDownloadPath, file.FileDescription);
        }

        /// <summary>
        /// Handles the Click event of the btnUploadImage control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void BtnUploadImageClick(object sender, EventArgs e)
        {
            var dirName = string.Format("/{0}/{1}", DateTime.Now.ToString("yyyy"), DateTime.Now.ToString("MM"));
            var dir = BlogService.GetDirectory(dirName);
            var file = BlogService.UploadFile(txtUploadImage.PostedFile.InputStream, txtUploadImage.PostedFile.FileName, dir, true);

            txtContent.Text += string.Format("<img src=\"{0}image.axd?picture={1}\" />", Utils.RelativeWebRoot, Server.UrlEncode(file.AsImage.FilePath));
            txtRawContent.Text += string.Format("<img src=\"{0}image.axd?picture={1}\" />", Utils.RelativeWebRoot, Server.UrlEncode(file.AsImage.FilePath));
        }

		/// <summary>
        /// Handles the Click event of the btnUploadVideo control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void BtnUploadVideoClick(object sender, EventArgs e) {
			
			// default media folder
			var mediaFolder = "media";

			// get the mediaplayer extension and use it's folder
			var mediaPlayerExtension = BlogEngine.Core.Web.Extensions.ExtensionManager.GetExtension("MediaElementPlayer");
			mediaFolder = mediaPlayerExtension.Settings[0].GetSingleValue("folder");

			var folder = Utils.RelativeWebRoot + mediaFolder + "/";
			var fileName = txtUploadVideo.FileName;

			Upload(folder, txtUploadVideo, fileName);

			var shortCode = "[video src=\"" + fileName + "\"]";

			txtContent.Text += shortCode;
			txtRawContent.Text += shortCode;
		}


        /// <summary>
        /// Handles the CheckedChanged event of the cbUseRaw control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void CbUseRawCheckedChanged(object sender, EventArgs e)
        {
            if (cbUseRaw.Checked)
            {
                txtRawContent.Text = txtContent.Text;
                var cookie = new HttpCookie(RawEditorCookie, "1") { Expires = DateTime.Now.AddYears(3) };
                Response.Cookies.Add(cookie);
            }
            else
            {
                txtContent.Text = txtRawContent.Text;
                if (Request.Cookies[RawEditorCookie] != null)
                {
                    var cookie = new HttpCookie(RawEditorCookie) { Expires = DateTime.Now.AddYears(-3) };
                    Response.Cookies.Add(cookie);
                }
            }

            txtRawContent.Visible = cbUseRaw.Checked;
            txtContent.Visible = !cbUseRaw.Checked;

            // Response.Redirect(Request.RawUrl);
        }

        /// <summary>
        /// Handles the ServerValidate event of the valExist control.
        /// </summary>
        /// <param name="source">The source of the event.</param>
        /// <param name="args">The <see cref="System.Web.UI.WebControls.ServerValidateEventArgs"/> instance containing the event data.</param>
        private void ValExistServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid =
                !Category.Categories.Any(
                    cat => cat.Title.Equals(txtCategory.Text.Trim(), StringComparison.OrdinalIgnoreCase));
        }

        #endregion
    }
}