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
    public class Answer : ActiveType<Answer>
    {
        [ActiveType]
        public class Vote : ActiveType<Vote>
        {
            [ActiveField(IsOwner = false)]
            public User User { get; set; }

            [ActiveField]
            public int Points { get; set; }
        }

        public Answer()
        {
            Voters = new LazyList<Vote>();
        }

        [ActiveField]
        public string Body { get; set; }

        [ActiveField]
        public DateTime Asked { get; set; }

        [ActiveField]
        public int Votes { get; set; }

        [ActiveField(IsOwner = false)]
        public User Author { get; set; }

        [ActiveField]
        public LazyList<Vote> Voters { get; set; }
    }
}
