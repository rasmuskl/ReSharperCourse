namespace Admin.Comments
{
    using System;
    using System.Web.UI;
    using BlogEngine.Core;

    /// <summary>
    /// The admin comments menu.
    /// </summary>
    public partial class Menu : UserControl
    {
        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> object that contains the event data.</param>
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            SetCounters();
        }

        /// <summary>
        /// Comment counter
        /// </summary>
        protected int CommentCount { get; set; }
        /// <summary>
        /// Pingback/trackback counter
        /// </summary>
        protected int PingbackCount { get; set; }
        /// <summary>
        /// Spam counter
        /// </summary>
        protected int SpamCount { get; set; }
        /// <summary>
        /// Pending approval
        /// </summary>
        protected int PendingCount { get; set; }

        /// <summary>
        /// Indicate that menu item selected
        /// </summary>
        /// <param name="pg">Page address</param>
        /// <returns>CSS class to append for current menu item</returns>
        protected string Current(string pg)
        {
            if (Request.Path.ToLower().Contains(pg.ToLower()))
            {
                return "class=\"content-box-selected\"";
            }
            return "";
        }

        /// <summary>
        /// Gets the cookie with visitor information if any is set.
        ///     Then fills the contact information fields in the form.
        /// </summary>
        private void SetCounters()
        {
            foreach (Post p in Post.Posts)
            {
                PendingCount += p.NotApprovedComments.Count;
                PingbackCount += p.Pingbacks.Count;
                CommentCount += p.ApprovedComments.Count;
                SpamCount += p.SpamComments.Count;
            }
        }

    }
}
