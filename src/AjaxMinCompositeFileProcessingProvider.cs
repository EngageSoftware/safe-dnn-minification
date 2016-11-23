// <copyright file="AjaxMinCompositeFileProcessingProvider.cs" company="Engage Software">
// Copyright (c) 2015
// by Engage Software ( http://www.engagesoftware.com )
// </copyright>
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
// TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
// THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
// CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
// DEALINGS IN THE SOFTWARE.

namespace Engage.Dnn
{
    using System.Linq;

    using ClientDependency.Core;
    using ClientDependency.Core.CompositeFiles.Providers;

    using DotNetNuke.Web.Client;

    using JetBrains.Annotations;

    using Microsoft.Ajax.Utilities;

    /// <summary>A provider for combining, minifying, compressing and saving composite scripts/css files</summary>
    [PublicAPI]
    public class AjaxMinCompositeFileProcessingProvider : CompositeFileProcessingProvider
    {
        /// <summary>Settings for minifying CSS</summary>
        private static readonly CssSettings CssSettings = new CssSettings { TermSemicolons = true, CommentMode = CssComment.Hacks, };

        /// <summary>Settings for minifying JavaScript code</summary>
        private static readonly CodeSettings JsSettings = new CodeSettings { TermSemicolons = true, EvalTreatment = EvalTreatment.MakeAllSafe, InlineSafeStrings = false, };

        /// <summary>The minifier</summary>
        private readonly Minifier minifier = new Minifier();

        /// <summary>The client resource settings</summary>
        private readonly ClientResourceSettings clientResourceSettings = new ClientResourceSettings();

        /// <summary>Gets a value indicating whether CSS is configured to be minified.</summary>
        private bool MinifyCss => this.clientResourceSettings.EnableCssMinification() ?? this.EnableCssMinify;

        /// <summary>Gets a value indicating whether JavaScript is configured to be minified.</summary>
        private bool MinifyJs => this.clientResourceSettings.EnableJsMinification() ?? this.EnableJsMinify;

        /// <summary>Minifies the given <paramref name="fileContents"/>.</summary>
        /// <param name="fileContents">The contents of the file.</param>
        /// <param name="type">The type of the resource.</param>
        /// <returns>The minified or full contents of <paramref name="fileContents"/>, based on the minification configuration.</returns>
        public override string MinifyFile(string fileContents, ClientDependencyType type)
        {
            if (type == ClientDependencyType.Css && this.MinifyCss)
            {
                var minifiedCss = this.minifier.MinifyStyleSheet(fileContents, CssSettings);
                if (!this.minifier.ErrorList.Any(error => error.IsError))
                {
                    return minifiedCss;
                }
            }

            if (type == ClientDependencyType.Javascript && this.MinifyJs)
            {
                var minifiedScript = this.minifier.MinifyJavaScript(fileContents, JsSettings);
                if (!this.minifier.ErrorList.Any(error => error.IsError))
                {
                    return minifiedScript;
                }
            }

            return fileContents;
        }
    }
}
