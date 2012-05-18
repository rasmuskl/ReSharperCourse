// --------------------------------------------------------------------------------------------------------------------
// <summary>
//   The admin_html editor.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Admin
{
    using System.Web.UI;

    /// <summary>
    /// The admin_html editor.
    /// </summary>
    public partial class HtmlEditor : UserControl
    {
        #region Properties

        /// <summary>
        /// Gets or sets TabIndex.
        /// </summary>
        public short TabIndex
        {
            get
            {
                return this.TinyMCE1.TabIndex;
            }

            set
            {
                this.TinyMCE1.TabIndex = value;
            }
        }

        /// <summary>
        /// Gets or sets Text.
        /// </summary>
        public string Text
        {
            get
            {
                return this.TinyMCE1.Text;
            }

            set
            {
                this.TinyMCE1.Text = value;
            }
        }

        #endregion
    }
}