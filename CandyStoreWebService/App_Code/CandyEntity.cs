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
namespace CandyStoreEntities
{
    public class CandyEntity
    {
        private string _candyName;
        private string _candyImageUrl;
        private string _description;
        private DateTime _date;

        public string CandyName
        {
            get { return _candyName; }
            set { _candyName = value; }
        }

        public string CandyImageUrl
        {
            get { return _candyImageUrl; }
            set { _candyImageUrl = value; }
        }

        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }

        public DateTime Updated
        {
            get { return _date; }
            set { _date = value; }
        }
    }
}