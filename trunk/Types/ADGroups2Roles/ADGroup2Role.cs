/*
 * Ra-Brix - A Modular-based Framework for building 
 * Web Applications Copyright 2009 - Thomas Hansen 
 * thomas@ra-ajax.org. Unless permission is 
 * explicitly given this code is licensed under the 
 * GNU Affero GPL version 3 which can be found in the 
 * license.txt file on disc.
 * 
 */

using Ra.Brix.Data;

namespace ADGroups2RolesRecords
{
    [ActiveType]
    public class AdGroup2Role : ActiveType<AdGroup2Role>
    {
        [ActiveField]
        public string GroupName { get; set; }

        [ActiveField]
        public string RoleName { get; set; }

        public override void Save()
        {
            if (GroupName == null)
            {
                GroupName = string.Empty;
            }
            if (RoleName == null)
            {
                RoleName = string.Empty;
            }
            base.Save();
        }
    }
}