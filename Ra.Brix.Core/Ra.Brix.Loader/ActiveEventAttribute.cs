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

namespace Ra.Brix.Loader
{
    /**
     * Mark your methods with this attribute to make then catch Ra.Brix Active Events. 
     * The Name property is the second argument to the RaiseEvent, or the "name" of the 
     * event being raised. You can mark your methods with multiple instances of this 
     * attribute to catch multiple events in the same event handler.
     */
    [AttributeUsage(AttributeTargets.Method, AllowMultiple=true, Inherited=true)]
    public class ActiveEventAttribute : Attribute
    {
        /**
         * Name of event
         */
        public string Name;

        /**
         * Empty CTOR
         */
        public ActiveEventAttribute()
        { }

        /**
         * CTOR taking the name of the event you want your method to catch.
         */
        public ActiveEventAttribute(string name)
        {
            Name = name;
        }
    }
}
