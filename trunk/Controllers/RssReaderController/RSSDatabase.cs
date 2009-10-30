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
using System.Web;
using Ra.Brix.Data;

namespace RssReaderController
{
    public sealed class RssDatabase
    {
        public static List<Rss> Database
        {
            get
            {
                if (HttpContext.Current.Application["RssReaderController.RssDatabase.Database"] == null)
                {
                    List<Rss> tmp = new List<Rss>();
                    foreach (RssReaderRecords.RssItem idx in ActiveRecord<RssReaderRecords.RssItem>.Select())
                    {
                        tmp.Add(new Rss(idx.URL));
                    }
                    HttpContext.Current.Application["RssReaderController.RssDatabase.Database"] = tmp;
                }
                List<Rss> items = (List<Rss>)HttpContext.Current.Application["RssReaderController.RssDatabase.Database"];
                foreach (Rss idx in items)
                {
                    if ((DateTime.Now - idx.InitializedDate).TotalMinutes > 30)
                    {
                        idx.Inititalize();
                    }
                }
                return items;
            }
        }

        public static void Reset()
        {
            HttpContext.Current.Application["RssReaderController.RssDatabase.Database"] = null;
        }

        public static RssItem FindItem(Guid id)
        {
            foreach (Rss idx in Database)
            {
                RssItem tmp = idx.Items.Find(
                    delegate(RssItem i)
                    {
                        return i.Id == id;
                    });
                if (tmp != null)
                    return tmp;
            }
            return null;
        }
    }
}