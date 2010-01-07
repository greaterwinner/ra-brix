/*
 * Ra-Brix - A Modular-based Framework for building 
 * Web Applications Copyright 2009 - Thomas Hansen 
 * thomas@ra-ajax.org. Unless permission is 
 * explicitly given this code is licensed under the 
 * GNU Affero GPL version 3 which can be found in the 
 * license.txt file on disc.
 * 
 */

using System.Web;
using Ra.Brix.Loader;
using Ra.Brix.Types;
using SettingsRecords;
using ArticlePublisherRecords;
using Ra.Brix.Data;

namespace ArticlePublisherController
{
    [ActiveController]
    public class ArticlePublisherController
    {
        [ActiveEvent(Name = "Page_Init_InitialLoading")]
        protected void InitialLoadingOfPage(object sender, ActiveEventArgs e)
        {
            string contentId = HttpContext.Current.Request.Params["ContentID"];
            if (contentId != null)
            {
                contentId = contentId.Trim('/');
            }
            if (contentId == null)
            {
                if (Settings.Instance["ArticlePublisherHideLandingPage"] == "True")
                    return;
                Node node = new Node();
                int idxNo = 0;
                foreach (Article idx in Article.Select(Criteria.Sort("Published", false)))
                {
                    // Making sure we only show the 10 latest articles...
                    if (++idxNo == 10)
                        break;
                }
                ActiveEvents.Instance.RaiseLoadControl(
                    "ArticlePublisherModules.LandingPage",
                    "dynMid",
                    node);
            }
        }
    }
}
