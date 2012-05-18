#region using

using System.Text.RegularExpressions;
using System.Web;

using BlogEngine.Core;
using BlogEngine.Core.Web.Controls;

using CodeFormatter;

#endregion

/// <summary>
/// Converts text to formatted syntax highlighted code.
/// </summary>
/// <remarks>
/// It is a work in progress.....
/// </remarks>
[Extension("Converts text to formatted syntax highlighted code (beta).", "0.1", "www.manoli.net")]
public class CodeFormatterExtension
{
    #region Constants and Fields

    /// <summary>
    /// The code regex.
    /// </summary>
    private static readonly Regex CodeRegex =
        new Regex(
            @"(?<begin>\[code:(?<lang>.*?)(?:;ln=(?<linenumbers>(?:on|off)))?(?:;alt=(?<altlinenumbers>(?:on|off)))?(?:;(?<title>.*?))?\])(?<code>.*?)(?<end>\[/code\])",
            RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase | RegexOptions.Singleline);

    #endregion

    #region Constructors and Destructors

    /// <summary>
    /// Initializes static members of the <see cref="CodeFormatterExtension"/> class. 
    /// </summary>
    static CodeFormatterExtension()
    {
        Page.Serving += ServingContent;
        Post.Serving += ServingContent;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Codes the evaluator.
    /// </summary>
    /// <param name="match">The match.</param>
    /// <returns>The code string.</returns>
    private static string CodeEvaluator(Match match)
    {
        if (!match.Success)
        {
            return match.Value;
        }

        var options = new HighlightOptions
            {
                Language = match.Groups["lang"].Value,
                Code = match.Groups["code"].Value,
                DisplayLineNumbers = match.Groups["linenumbers"].Value == "on" ? true : false,
                Title = match.Groups["title"].Value,
                AlternateLineNumbers = match.Groups["altlinenumbers"].Value == "on" ? true : false
            };

        var result = match.Value.Replace(match.Groups["begin"].Value, string.Empty);
        result = result.Replace(match.Groups["end"].Value, string.Empty);
        result = Highlight(options, result);
        return result;
    }

    /// <summary>
    /// Returns the formatted text.
    /// </summary>
    /// <param name="options">
    /// Whatever options were set in the regex groups.
    /// </param>
    /// <param name="text">
    /// Send the e.body so it can get formatted.
    /// </param>
    /// <returns>
    /// The formatted string of the match.
    /// </returns>
    private static string Highlight(HighlightOptions options, string text)
    {
        switch (options.Language)
        {
            case "c#":
                var csf = new CSharpFormat
                    {
                        LineNumbers = options.DisplayLineNumbers,
                        Alternate = options.AlternateLineNumbers
                    };
                return HttpContext.Current.Server.HtmlDecode(csf.FormatCode(text));

            case "vb":
                var vbf = new VisualBasicFormat
                    {
                        LineNumbers = options.DisplayLineNumbers,
                        Alternate = options.AlternateLineNumbers
                    };
                return HttpContext.Current.Server.HtmlDecode(vbf.FormatCode(text));

            case "js":
                var jsf = new JavaScriptFormat
                    {
                        LineNumbers = options.DisplayLineNumbers,
                        Alternate = options.AlternateLineNumbers
                    };
                return HttpContext.Current.Server.HtmlDecode(jsf.FormatCode(text));

            case "html":
                var htmlf = new HtmlFormat
                    {
                        LineNumbers = options.DisplayLineNumbers,
                        Alternate = options.AlternateLineNumbers
                    };
                text = Utils.StripHtml(text);
                var code = htmlf.FormatCode(HttpContext.Current.Server.HtmlDecode(text)).Trim();
                return code.Replace("\r\n", "<br />").Replace("\n", "<br />");

            case "xml":
                var xmlf = new HtmlFormat
                    {
                        LineNumbers = options.DisplayLineNumbers,
                        Alternate = options.AlternateLineNumbers
                    };
                text = text.Replace("<br />", "\r\n");
                text = Utils.StripHtml(text);
                var xml = xmlf.FormatCode(HttpContext.Current.Server.HtmlDecode(text)).Trim();
                return xml.Replace("\r\n", "<br />").Replace("\n", "<br />");

            case "tsql":
                var tsqlf = new TsqlFormat
                    {
                        LineNumbers = options.DisplayLineNumbers,
                        Alternate = options.AlternateLineNumbers
                    };
                return HttpContext.Current.Server.HtmlDecode(tsqlf.FormatCode(text));

            case "msh":
                var mshf = new MshFormat
                    {
                        LineNumbers = options.DisplayLineNumbers,
                        Alternate = options.AlternateLineNumbers
                    };
                return HttpContext.Current.Server.HtmlDecode(mshf.FormatCode(text));
        }

        return string.Empty;
    }

    /// <summary>
    /// Handles the Serving event of the control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="BlogEngine.Core.ServingEventArgs"/> instance containing the event data.</param>
    private static void ServingContent(object sender, ServingEventArgs e)
    {
        if (e.Body.Contains("[/code]"))
        {
            e.Body = CodeRegex.Replace(e.Body, new MatchEvaluator(CodeEvaluator));
        }
    }

    #endregion

    /// <summary>
    /// Handles all of the options for changing the rendered code.
    /// </summary>
    private class HighlightOptions
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="HighlightOptions"/> class.
        /// </summary>
        public HighlightOptions()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HighlightOptions"/> class.
        /// </summary>
        /// <param name="language">
        /// The language.
        /// </param>
        /// <param name="title">
        /// The title.
        /// </param>
        /// <param name="linenumbers">
        /// The linenumbers.
        /// </param>
        /// <param name="code">
        /// The code string.
        /// </param>
        /// <param name="alternateLineNumbers">
        /// The alternate line numbers.
        /// </param>
        public HighlightOptions(string language, string title, bool linenumbers, string code, bool alternateLineNumbers)
        {
            this.Language = language;
            this.Title = title;
            this.AlternateLineNumbers = alternateLineNumbers;
            this.Code = code;
            this.DisplayLineNumbers = linenumbers;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether AlternateLineNumbers.
        /// </summary>
        public bool AlternateLineNumbers { get; set; }

        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether DisplayLineNumbers.
        /// </summary>
        public bool DisplayLineNumbers { get; set; }

        /// <summary>
        /// Gets or sets Language.
        /// </summary>
        public string Language { get; set; }

        /// <summary>
        /// Gets or sets Title.
        /// </summary>
        public string Title { get; set; }

        #endregion
    }
}