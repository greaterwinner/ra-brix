﻿/*
 * Ra-Brix - A Modular-based Framework for building 
 * Web Applications Copyright 2009 - Thomas Hansen 
 * thomas@ra-ajax.org. Unless permission is 
 * explicitly given this code is licensed under the 
 * GNU Affero GPL version 3 which can be found in the 
 * license.txt file on disc.
 * 
 */

using System;
using LanguageRecords;
using Ra.Brix.Loader;
using Ra.Brix.Types;
using Ra.Effects;
using Ra.Widgets;

namespace PromoModules
{
    [ActiveModule]
    public class ViewPromoters : System.Web.UI.UserControl, IModule
    {
        protected global::Components.Grid grd;

        public void InitialLoading(Node node)
        {
            Load +=
                delegate
                {
                    grd.DataSource = node["Grid"];
                    grd.DataBind();
                };
        }
    }
}