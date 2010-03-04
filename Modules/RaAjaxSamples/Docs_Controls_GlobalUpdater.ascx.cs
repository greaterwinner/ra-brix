/*
 * Ra-Ajax - A Managed Ajax Library for ASP.NET and Mono
 * Copyright 2008 - 2009 - Thomas Hansen thomas@ra-ajax.org
 * This code is licensed under the GPL version 3 which 
 * can be found in the license.txt file on disc.
 *
 */

using System;
using Ra.Extensions;
using Ra.Brix.Loader;

namespace RaAjaxSamples
{
    [ActiveModule]
    public class Docs_Controls_GlobalUpdater : System.Web.UI.UserControl
    {
        protected global::Ra.Widgets.Label lbl;

        protected void btn_Click(object sender, EventArgs e)
        {
            System.Threading.Thread.Sleep(2000);
            lbl.Text = "Since we had a Thread Sleep on the server for about 2 seconds" +
                "the Updater kicked in";
        }
    }
}
