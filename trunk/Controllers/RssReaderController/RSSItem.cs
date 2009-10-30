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

namespace RssReaderController
{
    public class RssItem
    {
        private readonly Guid _id = Guid.NewGuid();

        public RssItem(string header, string body, DateTime date, string url)
        {
            Header = header;
            Body = body;
            Date = date;
            Url = url;
        }

        public Guid Id
        {
            get { return _id; }
        }

        public string Url { get; set; }

        public string Header { get; set; }

        public string Body { get; set; }

        public DateTime Date { get; set; }
    }
}
