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
using Ra.Brix.Data;

namespace ArticlePublisherRecords
{
    [ActiveType]
    public class Article : ActiveType<Article>
    {
        [ActiveField]
        public string Header { get; set; }

        [ActiveField]
        public string URL { get; set; }

        [ActiveField]
        public string Body { get; set; }

        [ActiveField]
        public DateTime Published { get; set; }

        public static Article FindArticle(string contentId)
        {
            return SelectFirst(Criteria.Eq("URL", contentId));
        }
    }
}
