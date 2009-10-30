/*
 * Ra-Brix - A Modular-based Framework for building 
 * Web Applications Copyright 2009 - Thomas Hansen 
 * thomas@ra-ajax.org. Unless permission is 
 * explicitly given this code is licensed under the 
 * GNU Affero GPL version 3 which can be found in the 
 * license.txt file on disc.
 * 
 */

using System;
using System.Collections.Generic;
using System.Xml;
using System.Net;

namespace RssReaderController
{
    public class Rss
    {
        private string _url;
        private readonly List<RssItem> _items = new List<RssItem>();
        private readonly Guid _id = Guid.NewGuid();

        public Rss(string url)
        {
            Url = url;
        }

        public Guid Id
        {
            get { return _id; }
        }

        public string Url
        {
            get { return _url; }
            set { _url = value; Inititalize(); }
        }

        public DateTime InitializedDate { get; set; }

        public string Title { get; private set; }

        public string WebLink { get; private set; }

        public List<RssItem> Items
        {
            get { return _items; }
        }

        public void Inititalize()
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                HttpWebRequest req = WebRequest.Create(_url) as HttpWebRequest;
                if (req != null)
                {
                    HttpWebResponse res = req.GetResponse() as HttpWebResponse;
                    if (res != null)
                        doc.Load(res.GetResponseStream());
                }

                if (doc.DocumentElement == null)
                {
                    throw new ArgumentException("Couldn't download document: " + _url);
                }

                Title = doc.DocumentElement.SelectNodes("channel/title")[0].InnerText;
                WebLink = doc.DocumentElement.SelectNodes("channel/link")[0].InnerText;

                XmlNodeList list = doc.DocumentElement.SelectNodes("channel/item");
                if (list != null)
                {
                    foreach (XmlElement idx in list)
                    {
                        if (idx == null)
                            continue;
                        string title = idx.SelectNodes("title")[0].InnerText;
                        DateTime date = DateTime.Parse(idx.SelectNodes("pubDate")[0].InnerText.Replace(" +0000", ""));
                        string body = idx.SelectNodes("description")[0].InnerText;
                        string url = idx.SelectNodes("guid").Count > 0 ?
                            idx.SelectNodes("guid")[0].InnerText :
                            idx.SelectNodes("link")[0].InnerText;
                        Items.Add(new RssItem(title, body, date, url));
                    }
                }
                InitializedDate = DateTime.Now;
            }
            catch
            {
                ;// Intentionally do nothing...
            }
        }
    }
}
