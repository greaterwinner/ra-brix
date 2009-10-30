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

namespace Ra.Brix.Data
{
    /**
     * Mark your well known types or entity types with this attribute to make them serializable.
     * In addition you must inherit from ActiveRecord with the type of the type you're creating
     * as the generic type argument. Notice that this attribute is for classes, you still need to
     * mark every property that you wish to serialize with the ActiveFieldAttribute.
     */
    [AttributeUsage(AttributeTargets.Class, AllowMultiple=false)]
    public class ActiveRecordAttribute : Attribute
    {
    }
}
