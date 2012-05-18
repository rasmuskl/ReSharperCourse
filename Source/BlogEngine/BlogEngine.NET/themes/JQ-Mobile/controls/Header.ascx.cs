namespace MichaelJBaird.Themes.JQMobile.Controls
{
  #region using
  using System;
  using System.Web.UI;
  #endregion

  public partial class Header : UserControl
  {
    #region Page_Load
    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    protected void Page_Load(object sender, EventArgs e)
    {
    } 
    #endregion

    public string Title { get; set; }
  }
}