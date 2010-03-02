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
using LanguageRecords;
using Ra.Brix.Types;
using ExternalLinksRecords;
using System;
using System.Collections.Generic;

namespace MenuExternalLinksController
{
    [ActiveController]
    public class ExternalLinksController
    {
        [ActiveEvent(Name = "ApplicationStartup")]
        protected static void ApplicationStartup(object sender, ActiveEventArgs e)
        {
            Language.Instance.SetDefaultValue("ButtonExternalLinks", "External Links");
        }

        [ActiveEvent(Name = "GetMenuItems")]
        protected void GetMenuItems(object sender, ActiveEventArgs e)
        {
            // Edit External Links menu button
            e.Params["ButtonAdmin"]["ButtonCMS"]["ButtonExternalLinks"].Value = "Menu-CMS-ViewExternalLinks";

            // External Links buttons
            foreach (ExternalLink idx in ExternalLink.Select())
            {
                Node currentNode = e.Params;
                List<string> splits = new List<string>(
                    idx.Name.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries));
                while (true)
                {
                    if (splits.Count == 0)
                    {
                        currentNode.Value = "url:" + idx.URL;
                        break;
                    }
                    else
                    {
                        Node existing = currentNode.Find(
                            delegate(Node idxNode)
                            {
                                return idxNode.Name == splits[0];
                            });
                        if (existing != null)
                        {
                            // We're adding nodes to an existing path here, and hence the different logic...
                            currentNode = existing;
                            splits.RemoveAt(0);
                        }
                        else
                        {
                            // The "root node" does not exist here...
                            Node n = new Node(splits[0]);
                            n.Value = splits[0] + idx.URL.GetHashCode().ToString().Replace("-", "");
                            splits.RemoveAt(0);
                            currentNode.Add(n);
                            currentNode = n;
                        }
                    }
                }
            }
        }

        [ActiveEvent(Name = "Menu-CMS-ViewExternalLinks")]
        protected void ViewExternalLinks(object sender, ActiveEventArgs e)
        {
            Node node = new Node();
            int idxNo = 0;
            foreach (ExternalLink idx in ExternalLink.Select())
            {
                node["ModuleSettings"]["Links"]["Link" + idxNo]["Name"].Value = idx.Name;
                node["ModuleSettings"]["Links"]["Link" + idxNo]["URL"].Value = idx.URL;
                node["ModuleSettings"]["Links"]["Link" + idxNo]["ID"].Value = idx.ID;
                idxNo += 1;
            }
            ActiveEvents.Instance.RaiseLoadControl(
                "ExternalLinksModules.ViewLinks",
                "dynMid",
                node);
        }

        [ActiveEvent(Name = "CreateNewExternalLink")]
        protected void CreateNewExternalLink(object sender, ActiveEventArgs e)
        {
            ExternalLink l = new ExternalLink();
            l.Save();
            int idxNo = 0;
            foreach (ExternalLink idx in ExternalLink.Select())
            {
                e.Params["Links"]["Link" + idxNo]["Name"].Value = idx.Name;
                e.Params["Links"]["Link" + idxNo]["URL"].Value = idx.URL;
                e.Params["Links"]["Link" + idxNo]["ID"].Value = idx.ID;
                idxNo += 1;
            }
        }

        [ActiveEvent(Name = "ExternalLinksValueUpdated")]
        protected void ExternalLinksValueUpdated(object sender, ActiveEventArgs e)
        {
            int id = e.Params["ID"].Get<int>();
            string url = e.Params["URL"].Get<string>();
            string name = e.Params["Name"].Get<string>();
            ExternalLink l = ExternalLink.SelectByID(id);
            if (!string.IsNullOrEmpty(url))
                l.URL = url;
            if (!string.IsNullOrEmpty(name))
                l.Name = name;
            l.Save();
        }
    }
}
