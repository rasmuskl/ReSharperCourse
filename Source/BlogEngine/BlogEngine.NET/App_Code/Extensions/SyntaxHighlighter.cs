using System.Text;
using System.Web;
using System.Web.UI.HtmlControls;
using BlogEngine.Core;
using BlogEngine.Core.Web.Controls;
using BlogEngine.Core.Web.Extensions;
using Page=System.Web.UI.Page;
using System.Collections.Generic;
using System;

[Extension("Adds <a target=\"_new\" href=\"http://alexgorbatchev.com/wiki/SyntaxHighlighter\">Alex Gorbatchev's</a> source code formatter", "2.5.1", "<a target=\"_new\" href=\"http://dotnetblogengine.net/\">BlogEngine.NET</a>")]
public class SyntaxHighlighter
{
    #region Private members
    private const string ExtensionName = "SyntaxHighlighter";
    static protected Dictionary<Guid, ExtensionSettings> _blogsOptions = new Dictionary<Guid, ExtensionSettings>();
    static protected Dictionary<Guid, ExtensionSettings> _blogsThemes = new Dictionary<Guid, ExtensionSettings>();
    #endregion

    /// <summary>
    ///     The sync root.
    /// </summary>
    private static readonly object syncRoot = new object();

    private static ExtensionSettings Options
    {
        get
        {
            Guid blogId = Blog.CurrentInstance.Id;
            ExtensionSettings options = null;
            _blogsOptions.TryGetValue(blogId, out options);

            if (options == null)
            {
                lock (syncRoot)
                {
                    _blogsOptions.TryGetValue(blogId, out options);

                    if (options == null)
                    {
                        // Initializes
                        //   (1) Options
                        //   (3) Themees
                        // for the current blog instance.

                        // options
                        options = new ExtensionSettings("Options");
                        options.IsScalar = true;
                        options.Help = OptionsHelp();

                        options.AddParameter("cdnScriptsPath", "CDN Script Path", 250, false);
                        options.AddParameter("cdnStylesPath", "CDN Styles Path", 250, false);
                        options.AddParameter("gutter", "Gutter");
                        options.AddParameter("smart-tabs", "Smart tabs");
                        options.AddParameter("auto-links", "Auto links");
                        options.AddParameter("collapse", "Collapse");
                        options.AddParameter("tab-size", "Tab size");
                        options.AddParameter("toolbar", "Toolbar");

                        options.AddValue("cdnScriptsPath", "http://alexgorbatchev.com.s3.amazonaws.com/pub/sh/3.0.83/scripts/");
                        options.AddValue("cdnStylesPath", "http://alexgorbatchev.com.s3.amazonaws.com/pub/sh/3.0.83/styles/");
                        options.AddValue("gutter", true);
                        options.AddValue("smart-tabs", true);
                        options.AddValue("auto-links", true);
                        options.AddValue("collapse", false);
                        options.AddValue("tab-size", 4);
                        options.AddValue("toolbar", true);

                        _blogsOptions[blogId] = ExtensionManager.InitSettings(ExtensionName, options);

                        // themes
                        ExtensionSettings themes = new ExtensionSettings("Themes");
                        themes.IsScalar = true;
                        themes.AddParameter("SelectedTheme", "Themes", 20, false, false, ParameterType.ListBox);
                        themes.AddValue("SelectedTheme", new string[] { "Default", "Django", "Eclipse", "Emacs", "FadeToGrey", "MDUltra", "Midnight", "Dark" }, "Default");
                        _blogsThemes[blogId] = ExtensionManager.InitSettings(ExtensionName, themes);
                    }
                }
            }

            return options;
        }
    }

    private static ExtensionSettings Themes
    {
        get
        {
            // by invoking the "Options" property getter, we are ensuring
            // that an entry is put into _blogsThemes for the current blog instance.
            ExtensionSettings options = Options;
            return _blogsThemes[Blog.CurrentInstance.Id];
        }
    }

    static SyntaxHighlighter()
    {
        Post.Serving += AddSyntaxHighlighter;
        InitSettings();
    }

    private static void AddSyntaxHighlighter(object sender, ServingEventArgs e)
    {
        if (!ExtensionManager.ExtensionEnabled("SyntaxHighlighter"))
            return;

		if(e.Location == ServingLocation.Feed) 
            return;

        // if no code blocks on the page - don't bother
        if (!e.Body.ToLowerInvariant().Contains("<pre class=\"brush:"))
            return;
	
        HttpContext context = HttpContext.Current;
		
        Page page = (Page)context.CurrentHandler;

        if ((context.CurrentHandler is Page == false) || (context.Items[ExtensionName] != null))
        {
            return;
        }

        AddCssStyles(page);
        AddJavaScripts(page);
        AddOptions(page);

        context.Items[ExtensionName] = 1;
    }

    private static void AddCssStyles(Page page)
    {
        AddStylesheet("shCore.css", page);

        if (Themes != null)
        {
            switch (Themes.GetSingleValue("SelectedTheme"))
            {
                case "Django":
                    AddStylesheet("shThemeDjango.css", page);
                    break;
                case "Eclipse":
                    AddStylesheet("shThemeEclipse.css", page);
                    break;
                case "Emacs":
                    AddStylesheet("shThemeEmacs.css", page);
                    break;
                case "FadeToGrey":
                    AddStylesheet("shThemeFadeToGrey.css", page);
                    break;
                case "MDUltra":
                    AddStylesheet("shThemeMDUltra.css", page);
                    break;
                case "Midnight":
                    AddStylesheet("shThemeMidnight.css", page);
                    break;
                case "Dark":
                    AddStylesheet("shThemeRDark.css", page);
                    break;
                default:
                    AddStylesheet("shThemeDefault.css", page);
                    break;
            }
        }       
    }

