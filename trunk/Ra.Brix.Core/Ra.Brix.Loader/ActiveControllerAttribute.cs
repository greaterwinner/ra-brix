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
     * Mark your controllers with this Attribute. Notice that an Active Controller must
     * have a default constructor taking zero parameters. This constructor should also
     * ideally execute FAST since all controllers in your Ra-Brix project will be 
     * instantiated once every request.
     */
    public class ActiveControllerAttribute : Attribute
    {
    }
}
