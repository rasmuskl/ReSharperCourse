namespace BlogEngine.Core.Web.HttpModules
{
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Web;
    using System.IO;

    /// <summary>
    /// Handles pretty URL's and redirects them to the permalinks.
    /// </summary>
    public class UrlRewrite : IHttpModule
    {
        #region Constants and Fields

        /// <summary>
        /// The Year Regex.
        /// </summary>
        private static readonly Regex YearRegex = new Regex(
            "/([0-9][0-9][0-9][0-9])/",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        /// <summary>
        /// The Year Month Regex.
        /// </summary>
        private static readonly Regex YearMonthRegex = new Regex(
            "/([0-9][0-9][0-9][0-9])/([0-1][0-9])/",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        /// <summary>
        /// The Year Month Day Regex.
        /// </summary>
        private static readonly Regex YearMonthDayRegex = new Regex(
            "/([0-9][0-9][0-9][0-9])/([0-1][0-9])/([0-3][0-9])/",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        #endregion

        #region Implemented Interfaces

        #region IHttpModule

        /// <summary>
        /// Disposes of the resources (other than memory) used by the module that implements <see cref="T:System.Web.IHttpModule"/>.
        /// </summary>
        public void Dispose()
        {
            // Nothing to dispose
        }

        /// <summary>
        /// Initializes a module and prepares it to handle requests.
        /// </summary>
        /// <param name="context">An <see cref="T:System.Web.HttpApplication"/> that provides access to the methods, properties, and events common to all application objects within an ASP.NET application</param>
        public void Init(HttpApplication context)
        {
            context.BeginRequest += ContextBeginRequest;
        }

        #endregion

        #endregion

        #region Methods

        private static string GetUrlWithQueryString(HttpContext context)
        {
            return string.Format(
                "{0}?{1}", context.Request.Path, context.Request.QueryString);
        }

        /// <summary>
        /// Extracts the year and month from the requested URL and returns that as a DateTime.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="year">
        /// The year number.
        /// </param>
        /// <param name="month">
        /// The month number.
        /// </param>
        /// <param name="day">
        /// The day number.
        /// </param>
        /// <returns>
        /// Whether date extraction succeeded.
        /// </returns>
        private static bool ExtractDate(HttpContext context, out int year, out int month, out int day)
        {
            year = 0;
            month = 0;
            day = 0;

            if (!BlogSettings.Instance.TimeStampPostLinks) {
                return false;
            }

            var match = YearMonthDayRegex.Match(GetUrlWithQueryString(context));
            if (match.Success) {
                year = int.Parse(match.Groups[1].Value, CultureInfo.InvariantCulture);
                month = int.Parse(match.Groups[2].Value, CultureInfo.InvariantCulture);
                day = int.Parse(match.Groups[3].Value, CultureInfo.InvariantCulture);
                return true;
            }

            match = YearMonthRegex.Match(GetUrlWithQueryString(context));
            if (match.Success) {
                year = int.Parse(match.Groups[1].Value, CultureInfo.InvariantCulture);
                month = int.Parse(match.Groups[2].Value, CultureInfo.InvariantCulture);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Extracts the title from the requested URL.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="url">
        /// The url string.
        /// </param>
        /// <returns>
        /// The extract title.
        /// </returns>
        private static string ExtractTitle(HttpContext context, string url)
        {
            url = url.ToLowerInvariant().Replace("---", "-");
            if (url.Contains(BlogConfig.FileExtension) && url.EndsWith("/")) {
                url = url.Substring(0, url.Length - 1);
                context.Response.AppendHeader("location", url);
                context.Response.StatusCode = 301;
            }

            url = url.Substring(0, url.IndexOf(BlogConfig.FileExtension, StringComparison.Ordinal));
            var index = url.LastIndexOf("/", StringComparison.Ordinal) + 1;
            var title = url.Substring(index);
            return context.Server.HtmlEncode(title);
        }

        /// <summary>
        /// Gets the query string from the requested URL.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <returns>
        /// The query string.
        /// </returns>
        private static string GetQueryString(HttpContext context)
        {
            var query = context.Request.QueryString.ToString();
            return !string.IsNullOrEmpty(query) ? string.Format("&{0}", query) : string.Empty;
        }

        /// <summary>
        /// Rewrites the category.
        /// </summary>
        /// <param name="context">The HTTP context.</param>
        /// <param name="url">The URL string.</param>
        private static void RewriteCategory(HttpContext context, string url)
        {
            var title = ExtractTitle(context, url);
            foreach (var cat in from cat in Category.Categories
                                let legalTitle = Utils.RemoveIllegalCharacters(cat.Title).ToLowerInvariant()
                                where title.Equals(legalTitle, StringComparison.OrdinalIgnoreCase)
                                select cat) {
                if (url.Contains("/FEED/")) {
                    context.RewritePath(string.Format("syndication.axd?category={0}{1}", cat.Id, GetQueryString(context)), false);
                }
                else {
                    context.RewritePath(
                        string.Format("{0}default.aspx?id={1}{2}", Utils.ApplicationRelativeWebRoot, cat.Id, GetQueryString(context)), false);
                    break;
                }
            }
        }

        /// <summary>
        /// Rewrites the tag.
        /// </summary>
        /// <param name="context">The HTTP context.</param>
        /// <param name="url">The URL string.</param>
        private static void RewriteTag(HttpContext context, string url)
        {
            var tag = ExtractTitle(context, url);

            context.RewritePath(
                url.Contains("/FEED/")
                    ? string.Format("syndication.axd?tag={0}{1}", tag, GetQueryString(context))
                    : string.Format("{0}?tag=/{1}{2}", Utils.ApplicationRelativeWebRoot, tag, GetQueryString(context)),
                false);
        }

        /// <summary>
        /// The rewrite default.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        private static void RewriteDefault(HttpContext context)
        {
            var url = GetUrlWithQueryString(context);
            var page = string.Format("&page={0}", context.Request.QueryString["page"]);
            if (string.IsNullOrEmpty(context.Request.QueryString["page"])) {
                page = null;
            }

            if (YearMonthDayRegex.IsMatch(url)) {
                var match = YearMonthDayRegex.Match(url);
                var year = match.Groups[1].Value;
                var month = match.Groups[2].Value;
                var day = match.Groups[3].Value;
                var date = string.Format("{0}-{1}-{2}", year, month, day);
                context.RewritePath(string.Format("{0}default.aspx?date={1}{2}", Utils.ApplicationRelativeWebRoot, date, page), false);
            }
            else if (YearMonthRegex.IsMatch(url)) {
                var match = YearMonthRegex.Match(url);
                var year = match.Groups[1].Value;
                var month = match.Groups[2].Value;
                var path = string.Format("default.aspx?year={0}&month={1}", year, month);
                context.RewritePath(Utils.ApplicationRelativeWebRoot + path + page, false);
            }
            else if (YearRegex.IsMatch(url)) {
                var match = YearRegex.Match(url);
                var year = match.Groups[1].Value;
                var path = string.Format("default.aspx?year={0}", year);
                context.RewritePath(Utils.ApplicationRelativeWebRoot + path + page, false);
            }
            else {
                int defaultStart = url.IndexOf("default.aspx", StringComparison.OrdinalIgnoreCase);
                string newUrl = Utils.ApplicationRelativeWebRoot + url.Substring(defaultStart);

                context.RewritePath(newUrl);
            }
        }

        /// <summary>
        /// Rewrites the page.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="url">The URL string.</param>
        private static void RewritePage(HttpContext context, string url)
        {
            var slug = ExtractTitle(context, url);
            var page =
                Page.Pages.Find(
                    p => slug.Equals(Utils.RemoveIllegalCharacters(p.Slug), StringComparison.OrdinalIgnoreCase));

            if (page != null) {
                context.RewritePath(string.Format("{0}page.aspx?id={1}{2}", Utils.ApplicationRelativeWebRoot, page.Id, GetQueryString(context)), false);
            }
        }

        /// <summary>
        /// Rewrites the post.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="url">The URL string.</param>
        private static void RewritePost(HttpContext context, string url)
        {
            int year, month, day;

            var haveDate = ExtractDate(context, out year, out month, out day);
            var slug = ExtractTitle(context, url);

            // Allow for Year/Month only dates in URL (in this case, day == 0), as well as Year/Month/Day dates.
            // first make sure the Year and Month match.
            // if a day is also available, make sure the Day matches.
            var post = Post.Posts.Find(
                p =>
                (!haveDate || (p.DateCreated.Year == year && p.DateCreated.Month == month)) &&
                ((!haveDate || (day == 0 || p.DateCreated.Day == day)) &&
                 slug.Equals(Utils.RemoveIllegalCharacters(p.Slug), StringComparison.OrdinalIgnoreCase)));

            if (post == null) {
                return;
            }

            var q = GetQueryString(context);
            q = q.Contains("id=", StringComparison.OrdinalIgnoreCase) ? string.Format("{0}post.aspx?{1}", Utils.ApplicationRelativeWebRoot, q) : string.Format("{0}post.aspx?id={1}{2}", Utils.ApplicationRelativeWebRoot, post.Id, q);

            context.RewritePath(
                url.Contains("/FEED/")
                    ? string.Format("syndication.axd?post={0}{1}", post.Id, GetQueryString(context))
                    : q,
                false);
        }

        /// <summary>
        /// Rewrites the incoming file request to the actual handler
        /// </summary>
        /// <param name="context">the context</param>
        /// <param name="url">the url string</param>
        private static void RewriteFilePath(HttpContext context, string url)
        {
            var awr = Utils.AbsoluteWebRoot;
            url = url.ToLower().Replace(awr.ToString().ToLower(), string.Empty).Replace("files/", string.Empty);
            url = url.Substring(0, url.LastIndexOf(Path.GetExtension(url) ?? "", StringComparison.Ordinal));
            var npath = string.Format("{0}file.axd?file={1}", Utils.ApplicationRelativeWebRoot, url);
            context.RewritePath(npath);
        }

        /// <summary>
        /// Rewrites the incoming image request to the actual handler
        /// </summary>
        /// <param name="context">the context</param>
        /// <param name="url">the url string</param>
        private static void RewriteImagePath(HttpContext context, string url)
        {
            var awr = Utils.AbsoluteWebRoot;
            url = url.ToLower().Replace(awr.ToString().ToLower(), string.Empty).Replace("images/", string.Empty);
            url = url.Substring(0, url.LastIndexOf(Path.GetExtension(url) ?? "", StringComparison.Ordinal));
            var npath = string.Format("{0}image.axd?picture={1}", Utils.ApplicationRelativeWebRoot, url);
            context.RewritePath(npath);
        }

        private static void RewriteBundlePathForChildBlog(HttpContext context, string url)
        {
            if (url.Contains("/SCRIPTS/") || url.Contains("/STYLES/"))
            {
                var npath = url.Replace(Blog.CurrentInstance.RelativeWebRoot, "/");
                context.RewritePath(npath);
            }
        }

        /// <summary>
        /// Handles the BeginRequest event of the context control.
        /// </summary>
        /// <param name="sender">
        /// The source of the event.
        /// </param>
        /// <param name="e">
        /// The <see cref="System.EventArgs"/> instance containing the event data.
        /// </param>
        private static void ContextBeginRequest(object sender, EventArgs e)
        {
            var context = ((HttpApplication)sender).Context;
            var path = context.Request.Path.ToUpperInvariant();
            var url = GetUrlWithQueryString(context).ToUpperInvariant();

            path = path.Replace(".ASPX.CS", string.Empty);
            url = url.Replace(".ASPX.CS", string.Empty);

            // to prevent XSS
            url = HttpUtility.HtmlEncode(url);

            Blog blogInstance = Blog.CurrentInstance;

            // bundled scripts and styles are in the ~/scripts and ~/styles
            // redirect path from ~/child/scripts/js to ~/scripts/js etc.
            if (!blogInstance.IsPrimary)
            {
                if (url.Contains("/SCRIPTS/") || url.Contains("/STYLES/"))
                {
                    var npath = url.Replace(Blog.CurrentInstance.RelativeWebRoot.ToUpper(), "/");
                    context.RewritePath(npath);
                    return;
                }
            }

            if (Utils.IsCurrentRequestForHomepage) {
                var front = Page.GetFrontPage();
                if (front != null) {
                    url = front.RelativeLink.ToUpperInvariant();
                }
            }

            var urlContainsFileExtension = url.IndexOf(BlogConfig.FileExtension, StringComparison.OrdinalIgnoreCase) != -1;

            if(url.Contains("/FILES/") && url.Contains(".AXDX"))
                RewriteFilePath(context, url);
            if (url.Contains("/IMAGES/") && url.Contains(".JPGX"))
                RewriteImagePath(context, url);
            if (urlContainsFileExtension && url.Contains("/POST/")) {
                RewritePost(context, url);
            }
            else if (urlContainsFileExtension && url.Contains("/CATEGORY/")) {
                RewriteCategory(context, url);
            }
            else if (urlContainsFileExtension && url.Contains("/TAG/")) {
                RewriteTag(context, url);
            }
            else if (urlContainsFileExtension && url.Contains("/PAGE/")) {
                RewritePage(context, url);
            }
            else if (urlContainsFileExtension && url.Contains("/CALENDAR/")) {
                context.RewritePath(string.Format("{0}default.aspx?calendar=show", Utils.ApplicationRelativeWebRoot), false);
            }
            else if (urlContainsFileExtension && DefaultPageRequested(context))
            {
                RewriteDefault(context);
            }
            else if (urlContainsFileExtension && url.Contains("/AUTHOR/")) {
                var author = ExtractTitle(context, url);
                context.RewritePath(
                    string.Format("{0}default{1}?name={2}{3}", Utils.ApplicationRelativeWebRoot, BlogConfig.FileExtension, author, GetQueryString(context)),
                    false);
            }
            else if (urlContainsFileExtension && path.Contains("/BLOG.ASPX")) {
                context.RewritePath(string.Format("{0}default.aspx?blog=true{1}", Utils.ApplicationRelativeWebRoot, GetQueryString(context)));
            }
            else {
                // If this is blog instance that is in a virtual sub-folder, we will
                // need to rewrite the path for URL to a physical file.  This includes
                // requests such as the homepage (default.aspx), contact.aspx, archive.aspx,
                // any of the admin pages, etc, etc.

                if (blogInstance.IsSubfolderOfApplicationWebRoot &&
                    VirtualPathUtility.AppendTrailingSlash(path).IndexOf(blogInstance.RelativeWebRoot, StringComparison.OrdinalIgnoreCase) != -1) {
                    bool skipRewrite = false;
                    string rewriteQs = string.Empty;
                    string rewriteUrl = GetUrlWithQueryString(context);

                    int qsStart = rewriteUrl.IndexOf("?", StringComparison.Ordinal);
                    if (qsStart != -1)  // remove querystring.
                    {
                        rewriteQs = rewriteUrl.Substring(qsStart);
                        rewriteUrl = rewriteUrl.Substring(0, qsStart);
                    }

                    // Want to see if a specific page/file is being requested (something with a . (dot) in it).
                    // Because Utils.ApplicationRelativeWebRoot may contain a . (dot) in it, pathAfterAppWebRoot
                    // tells us if the actual path (after the AppWebRoot) contains a dot.
                    string pathAfterAppWebRoot = rewriteUrl.Substring(Utils.ApplicationRelativeWebRoot.Length);

                    if (!pathAfterAppWebRoot.Contains(".")) {
                        if (!rewriteUrl.EndsWith("/"))
                            rewriteUrl += "/";

                        rewriteUrl += "default.aspx";
                    }
                    else
                    {
                        var extension = Path.GetExtension(pathAfterAppWebRoot);
                        if (extension != null && extension.ToUpperInvariant() == ".AXD")
                            skipRewrite = true;
                    }

                        if (!skipRewrite) {
                        // remove the subfolder portion.  so /subfolder/ becomes /.
                        rewriteUrl = new Regex(Regex.Escape(blogInstance.RelativeWebRoot), RegexOptions.IgnoreCase).Replace(rewriteUrl, Utils.ApplicationRelativeWebRoot);

                        context.RewritePath(rewriteUrl + rewriteQs, false);
                    }
                }
            }
        }

        private static bool DefaultPageRequested(HttpContext context)
        {
            var url = context.Request.Url.ToString();
            var match = string.Format("{0}DEFAULT{1}", Utils.AbsoluteWebRoot, BlogConfig.FileExtension);

            var u = GetUrlWithQueryString(context);
            var m = YearMonthDayRegex.Match(u);

            // case when month/day clicked in the calendar widget/control
            // default page will be like site.com/2012/10/15/default.aspx
            if (!m.Success)
            {
                // case when month clicked in the month list
                // default page will be like site.com/2012/10/default.aspx
                m = YearMonthRegex.Match(u);
            }

            if (m.Success)
            {
                var s = string.Format("{0}{1}DEFAULT{2}", Utils.AbsoluteWebRoot, m.ToString().Substring(1), BlogConfig.FileExtension);

                Utils.Log("Url: " + url + "; s: " + s);

                if (url.Contains(s, StringComparison.OrdinalIgnoreCase))
                    return true;
            }
            
            return url.Contains(match, StringComparison.InvariantCultureIgnoreCase);
        }

        #endregion
    }
}