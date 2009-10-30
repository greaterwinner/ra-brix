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
using ASP = System.Web.UI;
using Ra.Widgets;
using WC = System.Web.UI.WebControls;
using Ra.Builder;
using Ra;

[assembly: ASP.WebResource("Components.Tracer.js", "text/javascript")]

namespace Components
{
    [ASP.ToolboxData("<{0}:Tracer runat=\"server\" />")]
    public class Tracer : RaWebControl
    {
        protected override void RenderRaControl(HtmlBuilder builder)
        {
            using (Element el = builder.CreateElement("span"))
            {
                AddAttributes(el);
                RenderChildren(builder.Writer);
            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            AjaxManager.Instance.IncludeScriptFromResource(typeof(Tracer), "Components.Tracer.js");
            base.OnPreRender(e);
        }

        protected override string GetClientSideScriptType()
        {
            return "new Ra.Tracer";
        }
    }
}
