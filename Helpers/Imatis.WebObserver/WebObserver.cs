/*
 * Ra-Ajax - A Managed Ajax Library for ASP.NET and Mono
 * Copyright 2008 - 2009 - Thomas Hansen thomas@ra-ajax.org
 * This code is licensed under the LGPL version 3 which 
 * can be found in the license.txt file on disc.
 *
 */

using System;
using System.ComponentModel;
using WEBCTRLS = System.Web.UI.WebControls;
using ASP = System.Web.UI;
using Ra.Widgets;
using HTML = System.Web.UI.HtmlControls;
using Ra.Builder;
using Ra;

[assembly: ASP.WebResource("Imatis.WebObserver.Js.WebObserver.js", "text/javascript")]

namespace Imatis.WebObserver
{
    [ASP.ToolboxData("<{0}:WebObserver runat=server></{0}:WebObserver>")]
    public class WebObserver : RaWebControl
    {
        [DefaultValue(400)]
        public int Width
        {
            get { return ViewState["Width"] == null ? 400 : (int)ViewState["Width"]; }
            set
            {
                if (value != Width)
                    SetJSONGenericValue("Width", value.ToString());
                ViewState["Width"] = value;
            }
        }

        [DefaultValue(400)]
        public int Height
        {
            get { return ViewState["Height"] == null ? 400 : (int)ViewState["Height"]; }
            set
            {
                if (value != Height)
                    SetJSONGenericValue("Height", value.ToString());
                ViewState["Height"] = value;
            }
        }

        [DefaultValue("")]
        public string DataURL
        {
            get { return ViewState["DataURL"] == null ? string.Empty : ViewState["DataURL"].ToString(); }
            set
            {
                if (value != DataURL)
                    SetJSONValueString("DataURL", value);
                ViewState["DataURL"] = value;
            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            AjaxManager.Instance.IncludeScriptFromResource(typeof(WebObserver), "Imatis.WebObserver.Js.WebObserver.js");
        }

        protected override string GetClientSideScriptType()
        {
            return "new Imatis.WebObserver";
        }

        protected override string GetClientSideScriptOptions()
        {
            string retVal = base.GetClientSideScriptOptions();
            if (!string.IsNullOrEmpty(DataURL))
                retVal += string.Format("dataURL:'{0}'", DataURL);
            return retVal;
        }

        protected override void RenderRaControl(HtmlBuilder builder)
        {
            using (Element el = builder.CreateElement("canvas"))
            {
                AddAttributes(el);
            }
        }

        protected override void AddAttributes(Element el)
        {
            el.AddAttribute("width", Width.ToString());
            el.AddAttribute("height", Height.ToString());
            base.AddAttributes(el);
        }
    }
}
