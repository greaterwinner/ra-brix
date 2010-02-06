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
using UserRecords;
using HelperGlobals;
using Ra.Brix.Types;

namespace ArticlePublisherRecords
{
    [ActiveType]
    public class Article : ActiveType<Article>
    {
        public Article()
        {
            Tags = new LazyList<Tag>();
            Followers = new LazyList<User>();
        }

        [ActiveField]
        public string Header { get; set; }

        [ActiveField]
        public string Ingress { get; set; }

        [ActiveField]
        public string Body { get; set; }

        [ActiveField]
        public string URL { get; set; }

        [ActiveField]
        public string IconImage { get; set; }

        [ActiveField]
        public string MainImage { get; set; }

        [ActiveField]
        public string OriginalImage { get; set; }

        [ActiveField]
        public int ViewCount { get; set; }

        [ActiveField(IsOwner = false)]
        public LazyList<Tag> Tags { get; set; }

        [ActiveField(IsOwner = false)]
        public LazyList<User> Followers { get; set; }

        [ActiveField(IsOwner = false)]
        public User Author { get; set; }

        [ActiveField]
        public DateTime Published { get; set; }

        public static Article FindArticle(string contentId)
        {
            return SelectFirst(Criteria.Eq("URL", contentId));
        }

        public override void Save()
        {
            if (ID == 0 && string.IsNullOrEmpty(URL))
            {
                // This is a newly created root page. Hence we must give it a unique URL
                GetUniqueURL(this);
                Author = User.SelectFirst(Criteria.Eq("Username", Users.LoggedInUserName));
            }
            if (string.IsNullOrEmpty(Ingress))
            {
                if (string.IsNullOrEmpty(Body))
                {
                    throw new Exception("Cannot have both an empty body and an empty ingress");
                }
                Ingress = Body;
                if (Ingress.Length > 50)
                {
                    Ingress = Ingress.Substring(0, 50) + "...";
                    Ingress = Ingress.Replace("<", "&lt;").Replace(">", "&gt;");
                }
            }
            base.Save();
        }

        private static void GetUniqueURL(Article page)
        {
            string url = page.Header.ToLower();
            if (url.Length > 35)
                url = url.Substring(0, 35);
            int index = 0;
            while (index < url.Length)
            {
                if (("abcdefghijklmnopqrstuvwxyz0123456789").IndexOf(url[index]) == -1)
                {
                    url = url.Substring(0, index) + "-" + url.Substring(index + 1);
                }
                index += 1;
            }
            url = url.Trim('-');
            bool found = true;
            while (found)
            {
                found = false;
                if (url.IndexOf("--") != -1)
                {
                    url = url.Replace("--", "-");
                    found = true;
                }
            }
            int countOfOldWithSameURL = CountWhere(Criteria.Like("URL", url));
            if (countOfOldWithSameURL > 0)
            {
                while (true)
                {
                    if (CountWhere(Criteria.Like("URL", (url + countOfOldWithSameURL))) > 0)
                    {
                        countOfOldWithSameURL += 1;
                    }
                    else
                        break;
                }
                url += (countOfOldWithSameURL).ToString();
            }
            page.URL = url;
        }
    }
}
