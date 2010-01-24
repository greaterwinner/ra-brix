/*
 * Ra-Brix - A Modular-based Framework for building 
 * Web Applications Copyright 2009 - Thomas Hansen 
 * thomas@ra-ajax.org. Unless permission is 
 * explicitly given this code is licensed under the 
 * GNU Affero GPL version 3 which can be found in the 
 * license.txt file on disc.
 * 
 */

using Ra.Brix.Loader;
using Ra.Brix.Types;
using System;
using Ra.Extensions.Widgets;
using Ra.Widgets;

namespace ArticlePublisherModules
{
    [ActiveModule]
    public class EditTags : System.Web.UI.UserControl, IModule
    {
        protected global::System.Web.UI.WebControls.Repeater rep;
        protected global::Ra.Widgets.Panel repWrp;

        protected void StickyChanged(object sender, EventArgs e)
        {
            CheckBox b = sender as CheckBox;
            int id = int.Parse(b.Xtra);
            Node node = new Node();
            node["ID"].Value = id;
            node["Sticky"].Value = b.Checked;
            ActiveEvents.Instance.RaiseActiveEvent(
                this,
                "ArticleTagStickyChanged",
                node);
        }

        public void InitialLoading(Node node)
        {
            rep.DataSource = node["Tags"];
            rep.DataBind();
        }

        public string GetCaption()
        {
            return "";
        }
    }
}