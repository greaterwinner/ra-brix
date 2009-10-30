/*
 * Ra-Brix - A Modular-based Framework for building 
 * Web Applications Copyright 2009 - Thomas Hansen 
 * thomas@ra-ajax.org. Unless permission is 
 * explicitly given this code is licensed under the 
 * GNU Affero GPL version 3 which can be found in the 
 * license.txt file on disc.
 * 
 */

using Ra.Brix.Types;

namespace Ra.Brix.Loader
{
    /**
     * Optional interface you can mark your Modules with. If you do your Modules will be called the
     * first time they load through the InitialLoading with whatever object you choose to RaiseYour 
     * events with.
     */
    public interface IModule
    {
        /**
         * Will be called when the Module is initially loaded with the initializationObject
         * parameter you pass into your LoadModule - if any.
         */
        void InitialLoading(Node node);

        // TODO: Remove....?
        string GetCaption();
    }
}
