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
using CMSRecords;
using Ra.Brix.Types;
using Ra.Brix.Data;
using System.Web;
using Ra;

namespace CMSController
{
    [ActiveController]
    public class CMSController
    {
        [ActiveEvent(Name = "ApplicationStartup")]
        protected static void ApplicationStartup(object sender, ActiveEventArgs e)
        {
            Language.Instance.SetDefaultValue("ButtonAdmin", "Admin");
            Language.Instance.SetDefaultValue("ButtonCMS", "CMS");
            Language.Instance.SetDefaultValue("ButtonCMSViewPages", "View Pages");
            Language.Instance.SetDefaultValue("ButtonCMSCreatePage", "Create Page");
            Language.Instance.SetDefaultValue(
                "CMSPageDeleted",
                "CMS Page was deleted. Application had to be refreshed");

            // Creating default page - if necessary...!
            if (ActiveType<Page>.Count == 0)
            {
                Page p = new Page();
                p.Header = "Home";
                p.Body = string.Format(@"
<a href=""http://ra-brix.org"" title=""Visit the Ra-Brix website"">
<img src=""media/skins/Gold/Images/bazaar.png"" style=""float:right;margin-left:10px;margin-top:-40px;"" alt=""Visit the Ra-Brix Bazaar"" />
</a>
<p>
    Welcome to Ra-Brix. This is the default page which has been created for you.
</p>
<p>
    The first time you login a user will be created for you with the given OpenID identity
    and the first user to login will become an Administrator and Power User automatically.
</p>
<p>
    If you don't know what OpenID is then you can create an OpenID (and get a good explanation about
    it) at <a href=""http://myopenid.com"" target=""_blank"">MyOpenID.com</a>...
</p>
<p>
    Then when you have logged in, you will want to visit our Bazaar where you can find a lot
    of modules and components you can install in your portal. Some for free and others for
    a fee.
</p>
<p>
    Tutorials, more downloads, updates and videos about how to use Ra-Brix can be 
    found at <a href=""http://ra-brix.org"">the Ra-Brix website</a>.
</p>
");
                p.URL = "home";
                p.HideFromMenu = false;
                p.Save();
                ActiveEvents.Instance.RaiseActiveEvent(
                    typeof(CMSController),
                    "DefaultCMSContentCreated");
            }
        }

        [ActiveEvent(Name = "GetMenuItems")]
        protected void GetMenuItems(object sender, ActiveEventArgs e)
        {
            e.Params["ButtonAdmin"]["ButtonCMS"].Value = "Menu-CMS";
            e.Params["ButtonAdmin"]["ButtonCMS"]["ButtonCMSViewPages"].Value = "Menu-CMS-ViewPages";
            e.Params["ButtonAdmin"]["ButtonCMS"]["ButtonCMSCreatePage"].Value = "Menu-CMS-CreatePage";

            // Pages...
            foreach (Page idx in ActiveType<Page>.Select(Criteria.NotLike("URL", "%/%")))
            {
                AddPageToMenu(idx, e.Params);
            }
        }

        [ActiveEvent(Name = "CMSCreateNewLink")]
        protected void CMSCreateNewLink(object sender, ActiveEventArgs e)
        {
            ActiveEvents.Instance.RaiseClearControls("dynPopup");

            Node node = new Node();
            node["URL"].Value = e.Params["URL"].Value;
            ActiveEvents.Instance.RaiseActiveEvent(
                this,
                "CMSInsertLink",
                node);
        }

