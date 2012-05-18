// --------------------------------------------------------------------------------------------------------------------
// <summary>
//   The widget.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Widgets.AuthorList
{
	using App_Code.Controls;

	/// <summary>
	/// The widget.
	/// </summary>
	public partial class Widget : WidgetBase
	{
		#region Properties

		/// <summary>
		/// Gets a value indicating whether the Widget is editable.
		/// </summary>
		public override bool IsEditable
		{
			get { return false; }
		}

		/// <summary>
		/// Gets the Widget name.
		/// </summary>
		public override string Name
		{
			get { return "AuthorList"; }
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// This method works as a substitute for Page_Load. You should use this method for
		/// data binding etc. instead of Page_Load.
		/// </summary>
		public override void LoadWidget()
		{
			// nothing to load
		}

		#endregion
	}
}