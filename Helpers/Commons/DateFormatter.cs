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
namespace HelperGlobals
{
    public class DateFormatter
    {
        public static string FormatDate(DateTime date)
        {
            TimeSpan span = DateTime.Now - date;
            if (span.TotalSeconds < 60D)
            {
                return ((int)span.TotalSeconds).ToString() + " seconds ago";
            }
            else if (span.TotalMinutes < 60D)
            {
                return ((int)span.TotalMinutes).ToString() + " minutes ago";
            }
            else if (span.TotalHours < 24D)
            {
                return ((int)span.TotalHours).ToString() + " hours ago";
            }
            else if (span.TotalDays < 7D)
            {
                return ((int)span.TotalDays).ToString() + " days ago";
            }
            return date.ToString("ddd d.MMM yy");
        }
    }
}
