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
using ASP = System.Web.UI.WebControls;
using Components;


namespace ADGroups2RolesModules
{
    [ActiveModule]
    public class ViewMappings : System.Web.UI.UserControl, IModule
    {
        protected global::Components.Grid grd;

        [ActiveEvent(Name = "GetTipOfToday")]
        protected static void GetTipOfToday(object sender, ActiveEventArgs e)
        {
            const string tmp = @"
If you are using the integrated AD Login to OpenID features of the Portal, you can actually
map the AD Groups to Roles in the Portal. This works so that one AD Group will effectively 
""translate"" to a Role object within the Portal.

Then whenever someone logs into the Portal from the AD Login, the Roles that the User belongs
to will be overridden by the AD Mappings.
";
            e.Params["Tip"]["TipOfADMappings"].Value = Language.Instance["TipOfADMappings", null, tmp];
        }

        protected void grid_CellEdited(object sender, Grid.GridEditEventArgs e)
        {
            Node node = new Node();
            node["ID"].Value = e.ID;
            node[e.Key].Value = e.NewValue;
            ActiveEvents.Instance.RaiseActiveEvent(
                this,
                "ChangeADGroupToRoleMapping",
                node);
        }

        protected void grid_RowDeleted(object sender, Grid.GridActionEventArgs e)
        {
            Node node = new Node();
            node["ID"].Value = e.ID;
            ActiveEvents.Instance.RaiseActiveEvent(
                this,
                "DeleteADGroup2RoleMappin",
                node);
        }

        public void InitialLoading(Node node)
        {
            grd.DataSource = node["Grid"];
        }

        [ActiveEvent("GetHelpContents")]
        protected static void GetHelpContents(object sender, ActiveEventArgs e)
        {
            e.Params[Language.Instance["ADGroups2RolesHelpLabel", null, "AD-Groups to Roles"]].Value = "Help-AboutADGroupsRoleMapper";
        }

        [ActiveEvent("Help-AboutADGroupsRoleMapper")]
        protected static void HelpAboutAdGroupsRoleMapper(object sender, ActiveEventArgs e)
        {
            const string helpAdGroups2RolesDefault = @"
<p>
The AD-Groups to Role Mapper will make it possible for you to associate Roles in the portal
with Groups in AD. This means that you can administrate your Active Directory and then have
groups in AD map to roles in the portal.
</p>
<p>
Then whenever a user logs on through the AD mechanism, he will have all his roles updated
according to what groups in Active Directory he belongs to, and what Roles in the portal
those Groups are mapped to.
</p>
";

            e.Params["Text"].Value = Language.Instance["ADGroups2RolesHelp", null, helpAdGroups2RolesDefault];
        }
    }
}
