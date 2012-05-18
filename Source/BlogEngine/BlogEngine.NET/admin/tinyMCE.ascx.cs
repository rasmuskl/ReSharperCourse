// --------------------------------------------------------------------------------------------------------------------
// <summary>
//   The admin tiny mce.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Admin
{
    using System.Web.UI;
    using System.Web.UI.WebControls;

    /// <summary>
    /// The admin tiny mce.
    /// </summary>
    public partial class TinyMce : UserControl
    {
        #region Properties

        /// <summary>
        /// Gets or sets Height.
        /// </summary>
        public Unit Height
        {
            get
            {
                return this.txtContent.Height;
            }

            set
            {
                this.txtContent.Height = value;
            }
        }

        /// <summary>
        /// Gets or sets TabIndex.
        /// </summary>
        public short TabIndex
        {
            get
            {
                return this.txtContent.TabIndex;
            }

            set
            {
                this.txtContent.TabIndex = value;
            }
        }

        /// <summary>
        /// Gets or sets Text.
        /// </summary>
        public string Text
        {
            get
            {
                return this.txtContent.Text;
            }

            set
            {
                this.txtContent.Text = value;
            }
        }

        /// <summary>
        /// Gets or sets Width.
        /// </summary>
        public Unit Width
        {
            get
            {
                return this.txtContent.Width;
            }

            set
            {
                this.txtContent.Width = value;
            }
        }

        #endregion
    }
}