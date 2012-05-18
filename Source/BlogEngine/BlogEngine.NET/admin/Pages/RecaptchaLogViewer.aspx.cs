// --------------------------------------------------------------------------------------------------------------------
// <summary>
//   The admin pages recaptcha log viewer.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Admin.Pages
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Globalization;
    using System.Linq;
    using System.Threading;
    using System.Web.Security;

    using BlogEngine.Core;

    using Recaptcha;

    using Page = System.Web.UI.Page;

    /// <summary>
    /// The admin pages recaptcha log viewer.
    /// </summary>
    public partial class RecaptchaLogViewer : Page
    {
        #region Constants and Fields

        /// <summary>
        ///     The gravatar image.
        /// </summary>
        private const string GravatarImage = "<img class=\"photo\" src=\"{0}\" alt=\"{1}\" />";

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets the website.
        /// </summary>
        /// <param name="website">The website.</param>
        /// <returns>A website string.</returns>
        public static string GetWebsite(object website)
        {
            if (website == null)
            {
                return string.Empty;
            }

            const string Templ = "<a href='{0}' target='_new' rel='{0}'>{1}</a>";

            var site = website.ToString();
            site = site.Replace("http://www.", string.Empty);
            site = site.Replace("http://", string.Empty);
            site = site.Length < 20 ? site : string.Format("{0}...", site.Substring(0, 17));

            return string.Format(Templ, website, site);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gravatars the specified email.
        /// </summary>
        /// <param name="email">
        /// The email.
        /// </param>
        /// <param name="author">
        /// The author.
        /// </param>
        /// <returns>
        /// The gravatar.
        /// </returns>
        protected string Gravatar(string email, string author)
        {
            if (BlogSettings.Instance.Avatar == "none")
            {
                return null;
            }

            if (String.IsNullOrEmpty(email) || !email.Contains("@"))
            {
                return string.Format(
                    "<img src=\"{0}themes/{1}/noavatar.jpg\" alt=\"{2}\" width=\"28\" height=\"28\" />", 
                    Utils.AbsoluteWebRoot, 
                    BlogSettings.Instance.Theme, 
                    author);
            }

            var hash = FormsAuthentication.HashPasswordForStoringInConfigFile(email.ToLowerInvariant().Trim(), "MD5");
            if (hash != null)
            {
                hash = hash.ToLowerInvariant();
            }

            var gravatar = string.Format("http://www.gravatar.com/avatar/{0}.jpg?s=28&amp;d=", hash);

            string link;
            switch (BlogSettings.Instance.Avatar)
            {
                case "identicon":
                    link = string.Format("{0}identicon", gravatar);
                    break;

                case "wavatar":
                    link = string.Format("{0}wavatar", gravatar);
                    break;

                default:
                    link = string.Format("{0}monsterid", gravatar);
                    break;
            }

            return string.Format(CultureInfo.InvariantCulture, GravatarImage, link, author);
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init"/> event to initialize the page.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> that contains the event data.</param>
        protected override void OnInit(EventArgs e)
        {
            Security.DemandUserHasRight(BlogEngine.Core.Rights.AccessAdminPages, true);

            this.BindGrid();

            base.OnInit(e);
        }

        /// <summary>
        /// Binds the grid.
        /// </summary>
        private void BindGrid()
        {
            var log = RecaptchaLogger.ReadLogItems();

            var comments = Post.Posts.SelectMany(post => post.Comments).ToDictionary(comment => comment.Id);

            var logView = new DataTable("LogView");
            logView.Columns.Add("Email");
            logView.Columns.Add("Date", typeof(DateTime));
            logView.Columns.Add("Author");
            logView.Columns.Add("Website");
            logView.Columns.Add("IP");
            logView.Columns.Add("RecaptchaAttempts", typeof(ushort));
            logView.Columns.Add("CommentTime", typeof(double));
            logView.Columns.Add("RecaptchaTime", typeof(double));

            var orphanedRecords = new List<RecaptchaLogItem>();

            foreach (var item in log)
            {
                if (comments.ContainsKey(item.CommentId))
                {
                    var comment = comments[item.CommentId];
                    logView.Rows.Add(
                        comment.Email, 
                        comment.DateCreated, 
                        comment.Author, 
                        comment.Website, 
                        comment.IP, 
                        item.NumberOfAttempts, 
                        item.TimeToComment, 
                        item.TimeToSolveCapcha);
                }
                else
                {
                    orphanedRecords.Add(item);
                }
            }

            if (orphanedRecords.Count > 0)
            {
                foreach (var orphan in orphanedRecords)
                {
                    log.Remove(orphan);
                }

                RecaptchaLogger.SaveLogItems(log);
            }

            using (var view = new DataView(logView))
            {
                view.Sort = "Date DESC";
                this.RecaptchaLog.DataSource = view;
                this.RecaptchaLog.DataBind();
            }
        }

        #endregion
    }
}