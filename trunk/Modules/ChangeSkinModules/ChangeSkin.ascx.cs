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
using Ra.Brix.Loader;
using Ra.Brix.Types;
using Ra.Widgets;
using System.IO;
using System.Collections.Generic;

namespace ChangeSkinModules
{
    [ActiveModule]
    public class ChangeSkin : System.Web.UI.UserControl, IModule
    {
        private class SkinFolder
        {
            public SkinFolder(string folder)
            {
                Folder = folder;
            }

            public string Folder { get; set; }
        }

        protected global::System.Web.UI.WebControls.Repeater rep;

        protected void ChangeSkinMethod(object sender, EventArgs e)
        {
            ImageButton btn = sender as ImageButton;
            Node node = new Node();
            node["RootFolder"].Value = btn.Xtra;

            ActiveEvents.Instance.RaiseActiveEvent(
                this,
                EventToSend,
                node); 
        }

        private string EventToSend
        {
            get { return ViewState["EventToSend"] as string; }
            set { ViewState["EventToSend"] = value; }
        }

        public void InitialLoading(Node init)
        {
            Load +=
                delegate
                {
                    EventToSend = init["EventToSend"].Get<string>();
                    List<SkinFolder> skins = new List<SkinFolder>();
                    foreach (string folderIdx in Directory.GetDirectories(Server.MapPath("~/media/skins/")))
                    {
                        string folder = folderIdx.Substring(folderIdx.LastIndexOf(System.IO.Path.DirectorySeparatorChar) + 1);
                        if (!folder.Contains(".svn"))
                            skins.Add(new SkinFolder(folder));
                    }
                    rep.DataSource = skins;
                    rep.DataBind();
                };
        }

        public string GetCaption()
        {
            return "";
        }
    }
}