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

namespace ChangeSkinModules
{
    [ActiveModule]
    public class UploadNewSkin : System.Web.UI.UserControl, IModule
    {
        protected global::System.Web.UI.WebControls.FileUpload file;
        protected global::Components.ExtButtonPost submit;

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            submit.DataBind();
        }

        protected void submit_Click(object sender, EventArgs e)
        {
            byte[] fileContent = file.FileBytes;
            Node node = new Node();
            node["FileContent"].Value = fileContent;

            ActiveEvents.Instance.RaiseActiveEvent(
                this,
                "ImportNewSkin",
                node);
        }

        public void InitialLoading(Node node)
        {
        }
    }
}