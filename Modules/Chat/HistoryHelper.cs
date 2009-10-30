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

namespace ChatModules
{
    internal class HistoryHelper
    {
        public HistoryHelper(string sentBy, DateTime when, string message)
        {
            SentBy = sentBy;
            When = when;
            Message = message;
        }

        public string SentBy { get; set; }
        public DateTime When { get; set; }
        public string Message { get; set; }
    }
}
