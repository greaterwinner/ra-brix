/*
 * Ra-Brix - A Modular-based Framework for building 
 * Web Applications Copyright 2009 - Thomas Hansen 
 * thomas@ra-ajax.org. Unless permission is 
 * explicitly given this code is licensed under the 
 * GNU Affero GPL version 3 which can be found in the 
 * license.txt file on disc.
 * 
 */

using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ra.Builder;

namespace Components
{
    /**
     * A DataGrid for displaying tabular data with complex logic.
     */
    [ToolboxData("<{0}:Grid runat=\"server\"></{0}:Grid>")]
    public class ExtButtonPost : WebControl
    {
        /**
         * The text that is displayed within the button, default value is string.Empty.
         */
        [DefaultValue("")]
        public string Text
        {
            get { return ViewState["Text"] == null ? "" : (string)ViewState["Text"]; }
            set { ViewState["Text"] = value; }
        }

        [DefaultValue("button")]
        public override string CssClass
        {
            get
            {
                if (string.IsNullOrEmpty(base.CssClass))
                    return "button";
                return base.CssClass;
            }
            set { base.CssClass = value; }
        }

        protected void AddAttributes(Element el)
        {
            el.AddAttribute("type", "submit");
            if (!string.IsNullOrEmpty(AccessKey))
                el.AddAttribute("accesskey", AccessKey);
            if (!Enabled)
                el.AddAttribute("disabled", "disabled");
            if (!string.IsNullOrEmpty(ToolTip))
                el.AddAttribute("title", ToolTip);
            if (!string.IsNullOrEmpty(CssClass))
                el.AddAttribute("class", CssClass);
            string style = Style.Value;
            if (!string.IsNullOrEmpty(style))
                el.AddAttribute("style", style);
        }

        public override void RenderControl(HtmlTextWriter writer)
        {
            using (HtmlBuilder builder = new HtmlBuilder())
            {
                using (Element el = builder.CreateElement("button"))
                {
                    AddAttributes(el);
                    using (Element bRight = builder.CreateElement("span"))
                    {
                        bRight.AddAttribute("class", "bRight");
                        using (Element bLeft = builder.CreateElement("span"))
                        {
                            bLeft.AddAttribute("class", "bLeft");
                            using (Element bCenter = builder.CreateElement("span"))
                            {
                                bCenter.AddAttribute("class", "bCenter");
                                using (Element content = builder.CreateElement("span"))
                                {
                                    content.AddAttribute("id", ClientID + "_LBL");
                                    content.Write(Text);
                                }
                            }
                        }
                    }
                }
                writer.Write(builder.ToString());
            }
        }
    }
}
