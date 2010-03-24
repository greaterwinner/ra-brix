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

namespace StackedModules
{
    [ActiveModule]
    public class AskQuestion : System.Web.UI.UserControl, IModule
    {
        protected global::Ra.Extensions.Widgets.InPlaceEdit header;
        protected global::Ra.Extensions.Widgets.RichEdit editor;

        protected void editor_GetImageDialog(object sender, EventArgs e)
        {
            ActiveEvents.Instance.RaiseActiveEvent(
                this,
                "CMSGetImageDialog");
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
                SaveQuestion();
            }
        }

        [ActiveEvent(Name = "CMSInsertLink")]
        protected void CMSInsertLink(object sender, ActiveEventArgs e)
        {
            string html = string.Format(
                @"<a href=""{0}"">{1}</a>", 
                e.Params["URL"].Get<string>(),
                e.Params["Text"].Get<string>());
            editor.PasteHTML(html);
        }

        [ActiveEvent(Name = "ImageFileChosenByFileDialog")]
        protected void ImageFileChosenByFileDialog(object sender, ActiveEventArgs e)
        {
            string fileName = e.Params["File"].Get<string>();
            string html = string.Format(
                @"<img src=""{0}"" alt=""unknown"" />", fileName);
            editor.PasteHTML(html);
        }

        [ActiveEvent(Name = "ResourceFileChosenByFileDialog")]
        protected void ResourceFileChosenByFileDialog(object sender, ActiveEventArgs e)
        {
            string fileName = e.Params["File"].Get<string>();
            string cssClass = "resourceDownload specificResource-" + 
                fileName.Substring(fileName.LastIndexOf('.') + 1).ToLowerInvariant();
            string html = string.Format(
                @"<a target=""_blank"" href=""{0}"" class=""{1}"" />", fileName, cssClass);
            editor.PasteHTML(html);
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
            SaveQuestion();
        }

        private void SaveQuestion()
        {
            Node node = new Node();
            node["Header"].Value = header.Text;
            node["Body"].Value = editor.Text;
            ActiveEvents.Instance.RaiseActiveEvent(
                this,
                "RequestSavingOfStackedQuestion",
                node);
        }

        public void InitialLoading(Node node)
        {
            Load +=
                delegate
                {
                    header.DataBind();
                    editor.Text = @"
<p>" + Language.Instance["YourQuestionGoesHere", null, "Your question goes here..."] + "</p>";
                };
        }
    }
}




