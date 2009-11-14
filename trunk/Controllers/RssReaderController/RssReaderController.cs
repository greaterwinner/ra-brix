/*
 * Ra-Brix - A Modular-based Framework for building 
 * Web Applications Copyright 2009 - Thomas Hansen 
 * thomas@ra-ajax.org. Unless permission is 
 * explicitly given this code is licensed under the 
 * GNU Affero GPL version 3 which can be found in the 
 * license.txt file on disc.
 * 
 */

using LanguageRecords;
using Ra.Brix.Data;
using Ra.Brix.Loader;
using Ra.Brix.Types;

namespace RssReaderController
{
    [ActiveController]
    public class RssReaderController
    {
        [ActiveEvent(Name = "ApplicationStartup")]
        protected static void ApplicationStartup2(object sender, ActiveEventArgs e)
        {
            Language.Instance.SetDefaultValue("ButtonRssConfigure", "Configure RSS");
            Language.Instance.SetDefaultValue("ButtonViewRSS", "View RSS");
        }

        [ActiveEvent(Name = "GetMenuItems")]
        protected void GetMenuItems(object sender, ActiveEventArgs e)
        {
            e.Params["ButtonAdmin"]["ButtonRssConfigure"].Value = "Menu-RssConfigure";
            e.Params["ButtonAppl"]["ButtonViewRSS"].Value = "Menu-ViewRSS";
        }

        [ActiveEvent(Name = "Menu-RssConfigure")]
        protected void RssConfigure(object sender, ActiveEventArgs e)
        {
            Node node = new Node();
            node["TabCaption"].Value = 
                Language.Instance["ConfigureRssModule", null, "Configure RSS module"];

            node["ModuleSettings"]["Grid"]["Columns"]["URL"]["Caption"].Value = Language.Instance["URL", null, "URL"];
            node["ModuleSettings"]["Grid"]["Columns"]["URL"]["ControlType"].Value = "InPlaceEdit";

            int idxNo = 0;
            foreach (RssReaderRecords.RssItem idx in ActiveType<RssReaderRecords.RssItem>.Select())
            {
                node["ModuleSettings"]["Grid"]["Rows"]["Row" + idxNo]["ID"].Value = idx.ID;
                node["ModuleSettings"]["Grid"]["Rows"]["Row" + idxNo]["URL"].Value = idx.URL;
                idxNo += 1;
            }

            ActiveEvents.Instance.RaiseLoadControl(
                "RssReaderModules.ConfigureRssReader",
                "dynMid",
                node);
        }

        [ActiveEvent(Name = "Menu-ViewRSS")]
        protected void ViewRSS(object sender, ActiveEventArgs e)
        {
            Node node = new Node();
            node["TabCaption"].Value = Language.Instance["NewsFromTheWorld", null, "News from the world"];
            int idxNo = 0;
            foreach (Rss idx in RssDatabase.Database)
            {
                node["ModuleSettings"]["Items"]["Item" + idxNo]["Caption"].Value = idx.Title;
                node["ModuleSettings"]["Items"]["Item" + idxNo]["Link"].Value = idx.WebLink;
                node["ModuleSettings"]["Items"]["Item" + idxNo]["ID"].Value = idx.Id;
                int idxNoItem = 0;
                foreach (RssItem idxItem in idx.Items)
                {
                    node["ModuleSettings"]["Items"]["Item" + idxNo]["Items"]["Item" + idxNoItem]["Caption"].Value = idxItem.Header;
                    node["ModuleSettings"]["Items"]["Item" + idxNo]["Items"]["Item" + idxNoItem]["Body"].Value = idxItem.Body;
                    node["ModuleSettings"]["Items"]["Item" + idxNo]["Items"]["Item" + idxNoItem]["Date"].Value = idxItem.Date;
                    node["ModuleSettings"]["Items"]["Item" + idxNo]["Items"]["Item" + idxNoItem]["URL"].Value = idxItem.Url;
                    idxNoItem += 1;
                }
                idxNo += 1;
            }
            ActiveEvents.Instance.RaiseLoadControl(
                "RssReaderModules.ViewRSSItems",
                "dynMid",
                node);
        }

        [ActiveEvent(Name = "DeleteRSSItem")]
        protected static void DeleteRSSItem(object sender, ActiveEventArgs e)
        {
            RssReaderRecords.RssItem item = ActiveType<RssReaderRecords.RssItem>.SelectByID(int.Parse(e.Params["ID"].Get<string>()));
            item.Delete();
            RssDatabase.Reset();
        }

        [ActiveEvent(Name = "EditRSSItemURL")]
        protected static void EditRSSItemURL(object sender, ActiveEventArgs e)
        {
            RssReaderRecords.RssItem item = ActiveType<RssReaderRecords.RssItem>.SelectByID(int.Parse(e.Params["ID"].Get<string>()));
            item.URL = e.Params["URL"].Get<string>();
            item.Save();
            RssDatabase.Reset();
        }

        [ActiveEvent(Name = "AddNewRSSItem")]
        protected static void AddNewRSSItem(object sender, ActiveEventArgs e)
        {
            RssReaderRecords.RssItem i = new RssReaderRecords.RssItem {URL = ""};
            i.Save();
            RssDatabase.Reset();

            e.Params["Grid"]["Columns"]["URL"]["Caption"].Value = Language.Instance["URL", null, "URL"];
            e.Params["Grid"]["Columns"]["URL"]["ControlType"].Value = "InPlaceEdit";

            int idxNo = 0;
            foreach (RssReaderRecords.RssItem idx in ActiveType<RssReaderRecords.RssItem>.Select())
            {
                e.Params["Grid"]["Rows"]["Row" + idxNo]["ID"].Value = idx.ID;
                e.Params["Grid"]["Rows"]["Row" + idxNo]["URL"].Value = idx.URL;
                idxNo += 1;
            }
        }

        [ActiveEvent(Name = "ApplicationStartup")]
        protected static void ApplicationStartup(object sender, ActiveEventArgs e)
        {
            if (ActiveType<RssReaderRecords.RssItem>.Count == 0)
            {
                RssReaderRecords.RssItem item = new RssReaderRecords.RssItem
                {
                    URL = "http://ra-ajax.org/thomas.blogger?rss=true"
                };
                item.Save();
            }
        }
    }
}



















