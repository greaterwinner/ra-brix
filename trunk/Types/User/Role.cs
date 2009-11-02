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

namespace UserRecords
{
    [ActiveType]
    public class Role : ActiveType<Role>
    {
        [ActiveField]
        public string Name { get; set; }

        public override void Save()
        {
            Role tmp = SelectFirst(Criteria.Eq("Name", Name));
            if (tmp != null && tmp.ID != ID)
                throw new ApplicationException("You cannot have two roles with the same name");
            base.Save();
        }
    }
}