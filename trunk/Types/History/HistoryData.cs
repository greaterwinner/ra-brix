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

namespace HistoryRecords
{
    [ActiveRecord]
    public class HistoryData : ActiveRecord<HistoryData>
    {
        [ActiveField]
        public string MenuEventName { get; set; }

        [ActiveField]
        public string Params { get; set; }

        [ActiveField]
        public string MenuText { get; set; }

        [ActiveField]
        public DateTime WhenRaised { get; private set; }

        [ActiveField]
        public string Username { get; set; }

        public override void Save()
        {
            if (ID == 0)
                WhenRaised = DateTime.Now;
            base.Save();
        }
    }
}