        [ActiveEvent(Name = "CMSGetHyperLinkDialog")]
        protected void CMSGetHyperLinkDialog(object sender, ActiveEventArgs e)
        {
            Node node = new Node();
            node["TabCaption"].Value =
                Language.Instance["CreateHyperLink", null, "Create HyperLink"];
            node["Width"].Value = 300;
            node["Height"].Value = 100;
            int idxNo = 0;
            foreach (Page idx in ActiveType<Page>.Select())
            {
                node["ModuleSettings"]["Pages"]["Page" + idxNo]["Header"].Value = idx.Header;
                if (idx.URL == "home")
                {
                    node["ModuleSettings"]["Pages"]["Page" + idxNo]["URL"].Value = "/";
                }
                else
                {
                    node["ModuleSettings"]["Pages"]["Page" + idxNo]["URL"].Value = idx.URL + ".aspx";
                }
                idxNo += 1;
            }
            ActiveEvents.Instance.RaiseLoadControl(
                "CMSModules.CreateHyperLink",
                "dynPopup",
                node);
        }

        [ActiveEvent(Name = "CMSGetImageDialog")]
        protected void CMSGetImageDialog(object sender, ActiveEventArgs e)
        {
            Node node = new Node();
            node["TabCaption"].Value =
                Language.Instance["SelectImage", null, "Select Image"];
            node["Width"].Value = 800;
            node["Height"].Value = 310;
            node["ModuleSettings"]["Mode"].Value = "Select";
            node["ModuleSettings"]["EventToRaise"].Value = "FileExplorerImageFileChosen";
            ActiveEvents.Instance.RaiseLoadControl(
                "ResourcesModules.Explorer",
                "dynPopup",
                node);
        }

        [ActiveEvent(Name = "FileExplorerImageFileChosen")]
        protected void FileExplorerFileChosen(object sender, ActiveEventArgs e)
        {
            string fileName = e.Params["File"].Get<string>();
            fileName = fileName.Replace(HttpContext.Current.Server.MapPath("~/"), "");
            fileName = fileName.Replace("\\", "/");
            Node node = new Node();
            node["File"].Value = fileName;
            ActiveEvents.Instance.RaiseActiveEvent(
                this,
                "ImageFileChosenByFileDialog",
                node);
            ActiveEvents.Instance.RaiseClearControls("dynPopup");
        }

        [ActiveEvent(Name = "CMSGetResourceDialog")]
        protected void CMSGetResourceDialog(object sender, ActiveEventArgs e)
        {
            Node node = new Node();
            node["TabCaption"].Value =
                Language.Instance["SelectImage", null, "Select Image"];
            node["Width"].Value = 800;
            node["Height"].Value = 310;
            node["ModuleSettings"]["Mode"].Value = "Select";
            node["ModuleSettings"]["EventToRaise"].Value = "FileExplorerResourceFileChosen";
            ActiveEvents.Instance.RaiseLoadControl(
                "ResourcesModules.Explorer",
                "dynPopup",
                node);
        }

        [ActiveEvent(Name = "FileExplorerResourceFileChosen")]
        protected void FileExplorerResourceFileChosen(object sender, ActiveEventArgs e)
        {
            string fileName = e.Params["File"].Get<string>();
            fileName = fileName.Replace(HttpContext.Current.Server.MapPath("~/"), "");
            fileName = fileName.Replace("\\", "/");
            Node node = new Node();
            node["File"].Value = fileName;
            ActiveEvents.Instance.RaiseActiveEvent(
                this,
                "ResourceFileChosenByFileDialog",
                node);
            ActiveEvents.Instance.RaiseClearControls("dynPopup");
        }

        [ActiveEvent(Name = "CMSCreateNewPage")]
        protected void CMSCreateNewPage(object sender, ActiveEventArgs e)
        {
            string name = e.Params["Name"].Get<string>();
            string parentURL = e.Params["Parent"].Get<string>();

            if (parentURL == "xxx-no-parent")
            {
                Page n = new Page();
                n.Header = name;
                n.Body = "<p>Default Text - change this...</p>";
                n.Save();
            }
            else
            {
                parentURL = parentURL.Replace("xxx-value", "");
                Page p = ActiveType<Page>.SelectFirst(Criteria.Eq("URL", parentURL));
                Page n = new Page();
                n.Header = name;
                p.Children.Add(n);
                p.Save();
            }
#pragma warning disable 168
            // Making sure we're getting the language correctly...
            string tmpBugger = Language.Instance["CMSPageCreated", null, "CMS Page was created. Application had to be refreshed"];
#pragma warning restore 168
            AjaxManager.Instance.Redirect("~/?message=CMSPageCreated");
        }

