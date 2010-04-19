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
using System.IO;
using ColorizerLibrary;
using Doxygen.NET;
using System.Web;
using System.Configuration;
using System.Collections.Generic;

namespace DoxygenDotNetViewDocsController
{
    [ActiveController]
    public class ViewDocsController
    {
        private static Dictionary<string, string> _tutorials = new Dictionary<string, string>();
        private static bool _hasTutorials = true;
        private static Dictionary<string, Tuple<string, string>> _tutorialContent = 
            new Dictionary<string, Tuple<string, string>>();

        [ActiveEvent(Name = "ApplicationStartup")]
        protected static void ApplicationStartup(object sender, ActiveEventArgs e)
        {
            Language.Instance.SetDefaultValue("ButtonDocumentation", "Documentation");
            Language.Instance.SetDefaultValue("ButtonTutorials", "Tutorials");
            Language.Instance.SetDefaultValue("ButtonClassesReferences", "Classes");
        }

        [ActiveEvent(Name = "GetMenuItems")]
        protected void GetMenuItems(object sender, ActiveEventArgs e)
        {
            GetClassesMenuItems(e);
            e.Params["ButtonDocumentation"].Value = "Menu-ViewDocumentation";
            if (_tutorials.Count == 0)
            {
                string root = HttpContext.Current.Server.MapPath("~/");
                if (Directory.Exists(root + "tutorials"))
                {
                    foreach (string fileName in Directory.GetFiles(root + "tutorials/", "*.txt"))
                    {
                        string[] tmpSplits = fileName.Split('-');
                        string tutorialName = tmpSplits[tmpSplits.Length - 1].Replace(".txt", "").Trim();
                        string tutorialUrl = tutorialName.Replace(" ", "-");
                        _tutorials[tutorialName] = "url:~/tutorials/" +
                            tutorialUrl.ToLower() +
                            ConfigurationManager.AppSettings["DefaultPageExtension"];
                    }
                }
                else
                {
                    _hasTutorials = false;
                }
            }
            if (_hasTutorials)
            {
                e.Params["ButtonTutorials"].Value = "tutorials";
                foreach (string idxKey in _tutorials.Keys)
                {
                    e.Params["ButtonTutorials"][idxKey].Value = _tutorials[idxKey];
                }
            }
        }

        private static Docs RaDocs
        {
            get
            {
                if (HttpContext.Current.Application["RaDocs"] == null)
                {
                    HttpContext.Current.Application["RaDocs"] =
                        new Docs(HttpContext.Current.Server.MapPath("~/docs-xml"));
                }
                return (Docs)HttpContext.Current.Application["RaDocs"];
            }
        }

        private static void GetClassesMenuItems(ActiveEventArgs e)
        {
            e.Params["ButtonClassesReferences"].Value = "Menu-ViewClassesReferences";
            foreach (Class idx in RaDocs.GetAllClasses())
            {
                switch (idx.FullName)
                {
                    // removing the classes we don't really NEED documentation for...
                    case "Ra.Extensions.Helpers.CometQueue":
                    case "Ra.Core.CallbackFilter":
                    case "Ra.Extensions.Widgets.AccordionView.EffectChange":
                    case "Ra.Extensions.Widgets.AutoCompleter.RetrieveAutoCompleterItemsEventArgs":
                    case "Ra.Core.PostbackFilter":
                    case "Ra.Widgets.ListItemCollection":
                    case "Ra.Widgets.StyleCollection":
                    case "Ra.Widgets.StyleCollection.StyleValue":
                    case "Ra.Widgets.ListItem":
                        break;
                    default:
                        {
                            e.Params["ButtonClassesReferences"][idx.Name].Value = 
                                "url:~/class/" + idx.FullName.Replace(".", "-");
                        } break;
                }
            }
        }

        [ActiveEvent(Name = "Page_Init_InitialLoading")]
        protected void InitialLoadingOfPage(object sender, ActiveEventArgs e)
        {
            string contentId = HttpContext.Current.Request.Params["ContentID"];
            if (contentId != null)
            {
                contentId = contentId.Trim('/');
            }
            if (string.IsNullOrEmpty(contentId))
                return;
            else if (contentId.IndexOf("tutorials/") == 0)
            {
                LoadTutorial(contentId, true);
            }
            else if (contentId.IndexOf("class/") == 0)
            {
                LoadClass(contentId);
            }
        }

