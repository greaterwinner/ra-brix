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

namespace ResourcesController
{
    [ActiveController]
    public class ResourcesController
    {
        [ActiveEvent(Name = "ApplicationStartup")]
        protected static void ApplicationStartup2(object sender, ActiveEventArgs e)
        {
            Language.Instance.SetDefaultValue("ButtonResourceManager", "Resources");
        }

        [ActiveEvent(Name = "GetMenuItems")]
        protected void GetMenuItems(object sender, ActiveEventArgs e)
        {
            e.Params["ButtonAppl"]["ButtonResourceManager"].Value = "Menu-ResourceManager";
        }

        [ActiveEvent(Name = "Menu-ResourceManager")]
        protected void OpenResourceManager(object sender, ActiveEventArgs e)
        {
            Node node = new Node();
            node["TabCaption"].Value = 
                Language.Instance["ResourceExplorer", null, "Resource Explorer"];
            ActiveEvents.Instance.RaiseLoadControl(
                "ResourcesModules.Explorer",
                "dynMid",
                node);
        }
    }
}
