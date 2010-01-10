/*
 * Ra-Brix - A Modular-based Framework for building 
 * Web Applications Copyright 2009 - Thomas Hansen 
 * thomas@ra-ajax.org. Unless permission is 
 * explicitly given this code is licensed under the 
 * GNU Affero GPL version 3 which can be found in the 
 * license.txt file on disc.
 * 
 */

using System;
using System.Web.UI;
using LanguageRecords;
using Ra.Brix.Loader;
using Ra.Brix.Types;
using System.IO;
using Ra.Extensions.Widgets;
using Components;
using Ra.Effects;
using Ra.Widgets;
using System.Globalization;
using Ra.Selector;

namespace ResourcesModules
{
    [ActiveModule]
    public class FlickrExplorer : System.Web.UI.UserControl, IModule
    {
        protected global::Ra.Widgets.Panel pnlWrp;
        protected global::Ra.Widgets.TextBox search;
        protected global::System.Web.UI.WebControls.Repeater rep;
        protected global::Ra.Widgets.Panel pnlRepWrp;

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            string query = search.Text;
            Node node = new Node();
            node["Query"].Value = query;
            ActiveEvents.Instance.RaiseActiveEvent(
                this,
                "SearchFlickrForImages",
                node);
            rep.DataSource = node["Images"];
            rep.DataBind();
            pnlRepWrp.ReRender();
            OldPreviewImageID = null;
        }

        private string OldPreviewImageID
        {
            get { return ViewState["OldPreviewImageID"] as string; }
            set { ViewState["OldPreviewImageID"] = value; }
        }

        protected void ImagePreview(object sender, EventArgs e)
        {
            Image imgClicked = sender as Image;
            imgClicked.Style[Styles.border] = "solid Blue 3px";
            Panel pnl = (sender as Control).Parent as Panel;
            Image img = Selector.SelectFirst<Image>(pnl,
                delegate(Control idx)
                {
                    if (idx is RaWebControl)
                    {
                        return (idx as RaWebControl).Style[Styles.zIndex] == "100";
                    }
                    return false;
                });
            img.Style[Styles.display] = "none";
            img.Visible = true;
            if (!string.IsNullOrEmpty(OldPreviewImageID))
            {
                Image old = Selector.SelectFirst<Image>(this,
                    delegate(Control idx)
                    {
                        if (idx is Image)
                        {
                            return (idx as Image).ImageUrl == OldPreviewImageID;
                        }
                        return false;
                    });
                new EffectRollUp(old, 500)
                    .ChainThese(
                        new EffectRollDown(img, 500))
                    .Render();
            }
            else
            {
                new EffectRollDown(img, 500)
                    .Render();
            }
            OldPreviewImageID = img.ImageUrl;
        }

        protected void MouseOutOverPreview(object sender, EventArgs e)
        {
            Image img = sender as Image;

            // If Old is not image url, then it's already fading out ...
            if (img.ImageUrl == OldPreviewImageID)
            {
                new EffectRollUp(img, 500)
                    .Render();
                OldPreviewImageID = null;
            }
        }

        protected void ImageChosen(object sender, EventArgs e)
        {
            Node node = new Node();
            node["ActiveFolder"].Value = ActiveFolder;
            node["ImageURL"].Value = (sender as Image).ImageUrl;
            node["ImageName"].Value = (sender as Image).AlternateText;
            ActiveEvents.Instance.RaiseActiveEvent(
                this,
                "ImageSelectedFromFlickr",
                node);
        }

        private string ActiveFolder
        {
            get { return ViewState["ActiveFolder"] as string; }
            set { ViewState["ActiveFolder"] = value; }
        }

        void IModule.InitialLoading(Node node)
        {
            search.Select();
            search.Focus();
            Load +=
                delegate
                {
                    ActiveFolder = node["ActiveFolder"].Get<string>();
                    pnlWrp.DataBind();
                };
        }

        string IModule.GetCaption()
        {
            return "";
        }
    }
}