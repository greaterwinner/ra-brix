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
         * If true, method will be called asynchronous. Note that if you use Async event handlers 
         * then you **CANNOT** in ANY ways access any parts of the Page, Response, Request, HttpContext
         * etc since these might very weel be discarded. You can also obviously NOT write to the
         * response at all using Async event handlers. Do not LoadControls or anything else like
         * that. Async event handlers are PURELY meant for "fire and forget" event handlers, and
         * in general terms you should in fact try to avoid them all together if you can, due to
         * these problems.
         */
        public bool Async;

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

        /**
         * CTOR taking the name of the event you want your method to catch, plus a boolean indicating
         * if the event handler should be called asynchronously.
         */
        public ActiveEventAttribute(string name, bool async)
        {
            Name = name;
            Async = async;
        }
    }
}
