/*
 * Ra-Brix - A Modular-based Framework for building 
 * Web Applications Copyright 2009 - Thomas Hansen 
 * thomas@ra-ajax.org. Unless permission is 
 * explicitly given this code is licensed under the 
 * GNU Affero GPL version 3 which can be found in the 
 * license.txt file on disc.
 * 
 */

using Ra.Brix.Loader;
using System.Reflection;
using System.IO;
using Ra.Brix.Types;
using ColorizerLibrary;
using System.Collections.Generic;

namespace RaAjaxSamples
{
    [ActiveController]
    public class CodeFileRetrieverController
    {
        [ActiveEvent(Name = "DoxygenDotNetGetClassNameForSample")]
        protected void DoxygenDotNetGetClassNameForSample(object sender, ActiveEventArgs e)
        {
            using (Stream stream =
                Assembly.GetExecutingAssembly()
                    .GetManifestResourceStream("RaAjaxSamples.Docs_Controls_" + 
                    e.Params["ClassOriginalName"].Get<string>() + ".ascx"))
            {
                if (stream != null)
                    e.Params["ClassName"].Value = "RaAjaxSamples.Docs_Controls_" +
                        e.Params["ClassOriginalName"].Get<string>();
                else
                    e.Params["ClassName"].Value = null;
            }
        }

        [ActiveEvent(Name = "DoxygenDotNetCheckIfSample")]
        protected void DoxygenDotNetCheckIfSample(object sender, ActiveEventArgs e)
        {
            using (Stream stream =
                Assembly.GetExecutingAssembly()
                    .GetManifestResourceStream("RaAjaxSamples.Docs_Controls_" +
                    e.Params["ClassName"].Get<string>() + ".ascx"))
            {
                if (stream != null)
                    e.Params["HasSample"].Value = true;
                else
                    e.Params["HasSample"].Value = false;
            }
        }

        [ActiveEvent(Name = "DoxygenDotNetGetClassCodeFiles")]
        protected void DoxygenDotNetGetClassCodeFiles(object sender, ActiveEventArgs e)
        {
            string className = e.Params["ClassName"].Get<string>();
            GetMarkup(className, e.Params);
            GetCode(className, e.Params);
        }

        private static Dictionary<string, string> _markupCode = new Dictionary<string, string>();
        private static void GetMarkup(string className, Node node)
        {
            string usxFile = "Docs_Controls_" + className + ".ascx";
            if (!_markupCode.ContainsKey(usxFile))
            {
                using (Stream stream =
                    Assembly.GetExecutingAssembly()
                        .GetManifestResourceStream("RaAjaxSamples." + usxFile))
                {
                    if (stream != null)
                    {
                        using (TextReader reader = new StreamReader(stream))
                        {
                            string code = reader.ReadToEnd();
                            CodeColorizer colorizer = ColorizerLibrary.Config.DOMConfigurator.Configure();
                            code =
                                colorizer.ProcessAndHighlightText(
                                    "<pre lang=\"xml\">" +
                                    code +
                                    "</pre>")
                                    .Replace("%@", "<span class=\"yellow-code\">%@</span>")
                                    .Replace(" %", " <span class=\"yellow-code\">%</span>");
                            _markupCode[usxFile] = code;
                        }
                    }
                }
            }
            if (_markupCode.ContainsKey(usxFile))
                node["Markup"].Value = _markupCode[usxFile];
        }

        private static Dictionary<string, string> _codeCode = new Dictionary<string, string>();
        private static void GetCode(string className, Node node)
        {
            string usxFile = "Docs_Controls_" + className + ".ascx.cs";
            if (!_codeCode.ContainsKey(usxFile))
            {
                using (Stream stream =
                    Assembly.GetExecutingAssembly()
                        .GetManifestResourceStream("RaAjaxSamples.CodeFiles." + usxFile))
                {
                    if (stream != null)
                    {
                        using (TextReader reader = new StreamReader(stream))
                        {
                            string code = reader.ReadToEnd();
                            CodeColorizer colorizer = ColorizerLibrary.Config.DOMConfigurator.Configure();
                            code =
                                colorizer.ProcessAndHighlightText(
                                    "<pre lang=\"cs\">" +
                                    code +
                                    "</pre>");
                            _codeCode[usxFile] = code;
                        }
                    }
                }
            }
            if (_codeCode.ContainsKey(usxFile))
                node["Code"].Value = _codeCode[usxFile];
        }
    }
}
