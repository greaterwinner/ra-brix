﻿/*
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
    public class Docs_Controls_CheckBox : System.Web.UI.UserControl
    {
        protected global::Ra.Widgets.CheckBox chk;

        protected void chk_CheckedChanged(object semder, EventArgs e)
        {
            if (chk.Checked)
                chk.Text = "Hey, get off my lap...!";
            else
                chk.Text = "I am so lonely... :(";
        }
    }
}
