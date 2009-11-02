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

namespace ChatRecords
{
    [ActiveType]
    public class ChatMessage : ActiveType<ChatMessage>
    {
        [ActiveField]
        public string SentByUsername { get; set; }

        [ActiveField]
        public string SentToUsername { get; set; }

        [ActiveField]
        public string Message { get; set; }

        [ActiveField]
        public DateTime When { get; set; }

        public override void Save()
        {
            When = DateTime.Now;
            base.Save();
        }
    }
}