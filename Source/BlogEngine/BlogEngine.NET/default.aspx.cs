#region Using

using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml;
using System.Collections.Generic;
using BlogEngine.Core;
using System.Web.UI;

#endregion

public partial class _default : BlogEngine.Core.Web.Controls.BlogBasePage
{
	protected void Page_Load(object sender, EventArgs e)
	{
		if (Page.IsCallback)
			return;
		
		if (Request.RawUrl.ToLowerInvariant().Contains("/category/"))
		{
			DisplayCategories();
		}
		else if (Request.RawUrl.ToLowerInvariant().Contains("/author/"))
		{
			DisplayAuthors();
		}
		else if (Request.RawUrl.ToLowerInvariant().Contains("?tag="))
		{
			DisplayTags();
		}
		else if (Request.QueryString["year"] != null || Request.QueryString["date"] != null || Request.QueryString["calendar"] != null)
		{
			if (Request.RawUrl.Contains("year="))
				Redirect();
			else
				DisplayDateRange();
		}
		else if (Request.QueryString["apml"] != null)
		{
			DisplayApmlFiltering();
		}
		else
		{			
			if (!BlogSettings.Instance.UseBlogNameInPageTitles)
				Page.Title = BlogSettings.Instance.Name + " | ";

			if (!string.IsNullOrEmpty(BlogSettings.Instance.Description))
				Page.Title += Server.HtmlEncode(BlogSettings.Instance.Description);
		}

		AddMetaKeywords();
		base.AddMetaTag("description", Server.HtmlEncode(BlogSettings.Instance.Description));
		base.AddMetaTag("author", Server.HtmlEncode(BlogSettings.Instance.AuthorName));		
	}

	private void DisplayApmlFiltering()
	{
		Uri url = null;
		if (Uri.TryCreate(Request.QueryString["apml"], UriKind.Absolute, out url))
		{
			Page.Title = "APML filtered list";
			try
			{
				Dictionary<Uri, XmlDocument> docs = Utils.FindSemanticDocuments(url, "apml");
				if (docs.Count > 0)
				{
					foreach (Uri key in docs.Keys)
					{
                        PostList1.ContentBy = ServingContentBy.Apml;
						PostList1.Posts = Search.ApmlMatches(docs[key], 30).FindAll(delegate(IPublishable p) { return p is Post; });
						PostList1.Posts.Sort(delegate(IPublishable ip1, IPublishable ip2) { return ip2.DateCreated.CompareTo(ip1.DateCreated); });
						Page.Title += " for " + Server.HtmlEncode(key.Host);
						break;
					}
				}
				else
				{
					divError.InnerHtml = "<h1 style=\"text-align:center\">APML document could not be found</h1><br /><br />";
					Page.Title = "APML not found";
				}
			}
			catch (NotSupportedException)
			{
				divError.InnerHtml = "<h1 style=\"text-align:center\">The website entered doesn't contain any information about APML</h1><br /><br />";
				Page.Title = "APML not found";
			}
			catch (System.Net.WebException)
			{
				divError.InnerHtml = "<h1 style=\"text-align:center\">Sorry, I couldn't connect to your website</h1><br /><br />";
				Page.Title = "APML address invalid";
			}
			catch (XmlException)
			{
				divError.InnerHtml = "<h1 style=\"text-align:center\">The APML document is not valid XML</h1><br /><br />";
				Page.Title = "APML document error";
			}
		}
		else if (PostList1.Posts == null || PostList1.Posts.Count == 0)
		{
			divError.InnerHtml = "<h1 style=\"text-align:center\">The URL could not be resolved</h1><br /><br />";
			Page.Title = "APML not found";
		}
	}
	
	/// <summary>
	/// Permanently redirects to the correct URL format if the page is requested with
	/// the old URL: /default.aspx?year=2007&month=12
	/// <remarks>
	/// The redirection is important so that we don't end up having 2 URLs 
	/// to the same resource. It's for SEO purposes.
	/// </remarks>
	/// </summary>
	private void Redirect()
	{
		string year = Request.QueryString["year"];
		string month = Request.QueryString["month"];
		string date = Request.QueryString["date"];
		string page = string.IsNullOrEmpty(Request.QueryString["page"]) ? string.Empty : "?page=" + Request.QueryString["page"];
		string rewrite = null;

		if (!string.IsNullOrEmpty(date))
		{
			DateTime dateParsed = DateTime.Parse(date);
			rewrite = Utils.RelativeWebRoot + dateParsed.Year + "/" + dateParsed.Month + "/" + dateParsed.Day + "/default.aspx";
		}
		else if (!string.IsNullOrEmpty(year) && !string.IsNullOrEmpty(month))
		{
			rewrite = Utils.RelativeWebRoot + year + "/" + month + "/default.aspx"; 
		}
		else if (!string.IsNullOrEmpty(year))
		{
			rewrite = Utils.RelativeWebRoot + year + "/default.aspx";
		}

		if (rewrite != null)
		{
			Response.Clear();
			Response.StatusCode = 301;
			Response.AppendHeader("location", rewrite + page);
			Response.End();
		}
	}

