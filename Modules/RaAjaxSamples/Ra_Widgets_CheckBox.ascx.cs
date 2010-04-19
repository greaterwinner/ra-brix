/*
 * Ra-Ajax - A Managed Ajax Library for ASP.NET and Mono
 * Copyright 2008 - 2009 - Thomas Hansen thomas@ra-ajax.org
 * This code is licensed under the GPL version 3 which 
 * can be found in the license.txt file on disc.
 *
 */

using System;
using Ra.Widgets;
using Ra.Brix.Loader;

namespace RaAjaxSamples
{
    [ActiveModule]
    public class Ra_Widgets_CheckBox : System.Web.UI.UserControl
    {
        protected void chk_CheckedChanged(object sender, EventArgs e)
        {
            (sender as CheckBox).Text = "I was changed at " + 
                DateTime.Now.ToString();
        }
    }
}
