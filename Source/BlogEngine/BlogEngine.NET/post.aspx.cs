#region Using

using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using BlogEngine.Core;
using BlogEngine.Core.Web.Controls;
using System.Collections.Generic;

#endregion

public partial class post : BlogEngine.Core.Web.Controls.BlogBasePage
{

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        if (!Security.IsAuthorizedTo(Rights.ViewPublicPosts))
        {
            Response.Redirect(Utils.RelativeWebRoot);
        }

        bool shouldThrow404 = false;


        CommentView1.Visible = ShowCommentsForm;
        disqus_box.Visible = ShowDisqusForm;

        var requestId = Request.QueryString["id"];
        Guid id;

        if ((!Utils.StringIsNullOrWhitespace(requestId)) && requestId.TryParse(out id))
        {

            Post post = Post.GetPost(id);

            if (post != null)
            {
                if (!Page.IsPostBack && !Page.IsCallback && Request.RawUrl.Contains("?id="))
                {
                    // If there's more than one post that has the same RelativeLink
                    // this post has then don't do a 301 redirect.

                    if (Post.Posts.FindAll(delegate(Post p)
                    { return p.RelativeLink.Equals(post.RelativeLink); }
                    ).Count < 2)
                    {
                        Response.Clear();
                        Response.StatusCode = 301;
                        Response.AppendHeader("location", post.RelativeLink.ToString());
                        Response.End();
                    }
                }
                else if (!post.IsVisible)
                {
                    Response.Redirect(Utils.RelativeWebRoot + "default.aspx", true);
                    //shouldThrow404 = true;
                }
                else
                {
                    this.Post = post;

                    var settings = BlogSettings.Instance;
                    string encodedPostTitle = Server.HtmlEncode(Post.Title);
                    string path = Utils.ApplicationRelativeWebRoot + "themes/" + BlogSettings.Instance.GetThemeWithAdjustments(null) + "/PostView.ascx";

                    PostViewBase postView = (PostViewBase)LoadControl(path);
                    postView.Post = Post;
                    postView.ID = Post.Id.ToString().Replace("-", string.Empty);
                    postView.Location = ServingLocation.SinglePost;
                    pwPost.Controls.Add(postView);

                    if (settings.EnableRelatedPosts)
                    {
                        related.Visible = true;
                        related.Item = this.Post;
                    }

                    CommentView1.Post = Post;

                    Page.Title = encodedPostTitle;
                    AddMetaKeywords();
                    AddMetaDescription();
                    base.AddMetaTag("author", Server.HtmlEncode(Post.AuthorProfile == null ? Post.Author : Post.AuthorProfile.FullName));

                    List<Post> visiblePosts = Post.Posts.FindAll(delegate(Post p) { return p.IsVisible; });
                    if (visiblePosts.Count > 0)
                    {
                        AddGenericLink("last", visiblePosts[0].Title, visiblePosts[0].RelativeLink);
                        AddGenericLink("first", visiblePosts[visiblePosts.Count - 1].Title, visiblePosts[visiblePosts.Count - 1].RelativeLink);
                    }

                    InitNavigationLinks();

                    phRDF.Visible = settings.EnableTrackBackReceive;

                    base.AddGenericLink("application/rss+xml", "alternate", encodedPostTitle + " (RSS)", postView.CommentFeed + "?format=ATOM");
                    base.AddGenericLink("application/rss+xml", "alternate", encodedPostTitle + " (ATOM)", postView.CommentFeed + "?format=ATOM");

                    if (BlogSettings.Instance.EnablePingBackReceive)
                    {
                        Response.AppendHeader("x-pingback", "http://" + Request.Url.Authority + Utils.RelativeWebRoot + "pingback.axd");
                    }

                    string commentNotificationUnsubscribeEmailAddress = Request.QueryString["unsubscribe-email"];
                    if (!string.IsNullOrEmpty(commentNotificationUnsubscribeEmailAddress))
                    {
                        if (Post.NotificationEmails.Contains(commentNotificationUnsubscribeEmailAddress))
                        {
                            Post.NotificationEmails.Remove(commentNotificationUnsubscribeEmailAddress);
                            Post.Save();
                            phCommentNotificationUnsubscription.Visible = true;
                        }
                    }
                }

            }

        }

        else
        {
            shouldThrow404 = true;
        }

        if (shouldThrow404)
        {
            Response.Redirect(Utils.RelativeWebRoot + "error404.aspx", true);
        }

    }

	/// <summary>
	/// Gets the next post filtered for invisible posts.
	/// </summary>
	private Post GetNextPost(Post post)
	{
		if (post.Next == null)
			return null;

		if (post.Next.IsVisible)
			return post.Next;

		return GetNextPost(post.Next);
	}

	/// <summary>
	/// Gets the prev post filtered for invisible posts.
	/// </summary>
	private Post GetPrevPost(Post post)
	{
		if (post.Previous == null)
			return null;

		if (post.Previous.IsVisible)
			return post.Previous;

		return GetPrevPost(post.Previous);
	}

	/// <summary>
	/// Inits the navigation links above the post and in the HTML head section.
	/// </summary>
	private void InitNavigationLinks()
	{
		if (BlogSettings.Instance.ShowPostNavigation)
		{
			Post next = GetNextPost(Post);
			Post prev = GetPrevPost(Post);

			if (next != null && !next.Deleted)
			{
				hlNext.NavigateUrl = next.RelativeLink;
				hlNext.Text = Server.HtmlEncode(next.Title + " >>");
				hlNext.ToolTip = Resources.labels.nextPost;
				base.AddGenericLink("next", next.Title, next.RelativeLink);
				phPostNavigation.Visible = true;
			}

			if (prev != null && !prev.Deleted)
			{
				hlPrev.NavigateUrl = prev.RelativeLink;
				hlPrev.Text = Server.HtmlEncode("<< " + prev.Title);
				hlPrev.ToolTip = Resources.labels.previousPost;
				base.AddGenericLink("prev", prev.Title, prev.RelativeLink);
				phPostNavigation.Visible = true;
			}
		}
	}

	/// <summary>
	/// Adds the post's description as the description metatag.
	/// </summary>
	private void AddMetaDescription()
	{
		base.AddMetaTag("description", Server.HtmlEncode(Post.Description));
	}

	/// <summary>
	/// Adds the post's tags as meta keywords.
	/// </summary>
	private void AddMetaKeywords()
	{
        if (Post.Tags.Count > 0)
		{
            base.AddMetaTag("keywords", Server.HtmlEncode(string.Join(",", Post.Tags.ToArray())));
		}
	}

	public Post Post;

    public static bool ShowCommentsForm 
    {
        get
        {
            return BlogSettings.Instance.ModerationType != BlogSettings.Moderation.Disqus;
        }
    }

    public static bool ShowDisqusForm
    {
        get
        {
            return BlogSettings.Instance.ModerationType == BlogSettings.Moderation.Disqus;
        }
    }
}
