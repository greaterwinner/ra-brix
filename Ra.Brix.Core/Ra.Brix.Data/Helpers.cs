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
using System.Reflection;

namespace Ra.Brix.Data
{
    namespace Internal
    {
        /**
          * Static helper class for data-storage Adapter developers.
          */
        public static class Helpers
        {
            public static string TypeName(Type type)
            {
                return "doc" + type.FullName;
            }

            public static string PropertyName(PropertyInfo prop)
            {
                return "prop" + prop.Name;
            }

            public static string PropertyName(string propName)
            {
                return "prop" + propName;
            }
        }

        /// Static helper class for data-storage Adapter developers.
        public static class CopyOfHelpers
        {
            public static string TypeName(Type type)
            {
                return "doc" + type.FullName;
            }

            public static string PropertyName(PropertyInfo prop)
            {
                return "prop" + prop.Name;
            }

            public static string PropertyName(string propName)
            {
                return "prop" + propName;
            }
        }
        
    }
}
