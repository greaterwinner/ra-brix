/*
 * Ra-Ajax - A Managed Ajax Library for ASP.NET and Mono
 * Copyright 2008 - 2009 - Thomas Hansen thomas@ra-ajax.org
 * This code is licensed under the GPL version 3 which 
 * can be found in the license.txt file on disc.
 *
 */

using System;
using Ra.Extensions;
using System.Threading;
using Ra.Widgets;
using Ra.Brix.Loader;

namespace RaAjaxSamples
{
    [ActiveModule]
    public class Docs_Controls_Dynamic : System.Web.UI.UserControl
    {
        protected global::Ra.Widgets.SelectList sel;
        protected global::Ra.Widgets.Dynamic dyn;

        protected void sel_SelectedIndexChanged(object semder, EventArgs e)
        {
            if (sel.SelectedItem.Value != "nothing")
                dyn.LoadControls(sel.SelectedItem.Value);
            else
                dyn.ClearControls();
        }

        protected void dyn_Reload(object sender, Dynamic.ReloadEventArgs e)
        {
            // Notice that this loading mechanism uses the Ra-Brix 
            // PluginLoader, while you if you use only Ra-Ajax should 
            // use Page.LoadControl but due to the nature of this 
            // website, which is a Ra-Brix application we need to 
            // load our UserControls as ActiveModules. You would 
            // probably use, in a pure Ra-Ajax solution, 
            // Page.LoadControl and it would work perfectly well 
            // none the less ...
            System.Web.UI.Control ctrl = 
                PluginLoader.Instance.LoadControl(e.Key);
            dyn.Controls.Add(ctrl);
        }
    }
}
