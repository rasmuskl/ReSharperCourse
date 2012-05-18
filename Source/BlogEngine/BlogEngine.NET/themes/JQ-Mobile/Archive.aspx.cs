namespace MichaelJBaird.Themes.JQMobile
{
  #region using
  using System;
  using System.Web;
  using System.Collections.Generic;
  using System.Web.UI;
  using System.Web.UI.WebControls;
  using System.Web.UI.HtmlControls;
  using BlogEngine.Core;
  using System.Text;
  using Resources;
  #endregion
  
  public partial class Archive : BlogEngine.Core.Web.Controls.BlogBasePage
  {
    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    protected void Page_Load(object sender, EventArgs e)
    {
      if (!IsPostBack && !IsCallback)
      {
        AddHeader();
        CreateArchive();
        AddTotals();
      }

      Page.Title = Server.HtmlEncode(Resources.labels.archive);
      base.AddMetaTag("description", Resources.labels.archive + " | " + BlogSettings.Instance.Name);
    } 

    /// <summary>
    /// Adds the header.
    /// </summary>
    private void AddHeader()
    {
      var jqmHeader = this.Page.Master.FindControl("jqmHeader");
      var path = Utils.ApplicationRelativeWebRoot + "themes/JQ-Mobile/Controls/Header.ascx";
      dynamic header = LoadControl(path);
      header.Title = labels.archive;
      jqmHeader.Controls.Add(header);
    }

    /// <summary>
    /// Sorts the categories.
    /// </summary>
    /// <param name="categories">The categories.</param>
    private SortedDictionary<string, Guid> SortCategories(Dictionary<Guid, string> categories)
    {
      SortedDictionary<string, Guid> dic = new SortedDictionary<string, Guid>();
      foreach (Category cat in Category.Categories)
      {
        bool postsExist = cat.Posts.FindAll(delegate(Post post)
        {
          return post.IsVisible;
        }).Count > 0;

        if (postsExist)
          dic.Add(cat.Title, cat.Id);
      }

      return dic;
    } 

    /// <summary>
    /// Creates the archive.
    /// </summary>
    private void CreateArchive()
    {
      foreach (Category cat in Category.Categories)
      {
        string name = cat.Title;
        List<Post> list = cat.Posts.FindAll(delegate(Post p) { return p.IsVisible; });

        HtmlGenericControl divider = new HtmlGenericControl("li");
        divider.Attributes.Add("data-role", "list-divider");
        divider.InnerHtml = string.Format("{0} <span class=\"ui-li-count\">{1}</span>", name, list.Count);
        ulArchive.Controls.Add(divider);


        foreach (Post post in list)
        {
          HtmlGenericControl postItem = new HtmlGenericControl("li");
          CreatePostRow(post, ref postItem);

          ulArchive.Controls.Add(postItem);
        }
      }
    } 

    /// <summary>
    /// Creates the post row.
    /// </summary>
    /// <param name="post">The post.</param>
    /// <param name="postItem">The post item.</param>
    private static void CreatePostRow(Post post, ref HtmlGenericControl postItem)
    {
      StringBuilder postInnerHtml = new StringBuilder();
      var authorName = post.AuthorProfile != null ? post.AuthorProfile.DisplayName : post.Author;
      postInnerHtml.AppendFormat("<a href=\"{0}\"><h3>{1}</h3><p>by <strong>{2}</strong></p><p><em>{3}</em></p>", post.RelativeLink, post.Title, authorName, post.DateCreated.ToShortDateString());

      if (BlogSettings.Instance.IsCommentsEnabled)
      {
        if (BlogSettings.Instance.ModerationType == BlogSettings.Moderation.Disqus)
          postInnerHtml.AppendFormat("<span class=\"ui-li-count\"><a href=\"{0}#disqus_thread\">{1}</a></span>", post.PermaLink, Resources.labels.comments);
        else
          postInnerHtml.AppendFormat("<span class=\"ui-li-count\">{0}</span>", post.ApprovedComments.Count.ToString());
      }

      postInnerHtml.Append("</a>");
      postItem.InnerHtml = postInnerHtml.ToString();
    } 

    /// <summary>
    /// Adds the totals.
    /// </summary>
    private void AddTotals()
    {
      int comments = 0;
      int raters = 0;
      List<Post> posts = Post.Posts.FindAll(delegate(Post p) { return p.IsVisible; });
      foreach (Post post in posts)
      {
        comments += post.ApprovedComments.Count;
        raters += post.Raters;
      }

      ltPosts.Text = posts.Count + " " + Resources.labels.posts.ToLowerInvariant();
      if (BlogSettings.Instance.IsCommentsEnabled && BlogSettings.Instance.ModerationType != BlogSettings.Moderation.Disqus)
        ltComments.Text = "<span>" + comments + " " + Resources.labels.comments.ToLowerInvariant() + "</span><br />";

      if (BlogSettings.Instance.EnableRating)
        ltRaters.Text = raters + " " + Resources.labels.raters.ToLowerInvariant();
    } 
  }
}
