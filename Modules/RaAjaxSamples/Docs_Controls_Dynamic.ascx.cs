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
            string fileName = "~/Docs-Controls/" + e.Key;
            System.Web.UI.Control ctrl = Page.LoadControl(fileName);
            dyn.Controls.Add(ctrl);
        }
    }
}
