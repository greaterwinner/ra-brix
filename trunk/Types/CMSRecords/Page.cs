/*
 * Ra-Brix - A Modular-based Framework for building 
 * Web Applications Copyright 2009 - Thomas Hansen 
 * thomas@ra-ajax.org. Unless permission is 
 * explicitly given this code is licensed under the 
 * GNU Affero GPL version 3 which can be found in the 
 * license.txt file on disc.
 * 
 */

using Ra.Brix.Data;
using Ra.Brix.Types;

namespace CMSRecords
{
    [ActiveType]
    public class Page : ActiveType<Page>
    {
        public Page()
        {
            Children = new LazyList<Page>();
        }

        [ActiveField]
        public string Header { get; set; }

        [ActiveField]
        public string Body { get; set; }

        [ActiveField]
        public string URL { get; set; }

        [ActiveField]
        public LazyList<Page> Children { get; set; }

        public static Page FindPage(string contentId)
        {
            if (string.IsNullOrEmpty(contentId))
            {
                return SelectFirst(Criteria.Eq("URL", "home"));
            }
            return SelectFirst(Criteria.Eq("URL", contentId));
        }

        public override void Save()
        {
            CreateUniqueURLForNewChildren();
            if (ID == 0 && string.IsNullOrEmpty(URL))
            {
                // This is a newly created root page. Hence we must give it a unique URL
                GetUniqueURL(this, null);
            }
            base.Save();
        }

        private void CreateUniqueURLForNewChildren()
        {
            foreach (Page idxChild in Children)
            {
                if (idxChild.ID == 0 && string.IsNullOrEmpty(idxChild.URL))
                {
                    GetUniqueURL(idxChild, this);
                }
            }
        }

        private static void GetUniqueURL(Page page, Page parent)
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
            if (parent != null)
            {
                url = parent.URL + "/" + url;
                
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
