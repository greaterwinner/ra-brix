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

namespace TipTheAuthorsRecords
{
    [ActiveType]
    public class ArticleTip : ActiveType<ArticleTip>
    {
        [ActiveField]
        public string URL { get; set; }

        [ActiveField]
        public DateTime TipDate { get; set; }

        [ActiveField(IsOwner = false)]
        public User SubmittedBy { get; set; }

        public override void Save()
        {
            if (ID == 0)
            {
                TipDate = DateTime.Now;
            }
            base.Save();
        }
    }
}
