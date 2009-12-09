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

namespace CMSModules
{
    [ActiveModule]
    public class InstallNewApp : System.Web.UI.UserControl, IModule
    {
        protected global::System.Web.UI.WebControls.Button submit;
        protected global::System.Web.UI.WebControls.FileUpload uploader;

        protected void Page_Load(object sender, EventArgs e)
        {
            submit.DataBind();
        }

        protected void submit_Click(object sender, EventArgs e)
        {
            Node node = new Node();
            node["FileBytes"].Value = uploader.FileBytes;
            node["FileName"].Value = uploader.FileName;
            ActiveEvents.Instance.RaiseActiveEvent(
                this,
                "ApplicationUploadedForInstallation",
                node);
        }

        public void InitialLoading(Node node)
        {
            submit.Focus();
        }

        public string GetCaption()
        {
            return "";
        }
    }
}