    private static void AddJavaScripts(Page page)
    {
        BlogEngine.Core.Web.Scripting.Helpers.AddScript(
            page, string.Format("{0}Scripts/highlighter", Utils.ApplicationRelativeWebRoot), false, true, true);
    }

    #region Script/Style adding

    private static void AddJavaScript(string src, Page page)
    {
        page.ClientScript.RegisterStartupScript(page.GetType(), src, String.Format("<script type=\"text/javascript\" src=\"{0}\"></script>", GetUrl(ScriptsFolder(), src)));
    }

    private static void AddStylesheet(string href, Page page)
    {
        HtmlLink css = new HtmlLink();
        css.Attributes["type"] = "text/css";
        css.Attributes["rel"] = "stylesheet";
        css.Attributes["href"] = GetUrl(StylesFolder(), href);
        page.Header.Controls.Add(css);
    }

    private static void AddOptions(Page page)
    {
        StringBuilder sb = new StringBuilder();
        
        sb.AppendLine("<script type=\"text/javascript\" defer=\"defer\">");

        // add not-default options
        if (Options != null)
        {
            if(Options.GetSingleValue("gutter").ToLowerInvariant() == "false")
                sb.AppendLine(GetOption("gutter"));

            if (Options.GetSingleValue("smart-tabs").ToLowerInvariant() == "false")
                sb.AppendLine(GetOption("smart-tabs"));

            if (Options.GetSingleValue("auto-links").ToLowerInvariant() == "false")
                sb.AppendLine(GetOption("auto-links"));

            if (Options.GetSingleValue("collapse").ToLowerInvariant() == "true")
                sb.AppendLine(GetOption("collapse"));
            
            if (Options.GetSingleValue("toolbar").ToLowerInvariant() == "false")
                sb.AppendLine(GetOption("toolbar"));

            if (Options.GetSingleValue("tab-size") != "4")
                sb.AppendLine(GetOption("tab-size"));
        }  
        
        //sb.AppendLine("\tSyntaxHighlighter.all();");
        sb.AppendLine("</script>");
        page.ClientScript.RegisterStartupScript(page.GetType(), "SyntaxHighlighter", sb.ToString(), false);
    }

    private static string GetUrl(string folder, string url)
    {
        string s = HttpContext.Current.Server.UrlPathEncode(string.Format("{0}{1}", folder, url));
        if (!folder.ToLowerInvariant().Contains("http:") && !folder.ToLowerInvariant().Contains("https://"))
            s = Utils.ApplicationRelativeWebRoot + s;
        return s;
    }
    
    #endregion

    #region Private methods

    private static void InitSettings()
    {
        // call Options getter so default settings are loaded on application start.
        var s = Options;
    }

    static string OptionsHelp()
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("<p>This extension implements excellent Alex Gorbatchev's syntax highlighter JS library for source code formatting. Please refer to <a target=\"_new\" href=\"http://alexgorbatchev.com/wiki/SyntaxHighlighter:Usage\">this site</a> for usage.</p>");
        sb.AppendLine("<p><b>cdnScriptsPath</b>: Allows you to load the SyntaxHighlighter script from a CDN. Leave empty for local files</p>");
        sb.AppendLine("<p><b>cdnStylesPath</b>: Allows you to load the SyntaxHighlighter styles from a CDN. Leave empty for local files</p>");
        sb.AppendLine("<p><b>auto-links</b>: Allows you to turn detection of links in the highlighted element on and off. If the option is turned off, URLs won't be clickable.</p>");
        sb.AppendLine("<p><b>collapse</b>: Allows you to force highlighted elements on the page to be collapsed by default.</p>");
        sb.AppendLine("<p><b>gutter</b>:	Allows you to turn gutter with line numbers on and off.</p>");
        sb.AppendLine("<p><b>smart-tabs</b>:	Allows you to turn smart tabs feature on and off.</p>");
        sb.AppendLine("<p><b>tab-size</b>: Allows you to adjust tab size.</p>");
        sb.AppendLine("<p><b>toolbar</b>: Toggles toolbar on/off.</p>");
        sb.AppendLine("<p><a target=\"_new\" href=\"http://alexgorbatchev.com/wiki/SyntaxHighlighter:Configuration\">more...</a></p>");
        return sb.ToString();
    }

    static string GetOption(string opt)
    {
        if (Options != null)
        {
            string pattern = "\tSyntaxHighlighter.defaults['{0}'] = {1};";
            string val = Options.GetSingleValue(opt).ToLowerInvariant();
            return string.Format(pattern, opt, val);
        }
        return "";
    }

    static string ScriptsFolder()
    {
        if (Options != null)
        {
            if (!String.IsNullOrEmpty(Options.GetSingleValue("cdnScriptsPath")))
                return Options.GetSingleValue("cdnScriptsPath");
            else
                return "Scripts/syntaxhighlighter/";
        }
        return "";
    }

    static string StylesFolder()
    {
        if (Options != null)
        {
            if (!String.IsNullOrEmpty(Options.GetSingleValue("cdnStylesPath")))
                return Options.GetSingleValue("cdnStylesPath");
            else
                return "Styles/syntaxhighlighter/";
        }
        return "";
    }

    #endregion
}