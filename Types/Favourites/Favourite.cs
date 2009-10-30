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

namespace FavouritesRecords
{
    [ActiveRecord]
    public class Favourite : ActiveRecord<Favourite>
    {
        [ActiveField]
        public string EventName { get; set; }

        [ActiveField]
        public string Name { get; set; }

        [ActiveField]
        public string Username { get; set; }

        [ActiveField]
        public string Param { get; set; }
    }
}