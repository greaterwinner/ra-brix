﻿/*
 * Ra-Ajax - A Managed Ajax Library for ASP.NET and Mono
 * Copyright 2008 - 2009 - Thomas Hansen thomas@ra-ajax.org
 * This code is licensed under the GPL version 3 which 
 * can be found in the license.txt file on disc.
 *
 */

using System;
using Ra.Extensions.Widgets;
using Ra;
using Ra.Widgets;
using Ra.Brix.Loader;

namespace RaAjaxSamples
{
    [ActiveModule]
    public class Docs_Controls_SlidingMenu : System.Web.UI.UserControl
    {
        protected global::Ra.Widgets.Label lbl;
        protected global::Ra.Extensions.Widgets.SlidingMenu slider;

        protected void window_GetChildNodes(object sender, EventArgs e)
        {
            SlidingMenuLevel level = sender as SlidingMenuLevel;
            for (int idx = 0; idx < 5; idx++)
            {
                SlidingMenuItem i = new SlidingMenuItem();
                i.ID = level.ID + idx;
                i.Text = "Window " + idx;
                level.Controls.Add(i);

                SlidingMenuLevel l = new SlidingMenuLevel();
                l.Caption = "Window " + idx;
                l.ID = level.ID + "LL" + idx;
                l.GetChildNodes += window_GetChildNodes;
                i.Controls.Add(l);
            }
        }

        protected void slider_Navigate(object sender, EventArgs e)
        {
            lbl.Text = slider.ActiveLevel == null ? "null" : slider.ActiveLevel;
        }

        protected void switchToSaveAs_Click(object sender, EventArgs e)
        {
            slider.SlideTo("file/saveAs/pdf");
        }

        protected void switchToWindow_Click(object sender, EventArgs e)
        {
            slider.SlideTo("window/SliderMenuLevel11/SliderMenuLevel1LL10");
        }

        protected void slider_ItemClicked(object sender, EventArgs e)
        {
            SlidingMenuItem item = sender as SlidingMenuItem;
            lbl.Text = item.ID;
        }
    }
}