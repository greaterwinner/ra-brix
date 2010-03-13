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

namespace DoxygenDotNetViewDocsController
{
    [ActiveController]
    public class ViewDocsController
    {
        [ActiveEvent(Name = "ApplicationStartup")]
        protected static void ApplicationStartup(object sender, ActiveEventArgs e)
        {
            Language.Instance.SetDefaultValue("ButtonDocumentation", "Documentation");
        }

        [ActiveEvent(Name = "GetMenuItems")]
        protected void GetMenuItems(object sender, ActiveEventArgs e)
        {
            e.Params["ButtonDocumentation"].Value = "Menu-ViewDocumentation";
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
            Node node = new Node();
            using (TextReader reader = new StreamReader(e.Params["TutorialFile"].Get<string>()))
            {
                CodeColorizer colorizer = ColorizerLibrary.Config.DOMConfigurator.Configure();
                string tutorialSyntaxed = colorizer.ProcessAndHighlightText(reader.ReadToEnd());
                node["ModuleSettings"]["Text"].Value = tutorialSyntaxed;
                node["ModuleSettings"]["Header"].Value = e.Params["TutorialName"].Get<string>();
            }
            ActiveEvents.Instance.RaiseLoadControl(
                "DoxygentDotNetViewDocsModules.ViewTutorial",
                "dynMid",
                node);
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



















