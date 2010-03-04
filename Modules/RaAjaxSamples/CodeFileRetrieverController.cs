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

        private static void GetMarkup(string className, Node node)
        {
            string usxFile = "Docs_Controls_" + className + ".ascx";
            using (Stream stream =
                Assembly.GetExecutingAssembly()
                    .GetManifestResourceStream("RaAjaxSamples." + usxFile))
            {
                if (stream != null)
                {
                    using (TextReader reader = new StreamReader(stream))
                    {
                        node["Markup"].Value = reader.ReadToEnd();
                    }
                }
            }
        }

        private static void GetCode(string className, Node node)
        {
            string usxFile = "Docs_Controls_" + className + ".ascx.cs";
            using (Stream stream =
                Assembly.GetExecutingAssembly()
                    .GetManifestResourceStream("RaAjaxSamples.CodeFiles." + usxFile))
            {
                if (stream != null)
                {
                    using (TextReader reader = new StreamReader(stream))
                    {
                        node["Code"].Value = reader.ReadToEnd();
                    }
                }
            }
        }
    }
}