        private void AddPageToMenu(Page page, Node node)
        {
            if(page.HideFromMenu)
                return;
            string url = page.URL;
            if (url == "home")
            {
                url = "";
            }
            else
            {
                url += ".aspx";
            }
            url = "~/" + url;

            node[page.Header].Value = "url:" + url;
            foreach (Page idx in page.Children)
            {
                AddPageToMenu(idx, node[page.Header]);
            }
        }

        [ActiveEvent(Name = "CMSRequestDeletePage")]
        protected void CMSRequestDeletePage(object sender, ActiveEventArgs e)
        {
            Node init = new Node();
            init["TabCaption"].Value = Language.Instance["AreYouSure", null, "Are you sure?"];
            init["Width"].Value = 350;
            init["Height"].Value = 130;
            init["ModuleSettings"]["Text"].Value = 
                Language.Instance["ReallyDeleteCMSPage", null, 
                @"Do you really want to delete the selected page?"];
            init["ModuleSettings"]["EventToRaiseOnOK"].Value = "CMSDeletePage";
            init["ModuleSettings"]["Params"]["Name"].Value = "URL";
            init["ModuleSettings"]["Params"]["Value"].Value = e.Params["URL"].Get<string>();
            ActiveEvents.Instance.RaiseLoadControl(
                "CommonModules.MessageBox",
                "dynPopup",
                init);
        }

        [ActiveEvent(Name = "CMSDeletePage")]
        protected void CMSDeletePage(object sender, ActiveEventArgs e)
        {
            string url = e.Params["URL"].Get<string>();
            if (url == "home")
            {
                Node nodeMessage = new Node();
                nodeMessage["Message"].Value = Language.Instance[
                    "CannotDeleteHomePage", 
                    null, 
                    "You cannot delete the main landing page"];
                nodeMessage["Duration"].Value = 2000;
                ActiveEvents.Instance.RaiseActiveEvent(
                    this,
                    "ShowInformationMessage",
                    nodeMessage);
            }
            else
            {
                Page p = ActiveType<Page>.SelectFirst(Criteria.Eq("URL", url));
                p.Delete();

                // Refreshing page, with informational notification...
                AjaxManager.Instance.Redirect("~/?message=CMSPageDeleted");
            }
        }