        private void LoadClass(string contentId)
        {
            string className = contentId.Replace("class/", "").Replace("-", ".");
            Class c = RaDocs.GetTypeByName(className) as Class;
            Node node = new Node();
            node["Class"].Value = c;
            ((System.Web.UI.Page)HttpContext.Current.CurrentHandler).Title =
                Language.Instance["DocumentationFor", null, "Documentation for "] + c.Name; ;
            ActiveEvents.Instance.RaiseActiveEvent(
                this,
                "DoxygentDotNetShowClass",
                node);
        }

        private static void LoadTutorial(string contentId, bool setTitle)
        {
            // We have a tutorial link...
            if (!_tutorialContent.ContainsKey(contentId))
            {
                string tutorialFileName = " - " + contentId.Replace("tutorials/", "").Replace("-", " ") + ".txt";
                string root = HttpContext.Current.Server.MapPath("~/tutorials/");
                string fileName = Directory.GetFiles(root, "*" + tutorialFileName)[0];
                using (TextReader reader = new StreamReader(fileName))
                {
                    CodeColorizer colorizer = ColorizerLibrary.Config.DOMConfigurator.Configure();
                    string tutorialSyntaxed = colorizer.ProcessAndHighlightText(reader.ReadToEnd());
                    _tutorialContent[contentId] = new Tuple<string, string>(
                        fileName.Substring(fileName.LastIndexOf("\\") + 1).Replace(".txt", ""),
                        tutorialSyntaxed);
                }
            }
            Node node = new Node();
            node["ModuleSettings"]["Header"].Value = _tutorialContent[contentId].Left;
            node["ModuleSettings"]["Text"].Value = _tutorialContent[contentId].Right;
            if (setTitle)
            {
                ((System.Web.UI.Page)HttpContext.Current.CurrentHandler).Title =
                    Language.Instance["Tutorial:", null, "Tutorials: "] +
                    _tutorialContent[contentId].Left;
            }
            ActiveEvents.Instance.RaiseLoadControl(
                "DoxygentDotNetViewDocsModules.ViewTutorial",
                "dynMid",
                node);
        }

        [ActiveEvent(Name = "Menu-ViewDocumentation")]
        protected void ViewDocumentation(object sender, ActiveEventArgs e)
        {
            // Loading Documentation Browser
            Node node = new Node();
            node["TabCaption"].Value = 
                Language.Instance["ReferenceDocumentation", null, "Reference documentation"];
            node["Width"].Value = 450;
            node["Height"].Value = 450;
            ActiveEvents.Instance.RaiseLoadControl(
                "DoxygentDotNetViewDocsModules.ViewDocs",
                "dynPopup2",
                node);

            // Showing a message to inform user about usage
            Node nodeMessage = new Node();
            nodeMessage["Message"].Value = 
                Language.Instance[
                    "UseWindowToTopeRight", 
                    null, 
                    "Use the window at the top right corner to select classes or articles to read..."];
            nodeMessage["Duration"].Value = 2000;
            ActiveEvents.Instance.RaiseActiveEvent(
                this,
                "ShowInformationMessage",
                nodeMessage);
        }

        [ActiveEvent(Name = "DoxygentDotNetShowTutorial")]
        protected void DoxygentDotNetShowTutorial(object sender, ActiveEventArgs e)
        {
            LoadTutorial(e.Params["TutorialFile"].Get<string>(), false);
        }

        [ActiveEvent(Name = "DoxygentDotNetShowClass")]
        protected void DoxygentDotNetShowClass(object sender, ActiveEventArgs e)
        {
            Node node = new Node();
            node["ModuleSettings"]["Class"].Value = e.Params["Class"].Value;
            ActiveEvents.Instance.RaiseLoadControl(
                "DoxygentDotNetViewDocsModules.ViewClass",
                "dynMid",
                node);
        }
    }
}



















