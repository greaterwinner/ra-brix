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
using LanguageRecords;
using Ra.Brix.Types;
using System.Collections.Generic;
using HelperGlobals;
using System.Web;

namespace SitemapController
{
    [ActiveController]
    public class SitemapController
    {
        [ActiveEvent(Name = "ApplicationStartup")]
        protected static void ApplicationStartup(object sender, ActiveEventArgs e)
        {
            Language.Instance.SetDefaultValue("ButtonSitemap", "Sitemap");
        }

        [ActiveEvent(Name = "GetMenuItems")]
        protected static void GetMenuItems(object sender, ActiveEventArgs e)
        {
            e.Params["ButtonSitemap"].Value = "url:~/sitemap";
        }

        [ActiveEvent(Name = "Page_Init_InitialLoading")]
        protected void InitialLoadingOfPage(object sender, ActiveEventArgs e)
        {
            string contentId = HttpContext.Current.Request.Params["ContentID"];
            if (contentId != null)
            {
                contentId = contentId.Trim('/');
            }
            if (contentId == "sitemap")
            {
                Node node = new Node();
                ActiveEvents.Instance.RaiseActiveEvent(
                    this,
                    "GetMenuItems",
                    node);
                ActiveEvents.Instance.RaiseActiveEvent(
                    this,
                    "FilterMenuItems",
                    node);
                List<Tuple<string, string>> list = new List<Tuple<string, string>>();
                foreach (Node idx in node)
                {
                    RetrieveItems(idx, list);
                }
                Node n = new Node();
                int idxNo = 0;
                foreach (Tuple<string, string> idx in list)
                {
                    n["ModuleSettings"]["Items"]["I" + idxNo]["Name"].Value = Language.Instance[idx.Left];
                    n["ModuleSettings"]["Items"]["I" + idxNo]["URL"].Value = idx.Right;
                    idxNo += 1;
                }
                ActiveEvents.Instance.RaiseLoadControl(
                    "SitemapModules.ShowSitemap",
                    "dynMid",
                    n);
            }
        }

        private void RetrieveItems(Node node, List<Tuple<string, string>> list)
        {
            if (node.Value != null && node.Get<string>().IndexOf("url:") == 0)
            {
                string name = node.Name;
                string url = node.Get<string>().Replace("url:", "");
                url = url.Replace("~", ApplicationRoot.Root);
                Tuple<string, string> tmp = new Tuple<string, string>(name, url);
                list.Add(tmp);
            }
            foreach (Node idx in node)
            {
                RetrieveItems(idx, list);
            }
        }
    }
}
