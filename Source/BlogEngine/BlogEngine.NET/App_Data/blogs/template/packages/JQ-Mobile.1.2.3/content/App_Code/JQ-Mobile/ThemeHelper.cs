using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI.HtmlControls;
using BlogEngine.Core;

namespace Raisr.BE
{
  /// <summary>
  /// Helper / Utility methods that can be helpful for creatig themes, extensions, controls and widgets for blogengine.net. 
  /// 
  /// For more information visit: http://blog.raisr.net/page/Util-ThemeHelper.aspx
  /// </summary>
  public class ThemeHelper
  {
    #region Declarations / Fields

    private static readonly object lockObject = new object();
    private const string SCRIPT_SOURCE_TEMPLATE = "{0}js.axd?path={1}themes/{2}/{3}"; // header script reference template
    private const string STYLESHEET_SOURCE_TEMPLATE = "{0}themes/{1}/{2}"; //header style reference template

    /// <summary>
    /// The content type of the a served page.
    /// </summary>
    public enum PageType
    {
      Unknown,
      AllPosts,
      PostsByCategory,
      PostsByTag,
      PostsByAuthor,
      PostsByTimeRange,
      PostsByApmlFilter,
      Page,
      Archive,
      Contact,
      Search,
      CustomPage,
      Post
    }

    #endregion

    #region Script / Css / html helpers

    /// <summary>
    /// Adds a javascript reference to the head section of the current page.
    /// </summary>
    /// <param name="scriptFile">The javascript file including the relative folder within the theme.</param>
    public static void AddScriptReference(string scriptFile)
    {
      if (CurrentPage != null)
      {
        var scriptTag = new HtmlGenericControl("script");
        scriptTag.Attributes.Add("src", string.Format(SCRIPT_SOURCE_TEMPLATE, CurrentPage.ResolveUrl("~/"), CurrentPage.ResolveUrl("~/"), BlogSettings.Instance.Theme, scriptFile));
        scriptTag.Attributes.Add("type", "text/javascript");
        CurrentPage.Header.Controls.Add(scriptTag);
      }
    }

    /// <summary>
    /// Adds a css stylesheet reference to the head section of the current page.
    /// </summary>
    /// <param name="cssFile">The CSS file.</param>
    public static void AddStyleReference(string cssFile)
    {
      if (CurrentPage != null)
      {
        var linkTag = new HtmlLink
        {
          Href =
              string.Format(STYLESHEET_SOURCE_TEMPLATE, CurrentPage.ResolveUrl("~/"),
                            BlogSettings.Instance.Theme, cssFile)
        };
        linkTag.Attributes.Add("type", "text/css");
        linkTag.Attributes.Add("rel", "stylesheet");

        CurrentPage.Header.Controls.Add(linkTag);
      }
    }

    /// <summary>
    /// Gets the value of an attribute on a html element.
    /// </summary>
    /// <param name="attributeName">Name of the attribute.</param>
    /// <param name="input">The input.</param>
    /// <returns></returns>
    public static string GetAttributeValue(string attributeName, string input)
    {
      string pattern = attributeName + "=[\'|\"](?<value>.*?)[\'|\"]";
      string value = string.Empty;

      var attribureRegeex = new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.Multiline);
      var attributeMatch = attribureRegeex.Match(input);

      if (attributeMatch.Success)
      {
        value = attributeMatch.Groups["value"].Value;
      }

      return value;
    }

    #endregion

    #region Meta data

    /// <summary>
    /// Return the content type of the current served page.
    /// </summary>
    /// <returns></returns>
    public static PageType GetCurrentPageType()
    {
      PageType currentPageType = PageType.Unknown;

      var page = CurrentPage;
      if (page != null)
      {
        var currentHandler = Path.GetFileName(page.Request.CurrentExecutionFilePath).ToLower();

        if (currentHandler == "default.aspx")
        {
          if (page.ToString().Contains("default_aspx"))
          {
            // Copied from default.aspx
            if (page.Request.RawUrl.ToLowerInvariant().Contains("/category/"))
            {
              currentPageType = PageType.PostsByCategory;
            }
            else if (page.Request.RawUrl.ToLowerInvariant().Contains("/author/"))
            {
              currentPageType = PageType.PostsByAuthor;
            }
            else if (page.Request.RawUrl.ToLowerInvariant().Contains("?tag="))
            {
              currentPageType = PageType.PostsByTag;
            }
            else if (page.Request.QueryString["year"] != null || page.Request.QueryString["date"] != null ||
                     page.Request.QueryString["calendar"] != null)
            {
              currentPageType = PageType.PostsByTimeRange;
            }
            else if (page.Request.QueryString["apml"] != null)
            {
              currentPageType = PageType.PostsByApmlFilter;
            }
            else
            {
              currentPageType = PageType.AllPosts;
            }
          }
        }
        if (currentHandler == "page.aspx")
        {
          currentPageType = PageType.Page;
        }
        if (currentHandler == "archive.aspx")
        {
          currentPageType = PageType.Archive;
        }
        if (currentHandler == "contact.aspx")
        {
          currentPageType = PageType.Contact;
        }
        if (currentHandler == "search.aspx")
        {
          currentPageType = PageType.Search;
        }
        if (currentHandler == "custompage.aspx")
        {
          currentPageType = PageType.CustomPage;
        }
        if (currentHandler == "post.aspx")
        {
          currentPageType = PageType.Post;
        }
      }

      return currentPageType;
    }

