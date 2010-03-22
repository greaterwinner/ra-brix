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

namespace UserRecords
{
    [ActiveType]
    public class User : ActiveType<User>
    {
        public User()
        {
            Roles = new LazyList<Role>();
        }

        [ActiveField]
        public string Username { get; set; }

        [ActiveField]
        public string Email { get; set; }

        [ActiveField]
        public string Phone { get; set; }

        [ActiveField]
        public int Score { get; set; }

        [ActiveField]
        public string Biography { get; set; }

        [ActiveField]
        public DateTime LastLoggedIn { get; set; }

        [ActiveField(IsOwner = false)]
        public LazyList<Role> Roles { get; set; }

        public bool InRole(string roleName)
        {
            foreach (Role idx in Roles)
            {
                if (idx.Name == roleName)
                    return true;
            }
            return false;
        }

        public string GetRolesString()
        {
            string retVal = "";
            foreach (Role idx in Roles)
            {
                if (!string.IsNullOrEmpty(retVal))
                    retVal += ", ";
                retVal += idx.Name;
            }
            return retVal;
        }

        public override void Save()
        {
            if (ID == 0)
            {
                // Checking to see that this username is not taken...!
                // Usernames are UNIQUE...!
                if (CountWhere(Criteria.Eq("Username", Username)) > 0)
                {
                    throw new ApplicationException("Username; " + Username + " is already taken by another user...");
                }
            }
            if (Score <= -1000)
            {
                if (!InRole("Blocked"))
                {
                    Roles.Add(Role.SelectFirst(Criteria.Eq("Name", "Blocked")));
                }
            }
            base.Save();
        }
    }
}