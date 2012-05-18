namespace MichaelJBaird.Themes.JQMobile.Controls
{
  #region using
  using System;
  using System.Collections.Generic;
  using BlogEngine.Core;
  using BlogEngine.Core.Web.Controls;
  #endregion

  public partial class PostList : System.Web.UI.UserControl
  {
    #region Constants and Fields

    /// <summary>
    ///     The posts.
    /// </summary>
    private List<IPublishable> publishables;

    /// <summary>
    ///     Initializes a new instance of the <see cref = "PostList" /> class.
    /// </summary>
    public PostList()
    {
      this.ContentBy = ServingContentBy.AllContent;
    }

    #endregion

    #region Properties

    /// <summary>
    ///     Gets or sets the criteria by which the content is being served (by tag, category, author, etc).
    /// </summary>
    public ServingContentBy ContentBy { get; set; }

    /// <summary>
    ///     Gets or sets the list of posts to display.
    /// </summary>
    public List<IPublishable> Posts
    {
      get
      {
        return this.publishables;
      }

      set
      {
        this.publishables = value;
      }
    }

    #endregion

    /// <summary>
    /// Raises the <see cref="E:System.Web.UI.Control.Init"/> event.
    /// </summary>
    /// <param name="e">An <see cref="T:System.EventArgs"/> object that contains the event data.</param>
    protected override void OnInit(EventArgs e)
    {
      base.OnInit(e);

      if (!Security.IsAuthorizedTo(Rights.ViewPublicPosts))
      {
        this.Visible = false;
      }
    }

    /// <summary>
    /// Raises the <see cref="E:System.Web.UI.Control.Load"/> event.
    /// </summary>
    /// <param name="e">
    /// The <see cref="T:System.EventArgs"/> object that contains the event data.
    /// </param>
    protected override void OnLoad(EventArgs e)
    {
      base.OnLoad(e);

      if (this.Page.IsCallback)
      {
        return;
      }

      this.BindPosts();

      // Add MainHeader
      AddMainHeader();
    }

    /// <summary>
    /// Adds the main header.
    /// </summary>
    private void AddMainHeader()
    {
      var jqmHeader = this.Page.Master.FindControl("jqmHeader");
      var mainHeader = LoadControl("MainHeader.ascx");
      jqmHeader.Controls.Add(mainHeader);
    } 

		/// <summary>
    /// Binds the list of posts to individual postview.ascx controls
    ///     from the current theme.
    /// </summary>
    private void BindPosts()
    {
      if (this.Posts == null)
      {
        // no posts provided, load all posts by default
        Posts = Post.ApplicablePosts.ConvertAll(new Converter<Post, IPublishable>(delegate(Post p) { return p as IPublishable; }));
      }

      if (this.Posts.Count == 0)
      {
        return;
      }

      //var visiblePosts = Post.Posts.FindAll(p => p.IsVisibleToPublic);
      var visiblePosts = this.Posts.FindAll(p => p.IsVisible);

      BindPager(visiblePosts);

      var count = Math.Min(BlogSettings.Instance.PostsPerPage, visiblePosts.Count);
      var page = this.GetPageIndex();
      var index = page * count;
      var stop = count;
      if (index + count > visiblePosts.Count)
      {
        stop = visiblePosts.Count - index;
      }

      if (stop < 0 || stop + index > visiblePosts.Count)
      {
        return;
      }

      var path = string.Format("{0}themes/{1}/PostView.ascx", Utils.ApplicationRelativeWebRoot, BlogSettings.Instance.GetThemeWithAdjustments(this.Request.QueryString["theme"]));
      var counter = 0;

      foreach (Post post in visiblePosts.GetRange(index, stop))
      {
        if (counter == stop)
        {
          break;
        }

        var postView = (PostViewBase)this.LoadControl(path);
        postView.ShowExcerpt = ShowExcerpt();
        postView.Post = post;
        postView.ID = post.Id.ToString().Replace("-", string.Empty);
        postView.Location = ServingLocation.PostList;
        postView.Index = counter;
        this.posts.Controls.Add(postView);
        counter++;
      }
    } 

		/// <summary>
    /// Retrieves the current page index based on the QueryString.
    /// </summary>
    /// <returns>
    /// The get page index.
    /// </returns>
    private int GetPageIndex()
    {
      int index;
      if (int.TryParse(this.Request.QueryString["page"], out index))
      {
        index--;
      }

      return index;
    } 

    /// <summary>
    /// Whether or not to show the entire post or just the excerpt/description
    /// in the post list 
    /// </summary>
    /// <returns></returns>
    private bool ShowExcerpt()
    {
      string url = this.Request.RawUrl.ToUpperInvariant();
      bool tagOrCategory = url.Contains("/CATEGORY/") || url.Contains("?TAG=/");

      return BlogSettings.Instance.ShowDescriptionInPostList ||
          (BlogSettings.Instance.ShowDescriptionInPostListForPostsByTagOrCategory && tagOrCategory);
    }

    private void BindPager(List<IPublishable> posts)
    {
      dynamic jqmPager = LoadControl("Pager.ascx");
      jqmPager.Posts = posts;
      var jqmFooter = this.Page.Master.FindControl("jqmFooter");
      jqmFooter.Controls.Add(jqmPager);
    }
  }
}