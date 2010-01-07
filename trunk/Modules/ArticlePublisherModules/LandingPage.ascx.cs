﻿/*
 * Ra-Brix - A Modular-based Framework for building 
 * Web Applications Copyright 2009 - Thomas Hansen 
 * thomas@ra-ajax.org. Unless permission is 
 * explicitly given this code is licensed under the 
 * GNU Affero GPL version 3 which can be found in the 
 * license.txt file on disc.
 * 
 */

using Ra.Brix.Loader;
using Ra.Brix.Types;

namespace ArticlePublisherModules
{
    [ActiveModule]
    public class LandingPage : System.Web.UI.UserControl, IModule
    {
        public void InitialLoading(Node node)
        {
        }

        public string GetCaption()
        {
            return "";
        }
    }
}