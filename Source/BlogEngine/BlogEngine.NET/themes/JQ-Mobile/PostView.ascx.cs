namespace MichaelJBaird.Themes.JQMobile
{
  #region using
  using System;
  using BlogEngine.Core;
  using BlogEngine.Core.Web.Controls;
  #endregion

  public partial class PostView : PostViewBase
  {
    public bool IsCustomInitialize { get; set; }

    #region OnInit
    /// <summary>
    /// Raises the <see cref="E:System.Web.UI.Control.Init"/> event.
    /// </summary>
    /// <param name="e">An <see cref="T:System.EventArgs"/> object that contains the event data.</param>
    protected override void OnInit(System.EventArgs e)
    {
      base.OnInit(e);
      if (IsCustomInitialize)
      {
        OnLoad(e);
      }
    } 
    #endregion

    #region Page_Load
    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    protected void Page_Load(object sender, EventArgs e)
    {
      if (this.Location == ServingLocation.SinglePost)
      {
        // Add Post Header
        AddPostHeader();
      }
    } 
    #endregion

    #region AddPostHeader
    /// <summary>
    /// Adds the post header.
    /// </summary>
    private void AddPostHeader()
    {
      var header = this.Page.Master.FindControl("jqmHeader");
      dynamic postHeader = LoadControl("~/themes/JQ-Mobile/controls/Header.ascx");
      postHeader.Title = this.Post.Title;
      header.Controls.Add(postHeader);
    } 
    #endregion
  }
}