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
using System.ComponentModel;
using Ra.Effects;

namespace Components
{
    [ASP.ToolboxData("<{0}:TextAreaEdit runat=\"server\"></{0}:TextAreaEdit>")]
    public class TextAreaEdit : Panel
    {
        private readonly TextArea _text = new TextArea();
        private readonly Label _link = new Label();

        public event EventHandler TextChanged;

        public string Text
        {
            get { return _text.Text; }
            set { _text.Text = value; }
        }

        [DefaultValue(-1)]
        public int TextLength
        {
            get
            {
                return ViewState["TextLength"] == null ? -1 : (int)ViewState["TextLength"];
            }
            set { ViewState["TextLength"] = value; }
        }

        protected override void OnInit(EventArgs e)
        {
            EnsureChildControls();
            base.OnInit(e);
        }

        protected override void OnPreRender(EventArgs e)
        {
            if (string.IsNullOrEmpty(Text))
                _link.Text = "[null]";
            base.OnPreRender(e);
        }

        protected override void CreateChildControls()
        {
            base.CreateChildControls();

            _link.ID = "lbl";
            _link.Click += LinkClick;
            CreateTextForLabel();
            Controls.Add(_link);

            _text.ID = "txt";
            _text.Style[Styles.display] = "none";
            _text.Blur += TextUpdated;
            _text.EscPressed += TextEscPressed;
            Controls.Add(_text);
        }

        private void CreateTextForLabel()
        {
            if (TextLength == -1)
            {
                _link.Text = Text;
            }
            else
            {
                if (TextLength >= Text.Length)
                {
                    _link.Text = Text;
                }
                else
                {
                    _link.Text = Text.Substring(0, TextLength) + "...";
                }
            }
        }

        void TextEscPressed(object sender, EventArgs e)
        {
            _text.Style[Styles.display] = "none";
            _link.Style[Styles.display] = "";
        }

        private void TextUpdated(object sender, EventArgs e)
        {
            CreateTextForLabel();
            _text.Style[Styles.display] = "none";
            _link.Style[Styles.display] = "";

            if (TextChanged != null)
                TextChanged(this, new EventArgs());
        }

        private void LinkClick(object sender, EventArgs e)
        {
            _link.Style[Styles.display] = "none";
            _text.Style[Styles.display] = "none";
            new EffectRollDown(_text, 200)
                .ChainThese(
                    new EffectFocusAndSelect(_text))
                .Render();
        }
    }
}
