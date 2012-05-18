namespace MichaelJBaird.Themes.JQMobile
{
  #region using
  using System;
  using System.Web;
  using System.Web.UI;
  using Raisr.BE;
  using BlogEngine.Core;
  #endregion

  public partial class Master : MasterPage
  {
    private readonly string themePath;

    #region Master
    /// <summary>
    /// Initializes a new instance of the <see cref="Master"/> class.
    /// </summary>
    public Master()
    {
      themePath = Utils.ApplicationRelativeWebRoot + "themes/" + BlogEngine.Core.BlogSettings.Instance.Theme;
    }
    #endregion

    public ThemeHelper.PageType CurrentPageType { get; set; }

    #region OnInit
    /// <summary>
    /// Raises the <see cref="E:System.Web.UI.Control.Init"/> event.
    /// </summary>
    /// <param name="e">An <see cref="T:System.EventArgs"/> object that contains the event data.</param>
    protected override void OnInit(EventArgs e)
    {
      CurrentPageType = ThemeHelper.GetCurrentPageType();
      //If an alternate page should be served it has to be done in this early state to avoid
      //unnecessary duplicate code execution.
      SwitchPage();
      base.OnInit(e);
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
      SwitchContentControl();
    }
    #endregion

    #region Render
    /// <summary>
    /// Sends server control content to a provided <see cref="T:System.Web.UI.HtmlTextWriter"/> object, which writes the content to be rendered on the client.
    /// </summary>
    /// <param name="writer">The <see cref="T:System.Web.UI.HtmlTextWriter"/> object that receives the server control content.</param>
    protected override void Render(HtmlTextWriter writer)
    {
      //ThemeHelper.AddStyleReference("styles.css");

      base.Render(writer);
    }
    #endregion

    #region SwitchContentControl
    /// <summary>
    /// Replaces the default post linsting control used in default.aspx by a custom control.
    /// </summary>
    private void SwitchContentControl()
    {
      switch (CurrentPageType)
      {
        case ThemeHelper.PageType.AllPosts:
          SetAlternatePostListControl("Controls/PostList.ascx");
          break;
        case ThemeHelper.PageType.PostsByApmlFilter:
        case ThemeHelper.PageType.PostsByAuthor:
        case ThemeHelper.PageType.PostsByCategory:
        case ThemeHelper.PageType.PostsByTag:
        case ThemeHelper.PageType.PostsByTimeRange:
          SetAlternatePostListControl("Controls/PostList.ascx");
          break;
      }
    }
    #endregion

    #region AddMainHeader
    /// <summary>
    /// Adds the main header.
    /// </summary>
    private void AddMainHeader()
    {
      var jqmHeader = this.Page.Master.FindControl("jqmHeader");
      var mainHeader = LoadControl("controls/MainHeader.ascx");
      jqmHeader.Controls.Add(mainHeader);
    } 
    #endregion

    #region SwitchPage
    /// <summary>
    /// Replaces a default be.net page with a custom one.
    /// </summary>
    private void SwitchPage()
    {
      switch (CurrentPageType)
      {
        case ThemeHelper.PageType.Post:
          SetAlternatePost("Post.aspx");
          break;
        case ThemeHelper.PageType.Archive:
          SetAlternateArchive("Archive.aspx");
          break;
        case ThemeHelper.PageType.Contact:
          SetAlternateContact("Contact.aspx");
          break;
        case ThemeHelper.PageType.Search:
          SetAlternateSearch("Search.aspx");
          break;
      }
    } 
    #endregion

    #region AddHeader
    /// <summary>
    /// Adds the header.
    /// </summary>
    /// <param name="title">The title.</param>
    private void AddHeader(string title)
    {
      var jqmHeader = this.Page.Master.FindControl("jqmHeader");
      dynamic header = LoadControl("controls/Header.ascx");
      header.Title = title;
      jqmHeader.Controls.Add(header);
    }
    #endregion

    #region SetAlternatePost
    /// <summary>
    /// Sets the alternate post page.
    /// </summary>
    /// <param name="alternatePost">The alternate page.</param>
    private void SetAlternatePost(string alternatePost)
    {
      var newPath = themePath + "/" + alternatePost;
      var currentPage = HttpContext.Current.CurrentHandler as System.Web.UI.Page;
      var path = Utils.ApplicationRelativeWebRoot + "post.aspx";
      if (currentPage.Request.CurrentExecutionFilePath.ToLower() == path)
        Server.TransferRequest(newPath, true);
    }
    #endregion

    #region SetAlternateArchive
    /// <summary>
    /// Sets the alternate page.
    /// </summary>
    /// <param name="alternatePost">The alternate page.</param>
    private void SetAlternateArchive(string alternate)
    {
      var newPath = themePath + "/" + alternate;
      var currentPage = HttpContext.Current.CurrentHandler as System.Web.UI.Page;
      var path = Utils.ApplicationRelativeWebRoot + "archive.aspx";
      if (currentPage.Request.CurrentExecutionFilePath.ToLower() == path)
        Server.TransferRequest(newPath, true);
    }
    #endregion

    #region SetAlternateContact
    /// <summary>
    /// Sets the alternate page.
    /// </summary>
    /// <param name="alternatePost">The alternate page.</param>
    private void SetAlternateContact(string alternate)
    {
      var newPath = themePath + "/" + alternate;
      var currentPage = HttpContext.Current.CurrentHandler as System.Web.UI.Page;
      var path = Utils.ApplicationRelativeWebRoot + "contact.aspx";
      if (currentPage.Request.CurrentExecutionFilePath.ToLower() == path)
        Server.TransferRequest(newPath, true);
    }
    #endregion

    #region SetAlternateSearch
    /// <summary>
    /// Sets the alternate page.
    /// </summary>
    /// <param name="alternatePost">The alternate page.</param>
    private void SetAlternateSearch(string alternate)
    {
      var newPath = themePath + "/" + alternate;
      var currentPage = HttpContext.Current.CurrentHandler as System.Web.UI.Page;
      var path = Utils.ApplicationRelativeWebRoot + "search.aspx";
      if (currentPage.Request.CurrentExecutionFilePath.ToLower() == path)
        Server.Transfer(newPath, true);
    }
    #endregion

    #region SetAlternatePostListControl
    /// <summary>
    /// Sets the alternate post list control.
    /// </summary>
    /// <param name="alternateControlRelativePath">The alternate control relative path.</param>
    private void SetAlternatePostListControl(string alternateControlRelativePath)
    {
      var alternatePostList = LoadControl(themePath + "/" + alternateControlRelativePath);
      if (alternatePostList == null) return;

      var postListControl = cphBody.FindControl("PostList1");

      // Get the values from the original control. So we don't need to reimplement the data retrieval
      CopyControlProperty(postListControl, alternatePostList, "Posts", "Posts");
      CopyControlProperty(postListControl, alternatePostList, "ContentBy", "ContentBy");

      SetControlProperty(alternatePostList, "CurrentPageType", CurrentPageType);

      // Switch over to the new control
      var postListIndex = cphBody.Controls.IndexOf(postListControl);
      cphBody.Controls.RemoveAt(postListIndex);
      cphBody.Controls.AddAt(postListIndex, alternatePostList);

      // Add new Pager to Footer
      dynamic newPager = LoadControl("Controls/Pager.ascx");
      CopyControlProperty(postListControl, newPager, "Posts", "Posts");
      jqmFooter.Controls.Add(newPager);
    }
    #endregion

    #region CopyControlProperty
    /// <summary>
    /// Copies the value of one control property to another control. 
    /// </summary>
    /// <param name="source">The source.</param>
    /// <param name="target">The target.</param>
    /// <param name="sourcePropertyName">Name of the source property.</param>
    /// <param name="targetPropertyName">Name of the target property.</param>
    public static void CopyControlProperty(Control source, Control target, string sourcePropertyName, string targetPropertyName)
    {
      if (source != null && target != null)
      {
        var sourceType = source.GetType();
        var sourceProperty = sourceType.GetProperty(sourcePropertyName);

        if (sourceProperty != null)
        {
          var sourceValue = sourceProperty.GetValue(source, null);
          SetControlProperty(target, targetPropertyName, sourceValue);
        }
      }
    }
    #endregion

    #region SetControlProperty
    /// <summary>
    /// Sets a property on a control dynamicly.
    /// </summary>
    /// <param name="control">The control.</param>
    /// <param name="propertyName">Name of the property.</param>
    /// <param name="value">The value.</param>
    public static void SetControlProperty(Control control, string propertyName, object value)
    {
      if (control != null)
      {
        var property = control.GetType().GetProperty(propertyName);
        if (property != null)
        {
          property.SetValue(control, value, null);
        }
      }
    }
    #endregion
  }
}
