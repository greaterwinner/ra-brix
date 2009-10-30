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
using ASP = System.Web.UI.WebControls;
using Ra.Brix.Types;
using Components;

namespace LanguageEditModules
{
    [ActiveModule]
    public class EditLanguage : System.Web.UI.UserControl, IModule
    {
        protected global::Components.Grid grd;

        protected void grid_CellEdited(object sender, Grid.GridEditEventArgs e)
        {
            Language.Instance.ChangeValue(e.ID, null, e.NewValue.ToString());
        }

        [ActiveEvent(Name = "GetTipOfToday")]
        protected static void GetTipOfToday(object sender, ActiveEventArgs e)
        {
            const string tmp = @"
You can edit the current language of the portal yourself - if you have access to it.
This can be done within the ""Admin"" menu.
This will change that language globally for all users that are using your same language.

This might e particulary useful for updating languages that are either not explicitly a part
of your portal installation, or that your cannot find some other way.
";
            e.Params["Tip"]["TipOfEditLanguage"].Value = Language.Instance["TipOfEditLanguage", null, tmp];
        }

        public void InitialLoading(Node node)
        {
            Node tmp = new Node();
            tmp["Columns"]["Key"]["Caption"].Value = Language.Instance["LanguageEditKeyCaption", null, "Key"];
            tmp["Columns"]["Key"]["ControlType"].Value = "Label";
            tmp["Columns"][Language.Instance.UserLanguage]["Caption"].Value = Language.Instance.UserLanguage;
            tmp["Columns"][Language.Instance.UserLanguage]["ControlType"].Value = "TextAreaEdit";
            int idxNo = 0;
            foreach (string idx in Language.Instance.Keys)
            {
                tmp["Rows"]["Row" + idxNo]["ID"].Value = idx;
                tmp["Rows"]["Row" + idxNo]["Key"].Value = idx;
                tmp["Rows"]["Row" + idxNo][Language.Instance.UserLanguage].Value = Language.Instance[idx];
                idxNo += 1;
            }
            grd.DataSource = tmp;
        }

        public string GetCaption()
        {
            return "";
        }

        [ActiveEvent("GetHelpContents")]
        protected static void GetHelpContents(object sender, ActiveEventArgs e)
        {
            e.Params[Language.Instance["EditLanguageHelpLabel", null, "Edit Language Module"]].Value = "Help-AboutEditLanguageModule";
        }

        [ActiveEvent("Help-AboutEditLanguageModule")]
        protected static void Help_AboutEditLanguageModule(object sender, ActiveEventArgs e)
        {
            const string helpEditLanguageDefault = @"
<p>
The Edit Language Module can be used to edit the crrent language in the portal. If you want to 
edit another language than the current one, you need to switch to that language with the
""Select Language"" module.
</p>
<p>
If you switch to a language that doesn't have a translation yet, English will be used as
the default language until you edit it.
</p>
<p>
Most ""Keys"" have sane names which means you can guess which parts of the portal they
belong to. Like for instance a Menu item called ""Help"" would normally use ""ButtonHelp"" for
its language key.
</p>
";
            e.Params["Text"].Value = Language.Instance["EditLanguageModuleHelp", null, helpEditLanguageDefault];
        }
    }
}