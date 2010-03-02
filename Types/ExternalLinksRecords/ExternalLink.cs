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

namespace ExternalLinksRecords
{
    [ActiveType]
    public class ExternalLink : ActiveType<ExternalLink>
    {
        [ActiveField]
        public string URL { get; set; }

        [ActiveField]
        public string Name { get; set; }

        public override void Save()
        {
            if (string.IsNullOrEmpty(URL))
                URL = "null";
            if (string.IsNullOrEmpty(Name))
                Name = "null";
            base.Save();
        }
    }
}
