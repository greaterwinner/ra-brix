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

namespace StackedRecords
{
    [ActiveType]
    public class Question : ActiveType<Question>
    {
        [ActiveField]
        public string Header { get; set; }

        [ActiveField]
        public string URL { get; set; }

        [ActiveField]
        public string Body { get; set; }

        [ActiveField]
        public DateTime Asked { get; set; }

        [ActiveField]
        public DateTime LastAnswer { get; set; }

        [ActiveField(IsOwner = false)]
        public User Author { get; set; }

        [ActiveField]
        public LazyList<Answer> Answers { get; set; }

        public static Question FindArticle(string contentId)
        {
            return SelectFirst(Criteria.Eq("URL", contentId));
        }

        public override void Save()
        {
            if (ID == 0 && string.IsNullOrEmpty(URL) && Author == null)
            {
                // This is a newly created root page. Hence we must give it a unique URL
                GetUniqueURL(this);
                Author = User.SelectFirst(Criteria.Eq("Username", Users.LoggedInUserName));
                Asked = DateTime.Now;
                LastAnswer = Asked;
            }
            if (string.IsNullOrEmpty(Body))
            {
                throw new Exception("Cannot have a question without a body");
            }
            base.Save();
        }

        private static void GetUniqueURL(Question page)
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
            url = "stacked/" + url;
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
