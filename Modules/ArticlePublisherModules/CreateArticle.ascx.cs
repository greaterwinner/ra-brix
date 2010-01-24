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
using Ra.Brix.Types;
using System;
using Ra.Extensions.Widgets;
using Ra.Widgets;

namespace ArticlePublisherModules
{
    [ActiveModule]
    public class CreateArticle : System.Web.UI.UserControl, IModule
    {
        protected global::Ra.Extensions.Widgets.RichEdit editor;
        protected global::Ra.Extensions.Widgets.InPlaceEdit header;
        protected global::Ra.Widgets.TextArea ingress;
        protected global::Ra.Widgets.Image img;
        protected global::Ra.Widgets.TextBox tags;

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

        private bool IsMainImageMode
        {
            get { return ViewState["IsMainImageMode"] == null ? false : (bool)ViewState["IsMainImageMode"]; }
            set { ViewState["IsMainImageMode"] = value; }
        }

        protected void editor_GetImageDialog(object sender, EventArgs e)
        {
            IsMainImageMode = false;
            ActiveEvents.Instance.RaiseActiveEvent(
                this,
                "CMSGetImageDialog");
        }

        protected void img_Click(object sender, EventArgs e)
        {
            IsMainImageMode = true;
            ActiveEvents.Instance.RaiseActiveEvent(
                this,
                "CMSGetImageDialog");
        }

        [ActiveEvent(Name = "ImageFileChosenByFileDialog")]
        protected void ImageFileChosenByFileDialog(object sender, ActiveEventArgs e)
        {
            if (IsMainImageMode)
            {
                img.ImageUrl = e.Params["File"].Get<string>();
            }
            else
            {
                string fileName = e.Params["File"].Get<string>();
                string html = string.Format(
                    @"<img src=""{0}"" alt=""unknown"" />", fileName);
                editor.PasteHTML(html);
            }
        }

        protected void submit_Click(object sender, EventArgs e)
        {
            SaveArticle();
        }

        protected int ArticleID
        {
            get { return (int)ViewState["ArticleID"]; }
            set { ViewState["ArticleID"] = value; }
        }

        private void SaveArticle()
        {
            Node node = new Node();
            node["Header"].Value = header.Text;
            node["Body"].Value = editor.Text;
            node["Ingress"].Value = ingress.Text;
            node["Image"].Value = img.ImageUrl;
            node["ID"].Value = ArticleID;
            string[] strTags = tags.Text.Split(',');
            node["Tags"].Value = strTags;
            ActiveEvents.Instance.RaiseActiveEvent(
                this,
                "SaveArticle",
                node);
            ArticleID = node["ID"].Get<int>();
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

        [ActiveEvent(Name = "CMSInsertLink")]
        protected void CMSInsertLink(object sender, ActiveEventArgs e)
        {
            string html = string.Format(
                @"<a href=""{0}"">{1}</a>",
                e.Params["URL"].Get<string>(),
                e.Params["Text"].Get<string>());
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

        protected void editor_CtrlKeys(object sender, RichEdit.CtrlKeysEventArgs e)
        {
            if (e.Key == 's')
            {
                SaveArticle();
            }
        }

        public void InitialLoading(Node node)
        {
            Load +=
                delegate
                {
                    if (node["IsEditing"].Value != null && node["IsEditing"].Get<bool>())
                    {
                        ArticleID = node["ID"].Get<int>();
                        editor.Text = node["Body"].Get<string>();
                        header.Text = node["Header"].Get<string>();
                        ingress.Text = node["Ingress"].Get<string>();
                        tags.Text = node["TagList"].Get<string>();
                        img.ImageUrl = node["Image"].Get<string>();
                    }
                    else
                    {
                        ArticleID = -1;
                    }
                };
        }

        public string GetCaption()
        {
            return "";
        }
    }
}