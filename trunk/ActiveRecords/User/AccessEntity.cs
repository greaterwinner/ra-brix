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

namespace UserRecords
{
    [ActiveRecord]
    public class AccessEntity : ActiveRecord<AccessEntity>
    {
        [ActiveField]
        public string RoleName { get; set; }

        [ActiveField]
        public string MenuValue { get; set; }

        [ActiveField]
        public string Params { get; set; }
    }
}