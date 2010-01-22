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
using Ra.Brix.Types;
using UserRecords;
using HelperGlobals;

namespace ForumRecords
{
    [ActiveType]
    public class ForumPostVote : ActiveType<ForumPostVote>
    {
        [ActiveField(IsOwner = false)]
        public ForumPost ForumPost { get; set; }

        [ActiveField(IsOwner = false)]
        public User User { get; set; }

        [ActiveField]
        public int Score { get; set; }
    }
}
