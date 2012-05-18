// --------------------------------------------------------------------------------------------------------------------
// <summary>
//   The admin pages pages.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Admin.Pages
{
    using System;
    using Resources;

    using Page = System.Web.UI.Page;
    using App_Code;

    /// <summary>
    /// The admin pages pages.
    /// </summary>
    public partial class PagesPage : Page
    {
        #region Methods

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init"/> event to initialize the page.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> that contains the event data.</param>
        protected override void OnInit(EventArgs e)
        {
            WebUtils.CheckRightsForAdminPagesPages(false);
            MaintainScrollPositionOnPostBack = true;
          
            Page.Title = labels.pages;

            base.OnInit(e);
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Load"/> event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.EventArgs"/> object that contains the event data.</param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
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

        #endregion
    }
}