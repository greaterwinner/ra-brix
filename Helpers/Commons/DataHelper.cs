/*
 * Ra-Brix - A Modular-based Framework for building 
 * Web Applications Copyright 2009 - Thomas Hansen 
 * thomas@ra-ajax.org. Unless permission is 
 * explicitly given this code is licensed under the 
 * GNU Affero GPL version 3 which can be found in the 
 * license.txt file on disc.
 * 
 */

using System.Collections.Generic;
using Ra.Brix.Data;

namespace HelperGlobals
{
    public static class DataHelper
    {
        public static IEnumerable<Criteria> CreateSearchFilter(string propertyName, string filter)
        {
            string[] parts = filter.Split(' ');
            foreach (string idx in parts)
            {
                string idxText = idx;
                if (idxText.Length > 0)
                {
                    if (idxText[0] != '%')
                        idxText = "%" + idxText;
                    if (idxText[idxText.Length - 1] != '%')
                        idxText += "%";
                    yield return Criteria.Like(propertyName, idxText);
                }
            }
        }
    }
}