    #endregion

    #region Caching

    /// <summary>
    /// Removes an item from the current cache implementation.
    /// </summary>
    /// <param name="cacheKey">The cache key.</param>
    public static void CacheRemove(string cacheKey)
    {
      HttpRuntime.Cache.Remove(cacheKey);
    }

    /// <summary>
    /// Gets an item from the current cache implementation. If no item is cached, the callback func will be executed to 
    /// add the data to the cache.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="cacheKey">The cache key.</param>
    /// <param name="dataLoadCallback">The data load callback.</param>
    /// <returns></returns>
    public static T CacheGet<T>(string cacheKey, Func<T> dataLoadCallback) where T : class
    {
      var value = HttpRuntime.Cache[cacheKey] as T;

      if (value == null)
      {
        lock (lockObject)
        {
          if (value == null)
          {
            value = dataLoadCallback();
            HttpRuntime.Cache.Insert(cacheKey, value, null);
          }
        }
      }

      return value;
    }

    #endregion

    #region Content helpers

    /// <summary>
    /// Gets the source attribute of the first image tag found that is found in the text.
    /// </summary>
    /// <param name="content">The content.</param>
    /// <param name="useDefault">if set to <c>true</c> a default image url will be returned. The default url points to themes\yourtheme\images\default-thumb.gif.</param>
    /// <returns></returns>
    public static string GetFirstImageSource(string content, bool useDefault)
    {
      string source = null;

      if (!string.IsNullOrEmpty(content))
      {
        const string pattern = @"<img(.|\n)+?>";

        var match = Regex.Match(content, pattern, RegexOptions.IgnoreCase | RegexOptions.Multiline);

        if (match.Success)
        {
          source = GetAttributeValue("src", match.Value);
        }
      }
      if (string.IsNullOrEmpty(source) && useDefault)
      {
        source = Utils.RelativeWebRoot + "themes/" + BlogSettings.Instance.Theme + "/images/default-thumb.gif";
      }

      return source;
    }

    /// <summary>
    /// Gets the url of a comment avatar.
    /// </summary>
    /// <param name="size">The size.</param>
    /// <param name="comment">The comment.</param>
    /// <returns></returns>
    public static string GetAvatarImageUrl(int size, Comment comment)
    {
      string avatarImageTag = Avatar.GetAvatarImageTag(size, comment.Email, comment.Website, comment.Avatar, comment.Author);
      return GetFirstImageSource(avatarImageTag, false);
    }

    /// <summary>
    /// Returns the first number of letters from the content of the <see cref="IPublishable"/>. If the item has 
    /// a description, this value is used instead of the content. Existing html elements will be removed.
    /// </summary>
    /// <param name="contentItem">The content item.</param>
    /// <param name="maxChars">The max chars.</param>
    /// <returns></returns>
    public static string GetShortenedContent(IPublishable contentItem, int maxChars)
    {
      string content;

      if (!string.IsNullOrEmpty(contentItem.Description))
      {
        content = contentItem.Description;
      }
      else
      {
        content = Utils.StripHtml(contentItem.Content);
      }
      if (content.Length > maxChars)
      {
        content = content.Substring(0, maxChars);
      }

      return content;
    }

    #endregion

    #region Misc

    private static System.Web.UI.Page CurrentPage
    {
      get
      {
        System.Web.UI.Page currentPage = null;
        if (HttpContext.Current != null)
        {
          currentPage = HttpContext.Current.CurrentHandler as System.Web.UI.Page;
        }
        return currentPage;
      }
    }

    /// <summary>
    /// Tries to parse the input as an integer value. If an error occurs, the default value will be returned.
    /// </summary>
    /// <param name="input">The input.</param>
    /// <param name="defaultValue">The default value.</param>
    /// <returns></returns>
    public static int TryParseOrDefault(string input, int defaultValue)
    {
      int value;
      if (!Int32.TryParse(input, out value))
      {
        value = defaultValue;
      }
      return value;
    }

    #endregion
  }
}