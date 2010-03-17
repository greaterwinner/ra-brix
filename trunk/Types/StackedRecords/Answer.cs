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

namespace StackedRecords
{
    [ActiveType]
    public class Answer : ActiveType<Answer>
    {
        [ActiveField]
        public string Body { get; set; }

        [ActiveField]
        public DateTime Asked { get; set; }

        [ActiveField(IsOwner=false)]
        public User Author { get; set; }
    }
}
