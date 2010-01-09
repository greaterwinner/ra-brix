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

namespace ForumRecords
{
    [ActiveType]
    public class Forum : ActiveType<Forum>
    {
        public Forum()
        {
            Posts = new LazyList<ForumPost>();
        }

        [ActiveField]
        public string Name { get; set; }

        [ActiveField]
        public LazyList<ForumPost> Posts { get; set; }
    }
}



