/*
 * Ra-Ajax - A Managed Ajax Library for ASP.NET and Mono
 * Copyright 2008 - 2009 - Thomas Hansen thomas@ra-ajax.org
 * This code is licensed under the GPL version 3 which 
 * can be found in the license.txt file on disc.
 *
 */

using System;
using Ra.Brix.Loader;
using Ra.Extensions.Widgets;

namespace RaAjaxSamples
{
    [ActiveModule]
    public class Ra_Widgets_Button : System.Web.UI.UserControl
    {
        protected void btn_Click(object sender, EventArgs e)
        {
            (sender as ExtButton).Text = "I was clicked at " + DateTime.Now.ToString();
        }
    }
}
