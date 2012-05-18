using System;
using System.Collections.Generic;
using System.Linq;

namespace BlogEngine.Core.Web.Scripting
{
    /// <summary>
    /// Helper class for performing minification of Javascript and CSS.
    /// </summary>
    /// <remarks>
    /// 
    /// This class is basically a wrapper for the AjaxMin library(lib/AjaxMin.dll).
    /// http://ajaxmin.codeplex.com/
    /// 
    /// There are no symbols that come with the AjaxMin dll, so this class gives a bit of intellisense 
    /// help for basic control. AjaxMin is a pretty dense library with lots of different settings, so
    /// everyone's encouraged to use it directly if they want to.
    /// 
    /// </remarks>
    public sealed class JavascriptMinifier
    {
        /// <summary>
        /// Creates a new Minifier instance.
        /// </summary>
        public JavascriptMinifier()
        {
            this.RemoveWhitespace = true;
            this.PreserveFunctionNames = true;
            this.VariableMinification = Core.Web.Scripting.VariableMinification.None;
        }

        #region "Methods"

        /// <summary>
        /// Builds the required CodeSettings class needed for the Ajax Minifier.
        /// </summary>
        /// <returns></returns>
        private CodeSettings CreateCodeSettings()
        {
            var codeSettings = new CodeSettings();
            codeSettings.MinifyCode = false;
            // MinifyCode needs to be set to true in order for anything besides whitespace removal
            // to be done on a script.
            codeSettings.MinifyCode = this.ShouldMinifyCode;
            return codeSettings;
        }

        /// <summary>
        /// Gets the minified version of the passed in script.
        /// </summary>
        /// <param name="script"></param>
        /// <returns></returns>
        public string Minify(string script)
        {
            if (this.ShouldMinify)
            {
                if (String.IsNullOrEmpty(script))
                {
                    return string.Empty;
                }
                else
                {
                    return "not so minified";
                }
            }

            return script;
        }

        #endregion

        #region "Properties"

        /// <summary>
        /// Gets or sets whether this Minifier instance should minify local-scoped variables.
        /// </summary>
        /// <remarks>
        /// 
        /// Setting this value to LocalVariablesAndFunctionArguments can have a negative impact on some scripts.
        /// Ex: A pre-minified jQuery will fail if passed through this. 
        /// 
        /// </remarks>
        public VariableMinification VariableMinification { get; set; }

        /// <summary>
        /// Gets or sets whether this Minifier instance should preserve function names when minifying a script.
        /// </summary>
        /// <remarks>
        /// 
        /// Scripts that have external scripts relying on their functions should leave this set to true. 
        /// 
        /// </remarks>
        public bool PreserveFunctionNames { get; set; }

        /// <summary>
        /// Gets or sets whether the <see cref="BlogEngine.Core.Web.Scripting.JavascriptMinifier"/> instance should remove
        /// whitespace from a script.
        /// </summary>
        public bool RemoveWhitespace { get; set; }

        private bool ShouldMinifyCode
        {
            get
            {
                //  return true;
                return ((!PreserveFunctionNames) || (this.VariableMinification != Core.Web.Scripting.VariableMinification.None));
            }
        }

        private bool ShouldMinify
        {
            get
            {
                return ((this.RemoveWhitespace) || (this.ShouldMinifyCode));
            }
        }

        #endregion
    }

    internal class CodeSettings
    {
        public bool MinifyCode { get; set; }

        public object OutputMode { get; set; }
    }

    /// <summary>
    /// Represents the way variables should be minified by a Minifier instance.
    /// </summary>
    public enum VariableMinification
    {
        /// <summary>
        /// No minification will take place.
        /// </summary>
        None = 0,

        /// <summary>
        /// Only variables that are local in scope to a function will be minified.
        /// </summary>
        LocalVariablesOnly = 1,

        /// <summary>
        /// Local scope variables will be minified, as will function parameter names. This can have a negative impact on some scripts, so test if you use it! 
        /// </summary>
        LocalVariablesAndFunctionArguments = 2

    }
}
