﻿/*
 * Ra-Ajax - A Managed Ajax Library for ASP.NET and Mono
 * Copyright 2008 - 2009 - Thomas Hansen thomas@ra-ajax.org
 * This code is licensed under the GPL version 3 which 
 * can be found in the license.txt file on disc.
 *
 */

using System;
using Ra.Extensions;
using Ra.Extensions.Widgets;
using Ra.Brix.Loader;

namespace RaAjaxSamples
{
    [ActiveModule]
    public class Docs_Controls_Tree : System.Web.UI.UserControl
    {
        protected global::Ra.Widgets.Label lbl;
        protected global::Ra.Extensions.Widgets.Tree tree;

        protected void window_GetChildNodes(object sender, EventArgs e)
        {
            TreeNodes level = sender as TreeNodes;
            for (int idx = 0; idx < 5; idx++)
            {
                TreeNode i = new TreeNode();
                i.ID = level.ID + idx;
                i.Text = "Window " + idx;
                level.Controls.Add(i);

                TreeNodes l = new TreeNodes();
                l.ID = level.ID + "LL" + idx;
                l.GetChildNodes += window_GetChildNodes;
                i.Controls.Add(l);
            }
        }

        protected void tree_ItemClicked(object sender, EventArgs e)
        {
            lbl.Text = tree.SelectedNodes[0].Text;
        }
    }
}