        [ActiveEvent(Name = "CMSSaveEditedPage")]
        protected void CMSSaveEditedPage(object sender, ActiveEventArgs e)
        {
            string url = e.Params["URL"].Get<string>();
            string header = e.Params["Header"].Get<string>();
            string body = e.Params["Body"].Get<string>();
            bool hideFromMenu = e.Params["HideFromMenu"].Get<bool>();
            bool hideFromHeader = e.Params["HideFromHeader"].Get<bool>();
            Page p = ActiveType<Page>.SelectFirst(Criteria.Eq("URL", url));
            p.Header = header;
            p.Body = body;
            p.HideFromMenu = hideFromMenu;
            p.HideFromHeader = hideFromHeader;
            p.Save();

            // Showing info message...
            Node nodeMessage = new Node();
            string msg = string.Format(@"
'{0}' was save. Click the link to preview it.", header);
            nodeMessage["Message"].Value = msg;
            nodeMessage["Duration"].Value = 1000;
            ActiveEvents.Instance.RaiseActiveEvent(
                this,
                "ShowInformationMessage",
                nodeMessage);
        }

        [ActiveEvent(Name = "Menu-CMS-CreatePage")]
        protected void CMSCreatePage(object sender, ActiveEventArgs e)
        {
            Node node = new Node();
            node["TabCaption"].Value = Language.Instance["CMSCreateNewPage", null, "Create New Page"];
            int idxNo = 0;
            foreach (Page idx in ActiveType<Page>.Select())
            {
                node["ModuleSettings"]["Pages"]["Page" + idxNo]["Name"].Value = idx.Header;
                node["ModuleSettings"]["Pages"]["Page" + idxNo]["URL"].Value = "xxx-value" + idx.URL;
                idxNo += 1;
            }
            ActiveEvents.Instance.RaiseLoadControl(
                "CMSModules.CreatePage",
                "dynPopup",
                node);
        }

        [ActiveEvent(Name = "Menu-CMS-ViewPages")]
        protected void CMSViewPages(object sender, ActiveEventArgs e)
        {
            ShowViewPages();
        }

        private void ShowViewPages()
        {
            Node node = new Node();
            node["TabCaption"].Value = Language.Instance["CMS-ViewPages", null, "View CMS-Pages"];
            int idxNo = 0;
            foreach (Page idx in ActiveType<Page>.Select(Criteria.NotLike("URL", "%/%")))
            {
                AddPageToNode(node["ModuleSettings"]["Pages"]["Page" + idxNo], idx);
                idxNo += 1;
            }
            ActiveEvents.Instance.RaiseLoadControl(
                "CMSModules.ViewPages",
                "dynMid",
                node);
        }

        private void AddPageToNode(Node node, Page page)
        {
            node["Page"]["Name"].Value = page.Header;
            node["Page"]["URL"].Value = page.URL;
            int idxNo = 0;
            foreach (Page idx in page.Children)
            {
                AddPageToNode(node["Page"]["Children"]["Child" + idxNo], idx);
                idxNo += 1;
            }
        }

        [ActiveEvent(Name = "CMSGetBodyForURL")]
        protected void CMSGetBodyForURL(object sender, ActiveEventArgs e)
        {
            Page p = ActiveType<Page>.SelectFirst(Criteria.Eq("URL", e.Params["URL"].Get<string>()));
            e.Params["Body"].Value = p.Body;
            e.Params["HideFromMenu"].Value = p.HideFromMenu;
        }

        [ActiveEvent(Name = "Page_Init_InitialLoading")]
        protected void InitialLoadingOfPage(object sender, ActiveEventArgs e)
        {
            string contentId = HttpContext.Current.Request.Params["ContentID"];
            if (contentId != null)
            {
                contentId = contentId.Trim('/');
            }
            Page page = Page.FindPage(contentId);

            if (page != null)
            {
                string pageUrl = page.URL == "home" ? "" : (page.URL + ".aspx");
                string urlOfPage = "url:~/" + pageUrl;
                Node access = new Node();
                access["MenuValue"].Value = urlOfPage;
                ActiveEvents.Instance.RaiseActiveEvent(
                    this,
                    "CheckAccessToMenuItem",
                    access);
                if (access["DeniedAccess"].Value != null && access["DeniedAccess"].Get<bool>())
                {
                    // No access
                    Node node = new Node();
                    node["TabCaption"].Value = Language.Instance["NoAcces", null, "No Access...!"];
                    node["ModuleSettings"]["Header"].Value = Language.Instance["NoAcces", null, "No Access...!"];
                    node["ModuleSettings"]["Content"].Value = Language.Instance["NoAccesDetailed", null, @"
You do not have access to this page, and hence it is not displayed...!"];
                    ActiveEvents.Instance.RaiseLoadControl(
                        "CMSModules.NormalContent",
                        "dynMid",
                        node);
                }
                else
                {
                    // Access
                    Node node = new Node();
                    node["TabCaption"].Value = page.Header;
                    node["ModuleSettings"]["Header"].Value = page.Header;
                    node["ModuleSettings"]["Content"].Value = page.Body;
                    node["ModuleSettings"]["HideHeader"].Value = page.HideFromHeader;

                    ActiveEvents.Instance.RaiseLoadControl(
                        "CMSModules.NormalContent",
                        "dynMid",
                        node);
                }
            }
        }
    }
}
