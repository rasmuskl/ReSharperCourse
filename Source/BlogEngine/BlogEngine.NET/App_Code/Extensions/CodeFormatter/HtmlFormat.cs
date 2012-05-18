#region Copyright &copy; 2001-2003 Jean-Claude Manoli [jc@manoli.net]

/*
 * This software is provided 'as-is', without any express or implied warranty.
 * In no event will the author(s) be held liable for any damages arising from
 * the use of this software.
 * 
 * Permission is granted to anyone to use this software for any purpose,
 * including commercial applications, and to alter it and redistribute it
 * freely, subject to the following restrictions:
 * 
 *   1. The origin of this software must not be misrepresented; you must not
 *      claim that you wrote the original software. If you use this software
 *      in a product, an acknowledgment in the product documentation would be
 *      appreciated but is not required.
 * 
 *   2. Altered source versions must be plainly marked as such, and must not
 *      be misrepresented as being the original software.
 * 
 *   3. This notice may not be removed or altered from any source distribution.
 */
#endregion

namespace CodeFormatter
{
    using System.IO;
    using System.Text;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Generates color-coded HTML 4.01 from HTML/XML/ASPX source code.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This implementation assumes that code inside &lt;script&gt; blocks 
    ///         is JavaScript, and code inside &lt;% %&gt; blocks is C#.
    /// </para>
    /// <para>
    /// The default tab width is set to 2 characters in this class.
    /// </para>
    /// </remarks>
    public class HtmlFormat : SourceFormat
    {
        #region Constants and Fields

        /// <summary>
        /// The attrib regex.
        /// </summary>
        private readonly Regex attribRegex;

        /// <summary>
        /// To format embedded C# code.
        /// </summary>
        private readonly CSharpFormat csf;

        /// <summary>
        /// To format client-side JavaScript code.
        /// </summary>
        private readonly JavaScriptFormat jsf; 

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="HtmlFormat"/> class. 
        /// The html format.
        /// </summary>
        public HtmlFormat()
        {
            const string RegJavaScript = @"(?<=&lt;script(?:\s.*?)?&gt;).+?(?=&lt;/script&gt;)";
            const string RegComment = @"&lt;!--.*?--&gt;";
            const string RegAspTag = @"&lt;%@.*?%&gt;|&lt;%|%&gt;";
            const string RegAspCode = @"(?<=&lt;%).*?(?=%&gt;)";
            const string RegTagDelimiter = @"(?:&lt;/?!?\??(?!%)|(?<!%)/?&gt;)+";
            const string RegTagName = @"(?<=&lt;/?!?\??(?!%))[\w\.:-]+(?=.*&gt;)";
            const string RegAttributes = @"(?<=&lt;(?!%)/?!?\??[\w:-]+).*?(?=(?<!%)/?&gt;)";
            const string RegEntity = @"&amp;\w+;";
            const string RegAttributeMatch = @"(=?"".*?""|=?'.*?')|([\w:-]+)";

            // the regex object will handle all the replacements in one pass
            const string RegAll = "(" + RegJavaScript + ")|(" + RegComment + ")|(" + RegAspTag + ")|(" + RegAspCode + ")|(" +
                                  RegTagDelimiter + ")|(" + RegTagName + ")|(" + RegAttributes + ")|(" + RegEntity + ")";

            this.CodeRegex = new Regex(RegAll, RegexOptions.IgnoreCase | RegexOptions.Singleline);
            this.attribRegex = new Regex(RegAttributeMatch, RegexOptions.Singleline);

            this.csf = new CSharpFormat();
            this.jsf = new JavaScriptFormat();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Called to evaluate the HTML fragment corresponding to each 
        ///     matching token in the code.
        /// </summary>
        /// <param name="match">
        /// The <see cref="Match"/> resulting from a 
        ///     single regular expression match.
        /// </param>
        /// <returns>
        /// A string containing the HTML code fragment.
        /// </returns>
        protected override string MatchEval(Match match)
        {
            if (match.Groups[1].Success)
            {
                // JavaScript code
                var s = match.ToString();
                return this.jsf.FormatSubCode(match.ToString());
            }

            if (match.Groups[2].Success)
            {
                // comment
                StringBuilder sb;
                using (var reader = new StringReader(match.ToString()))
                {
                    string line;
                    sb = new StringBuilder();
                    while ((line = reader.ReadLine()) != null)
                    {
                        if (sb.Length > 0)
                        {
                            sb.Append("\n");
                        }

                        sb.Append("<span class=\"rem\">");
                        sb.Append(line);
                        sb.Append("</span>");
                    }
                }

                return sb.ToString();
            }

            if (match.Groups[3].Success)
            {
                // asp tag
                return string.Format("<span class=\"asp\">{0}</span>", match);
            }

            if (match.Groups[4].Success)
            {
                // asp C# code
                return this.csf.FormatSubCode(match.ToString());
            }

            if (match.Groups[5].Success)
            {
                // tag delimiter
                return string.Format("<span class=\"kwrd\">{0}</span>", match);
            }

            if (match.Groups[6].Success)
            {
                // html tagname
                return string.Format("<span class=\"html\">{0}</span>", match);
            }

            if (match.Groups[7].Success)
            {
                // attributes
                return this.attribRegex.Replace(match.ToString(), new MatchEvaluator(AttributeMatchEval));
            }

            if (match.Groups[8].Success)
            {
                // entity
                return string.Format("<span class=\"attr\">{0}</span>", match);
            }

            return match.ToString();
        }

        /// <summary>
        /// Called to evaluate the HTML fragment corresponding to each 
        ///     attribute's name/value in the code.
        /// </summary>
        /// <param name="match">
        /// The <see cref="Match"/> resulting from a 
        ///     single regular expression match.
        /// </param>
        /// <returns>
        /// A string containing the HTML code fragment.
        /// </returns>
        private static string AttributeMatchEval(Match match)
        {
            if (match.Groups[1].Success)
            {
                // attribute value
                return string.Format("<span class=\"kwrd\">{0}</span>", match);
            }

            if (match.Groups[2].Success)
            {
                // attribute name
                return string.Format("<span class=\"attr\">{0}</span>", match);
            }

            return match.ToString();
        }

        #endregion
    }
}