﻿/*
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
    public class ForumPost : ActiveType<ForumPost>
    {
        public ForumPost()
        {
            Replies = new LazyList<ForumPost>();
        }

        [ActiveField]
        public string Header { get; set; }

        [ActiveField]
        public string Body { get; set; }

        [ActiveField]
        public DateTime When { get; set; }

        [ActiveField]
        public string Name { get; set; }

        [ActiveField]
        public string URL { get; set; }

        [ActiveField]
        public LazyList<ForumPost> Replies { get; set; }

        [ActiveField(IsOwner = false)]
        public User RegisteredUser { get; set; }

        [ActiveField]
        public int Score { get; set; }

        public override void Save()
        {
            if (string.IsNullOrEmpty(Name) && RegisteredUser == null)
            {
                if (!string.IsNullOrEmpty(Users.LoggedInUserName))
                {
                    RegisteredUser = User.SelectFirst(Criteria.Eq("Username", Users.LoggedInUserName));
                }
                else
                {
                    Name = "Anonymous Coward";
                }
            }
            base.Save();
        }

        public bool IsRegisteredPosting
        {
            get { return RegisteredUser != null; }
        }

        public string GetNameOfPoster()
        {
            if (RegisteredUser == null)
                return Name;
            return RegisteredUser.Username;
        }
    }
}
