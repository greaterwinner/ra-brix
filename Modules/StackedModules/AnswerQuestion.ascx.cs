/*
 * Ra-Brix - A Modular-based Framework for building 
 * Web Applications Copyright 2009 - Thomas Hansen 
 * thomas@ra-ajax.org. Unless permission is 
 * explicitly given this code is licensed under the 
 * GNU Affero GPL version 3 which can be found in the 
 * license.txt file on disc.
 * 
 */

using LanguageRecords;
using Ra.Brix.Loader;
using Ra.Brix.Types;
using System;
using Ra.Widgets;
using Ra.Extensions.Widgets;
using Ra.Effects;

namespace StackedModules
{
    [ActiveModule]
    public class AnswerQuestion : System.Web.UI.UserControl, IModule
    {
        protected global::Ra.Extensions.Widgets.RichEdit editor;
        protected global::Ra.Widgets.Panel allWrp;
        protected global::Ra.Widgets.Panel answerBtnWrp;
        protected global::Ra.Extensions.Widgets.ExtButton showAnswer;

        private int QuestionID
        {
            get { return (int)ViewState["QuestionID"]; }
            set { ViewState["QuestionID"] = value; }
        }

        public void InitialLoading(Node node)
        {
            Load +=
                delegate
                {
                    showAnswer.DataBind();
                    QuestionID = node["QuestionID"].Get<int>();
                    editor.Text = 
                        "<p>" + 
                        Language.Instance["TypeYourAnswerHere", null, "Type your answer here..."] + 
                        "</p>";
                };
        }

        protected void editor_GetImageDialog(object sender, EventArgs e)
        {
            ActiveEvents.Instance.RaiseActiveEvent(
                this,
                "CMSGetImageDialog");
        }

        protected void showAnswer_Click(object sender, EventArgs e)
        {
            allWrp.Visible = true;
            allWrp.Style[Styles.display] = "none";
            new EffectRollUp(answerBtnWrp, 500)
                .ChainThese(
                    new EffectRollDown(allWrp, 500),
                    new EffectFocusAndSelect(editor))
                .Render();
        }

        protected void editor_GetResourceDialog(object sender, EventArgs e)
        {
            ActiveEvents.Instance.RaiseActiveEvent(
                this,
                "CMSGetResourceDialog");
        }

        protected void editor_GetHyperLinkDialog(object sender, EventArgs e)
        {
            Node node = new Node();
            node["AnchorText"].Value = editor.Selection;
            ActiveEvents.Instance.RaiseActiveEvent(
                this,
                "CMSGetHyperLinkDialog",
                node);
        }

        protected void editor_CtrlKeys(object sender, RichEdit.CtrlKeysEventArgs e)
        {
            if (e.Key == 's')
            {
                SaveAnswer();
            }
        }

        protected void editor_GetExtraToolbarControls(object sender, RichEdit.ExtraToolbarControlsEventArgs e)
        {
            // Save button
            LinkButton submit = new LinkButton();
            submit.ID = "submit";
            submit.CssClass = "editorBtn save";
            submit.Click += submit_Click;
            submit.Text = "&nbsp;";
            e.Controls.Add(submit);
        }

        protected void submit_Click(object sender, EventArgs e)
        {
            SaveAnswer();
        }

        private void SaveAnswer()
        {
            Node node = new Node();
            node["Body"].Value = editor.Text;
            node["QuestionID"].Value = QuestionID;
            node["Success"].Value = false;
            ActiveEvents.Instance.RaiseActiveEvent(
                this,
                "RequestSavingOfStackedAnswer",
                node);
            if (node["Success"].Get<bool>())
            {
                new EffectRollUp(allWrp, 250)
                    .ChainThese(
                        new EffectRollDown(answerBtnWrp, 250))
                    .Render();
                editor.Text =
                    "<p>" +
                    Language.Instance["TypeYourAnswerHere", null, "Type your answer here..."] +
                    "</p>";
            }
        }
    }
}




