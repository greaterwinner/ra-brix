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

namespace RaAjaxSamples
{
    [ActiveController]
    public class CodeFileRetrieverController
    {
        [ActiveEvent(Name = "DoxygenDotNetGetClassCodeFile")]
        protected void DoxygenDotNetGetClassCodeFile(object sender, ActiveEventArgs e)
        {
            string className = e.Params["ClassName"].Get<string>();
            string usxFile = "Docs_Controls_" + className + ".ascx";
            using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("RaAjaxSamples." + usxFile))
            {
                using (TextReader reader = new StreamReader(stream))
                {
                    e.Params["Markup"].Value = reader.ReadToEnd();
                }
            }
        }
    }
}
