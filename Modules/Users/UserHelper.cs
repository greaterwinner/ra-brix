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

namespace UsersModules
{
    public class UserHelper
    {
        public UserHelper(string username, DateTime lastLoggedIn, string rolesString)
        {
            Username = username;
            LastLoggedIn = lastLoggedIn;
            RolesString = rolesString;
        }

        public string Username { get; set; }
        public DateTime LastLoggedIn { get; set; }
        public string RolesString { get; set; }
    }
}