	private static readonly Regex YEAR_MONTH = new Regex("/([0-9][0-9][0-9][0-9])/([0-1][0-9])", RegexOptions.IgnoreCase | RegexOptions.Compiled);
	private static readonly Regex YEAR_MONTH_DAY = new Regex("/([0-9][0-9][0-9][0-9])/([0-1][0-9])/([0-3][0-9])", RegexOptions.IgnoreCase | RegexOptions.Compiled);

	/// <summary>
	/// Adds the post's tags as meta keywords.
	/// </summary>
	private void AddMetaKeywords()
	{
		if (Category.Categories.Count > 0)
		{
			string[] categories = new string[Category.Categories.Count];
			for (int i = 0; i < Category.Categories.Count; i++)
			{
				categories[i] = Category.Categories[i].Title;
			}

			string metakeywords = Server.HtmlEncode(string.Join(",", categories));
			System.Web.UI.HtmlControls.HtmlMeta tag = null;
			foreach (Control c in Page.Header.Controls)
			{
				if (c is System.Web.UI.HtmlControls.HtmlMeta && (c as System.Web.UI.HtmlControls.HtmlMeta).Name.ToLower() == "keywords")
				{
					tag = c as System.Web.UI.HtmlControls.HtmlMeta;
					tag.Content += ", " + metakeywords;
					break;
				}
			}
			if (tag == null)
			{
				base.AddMetaTag("keywords", metakeywords);
			} 
			//base.AddMetaTag("keywords", Server.HtmlEncode(string.Join(",", categories)));
		}
	}

	private void DisplayCategories()
	{
		if (!String.IsNullOrEmpty(Request.QueryString["id"]))
		{
			Guid categoryId = new Guid(Request.QueryString["id"]);
            PostList1.ContentBy = ServingContentBy.Category;
			PostList1.Posts = Post.GetPostsByCategory(categoryId).ConvertAll(new Converter<Post, IPublishable>(delegate(Post p) { return p as IPublishable; }));
			Page.Title = Category.GetCategory(categoryId).Title;
		}
	}

	private void DisplayAuthors()
	{
		if (!string.IsNullOrEmpty(Request.QueryString["name"]))
		{
			string author = Server.UrlDecode(Request.QueryString["name"]);
            PostList1.ContentBy = ServingContentBy.Author;
			PostList1.Posts = Post.GetPostsByAuthor(author).ConvertAll(new Converter<Post, IPublishable>(delegate(Post p) { return p as IPublishable; }));
			Title = "All posts by " + Server.HtmlEncode(author);
		}
	}

	private void DisplayTags()
	{
		if (!string.IsNullOrEmpty(Request.QueryString["tag"]))
		{
            PostList1.ContentBy = ServingContentBy.Tag;
			PostList1.Posts = Post.GetPostsByTag(Request.QueryString["tag"].Substring(1)).ConvertAll(new Converter<Post, IPublishable>(delegate(Post p) { return p as IPublishable; }));
			base.Title = " All posts tagged '" + Request.QueryString["tag"].Substring(1) + "'";
			//base.AddMetaTag("description", Server.HtmlEncode(BlogSettings.Instance.Description));
		}
	}

	private void DisplayDateRange()
	{
		string year = Request.QueryString["year"];
		string month = Request.QueryString["month"];
		string specificDate = Request.QueryString["date"];

		if (!string.IsNullOrEmpty(year) && !string.IsNullOrEmpty(month))
		{
			DateTime dateFrom = DateTime.Parse(year + "-" + month + "-01", CultureInfo.InvariantCulture);
			DateTime dateTo = dateFrom.AddMonths(1).AddMilliseconds(-1);
            PostList1.ContentBy = ServingContentBy.DateRange;
			PostList1.Posts = Post.GetPostsByDate(dateFrom, dateTo).ConvertAll(new Converter<Post, IPublishable>(delegate(Post p) { return p as IPublishable; }));
			Title = dateFrom.ToString("MMMM yyyy");
		}
		else if (!string.IsNullOrEmpty(year))
		{
			DateTime dateFrom = DateTime.Parse(year + "-01-01", CultureInfo.InvariantCulture);
			DateTime dateTo = dateFrom.AddYears(1).AddMilliseconds(-1);
            PostList1.ContentBy = ServingContentBy.DateRange;
			PostList1.Posts = Post.GetPostsByDate(dateFrom, dateTo).ConvertAll(new Converter<Post, IPublishable>(delegate(Post p) { return p as IPublishable; })); ;
			Title = dateFrom.ToString("yyyy");
		}
		else if (!string.IsNullOrEmpty(specificDate) && specificDate.Length == 10)
		{
			DateTime date = DateTime.Parse(specificDate, CultureInfo.InvariantCulture);
            PostList1.ContentBy = ServingContentBy.DateRange;
			PostList1.Posts = Post.GetPostsByDate(date, date).ConvertAll(new Converter<Post, IPublishable>(delegate(Post p) { return p as IPublishable; })); ;
			Title = date.ToString("MMMM d. yyyy");
		}
		else if (!string.IsNullOrEmpty(Request.QueryString["calendar"]))
		{
			calendar.Visible = true;
			PostList1.Visible = false;
			Title = Server.HtmlEncode(Resources.labels.calendar);
		}
	}
}
