/*
 * Ra-Brix - A Modular-based Framework for building 
 * Web Applications Copyright 2009 - Thomas Hansen 
 * thomas@ra-ajax.org. Unless permission is 
 * explicitly given this code is licensed under the 
 * GNU Affero GPL version 3 which can be found in the 
 * license.txt file on disc.
 * 
 */

using ADGroups2RolesRecords;
using LanguageRecords;
using Ra.Brix.Loader;
using Ra.Brix.Types;
using Ra.Brix.Data;
using UserRecords;

namespace ADGroup2RoleController
{
    [ActiveController]
    public class AdGroups2RoleController
    {
        [ActiveEvent(Name = "ApplicationStartup")]
        protected static void ApplicationStartup(object sender, ActiveEventArgs e)
        {
            Language.Instance.SetDefaultValue("ButtonAdmin", "Admin");
            Language.Instance.SetDefaultValue("ButtonRoles", "Roles");
            Language.Instance.SetDefaultValue("ButtonADMappings", "AD-Mappings");
            Language.Instance.SetDefaultValue("ButtonShowADMappings", "View Mappings");
            Language.Instance.SetDefaultValue("ButtonCreateADMapping", "Create New");
        }

        [ActiveEvent(Name = "GetMenuItems")]
        protected void GetMenuItems(object sender, ActiveEventArgs e)
        {
            e.Params["ButtonAdmin"]["ButtonRoles"]["ButtonADMappings"].Value = "Menu-AD-MappingsRoot";
            e.Params["ButtonAdmin"]["ButtonRoles"]["ButtonADMappings"]["ButtonShowADMappings"].Value = "Menu-MapRoles";
            e.Params["ButtonAdmin"]["ButtonRoles"]["ButtonADMappings"]["ButtonCreateADMapping"].Value = "Menu-MapRolesCreateNew";
        }

        [ActiveEvent("Menu-MapRolesCreateNew")]
        protected static void HelpAboutAdGroupsRoleMapper(object sender, ActiveEventArgs e)
        {
            Node init = new Node();
            init["TabCaption"].Value = Language.Instance["ADGroups2RoleCreateCaption", null, "New AD Mapping"];
            init["Width"].Value = 350;
            init["Height"].Value = 130;
            ActiveEvents.Instance.RaiseLoadControl(
                "ADGroups2RolesModules.CreateNew",
                "dynPopup",
                init);
        }

        [ActiveEvent(Name = "DeleteADGroup2RoleMappin")]
        protected void DeleteAdGroup2RoleMappin(object sender, ActiveEventArgs e)
        {
            int id = int.Parse(e.Params["ID"].Get<string>());
            AdGroup2Role a = ActiveType<AdGroup2Role>.SelectByID(id);
            a.Delete();
        }

        [ActiveEvent(Name = "CreateNewAD2RoleMapping")]
        protected void CreateNewAd2RoleMapping(object sender, ActiveEventArgs e)
        {
            AdGroup2Role ad = new AdGroup2Role();
            ad.GroupName = e.Params["ADGroupName"].Get<string>();
            ad.Save();

            ActiveEvents.Instance.RaiseClearControls("dynPopup");
            ActiveEvents.Instance.RaiseActiveEvent(
                this,
                "Menu-ViewAllRoles");

            OpenAdMapperModule();
        }

        private static void OpenAdMapperModule()
        {
            // Showing all mappings...!
            Node init = new Node();
            init["TabCaption"].Value = Language.Instance["ADGroups2RoleMapRolesCaption", null, "Map Roles"];

            init["ModuleSettings"]["Grid"]["Columns"]["GroupName"]["Caption"].Value = Language.Instance["ADGroups2RoleGroupCaption", null, "Group Name"];
            init["ModuleSettings"]["Grid"]["Columns"]["GroupName"]["ControlType"].Value = "InPlaceEdit";

            init["ModuleSettings"]["Grid"]["Columns"]["RoleName"]["Caption"].Value = Language.Instance["ADGroups2RoleCaption", null, "Role Name"];
            init["ModuleSettings"]["Grid"]["Columns"]["RoleName"]["ControlType"].Value = "List";
            int idxNo = 0;
            foreach (Role idx in ActiveType<Role>.Select())
            {
                init["ModuleSettings"]["Grid"]["Columns"]["RoleName"]["Values"]["Value" + idxNo].Value = idx.Name;
                idxNo += 1;
            }
            idxNo = 0;
            foreach (AdGroup2Role idx in ActiveType<AdGroup2Role>.Select())
            {
                init["ModuleSettings"]["Grid"]["Rows"]["Row" + idxNo]["ID"].Value = idx.ID;
                init["ModuleSettings"]["Grid"]["Rows"]["Row" + idxNo]["GroupName"].Value = idx.GroupName;
                init["ModuleSettings"]["Grid"]["Rows"]["Row" + idxNo]["RoleName"].Value = idx.RoleName;
                idxNo += 1;
            }

            ActiveEvents.Instance.RaiseLoadControl(
                "ADGroups2RolesModules.ViewMappings",
                "dynMid",
                init);
        }

        [ActiveEvent(Name = "Menu-MapRoles")]
        protected void MapRoles(object sender, ActiveEventArgs e)
        {
            OpenAdMapperModule();
        }

        [ActiveEvent(Name = "AfterUserLoggedIn")]
        protected void UserLoggedIn(object sender, ActiveEventArgs e)
        {
            string username = e.Params.Value.ToString();
            User user = ActiveType<User>.SelectFirst(Criteria.Eq("Username", username));
            if (e.Params["Groups"].Count > 0)
            {
                user.Roles.RemoveAll(
                    delegate(Role idx)
                        {
                            return idx.Name != "Administrator";
                        });
                foreach (Node idx in e.Params["Groups"])
                {
                    string groupName = idx.Get<string>();
                    if (ActiveType<AdGroup2Role>.CountWhere(Criteria.Eq("GroupName", groupName)) == 0)
                    {
                        AdGroup2Role newMapping = new AdGroup2Role();
                        newMapping.GroupName = groupName;
                        newMapping.Save();
                    }
                    foreach (AdGroup2Role idxMap in ActiveType<AdGroup2Role>.Select(Criteria.Eq("GroupName", groupName)))
                    {
                        string roleName = idxMap.RoleName;
                        if (!string.IsNullOrEmpty(roleName))
                        {
                            Role role = ActiveType<Role>.SelectFirst(Criteria.Eq("Name", roleName));
                            if (role != null)
                            {
                                user.Roles.Add(role);
                            }
                        }
                    }
                }
                user.Save();
            }
        }

        [ActiveEvent(Name = "ChangeADGroupToRoleMapping")]
        protected void ChangeAdGroupToRoleMapping(object sender, ActiveEventArgs e)
        {
            string roleName = e.Params["RoleName"].Get<string>();
            string groupName = e.Params["GroupName"].Get<string>();
            int id = int.Parse(e.Params["ID"].Get<string>());
            AdGroup2Role a = ActiveType<AdGroup2Role>.SelectByID(id);

            if (!string.IsNullOrEmpty(groupName))
            {
                a.GroupName = groupName;
            }
            if (!string.IsNullOrEmpty(roleName))
            {
                if (ActiveType<Role>.CountWhere(Criteria.Eq("Name", roleName)) == 0)
                {
                    Node nodeInfo = new Node();
                    string message = "You must choose an existing role.<br/>Nothing was saved...!";
                    nodeInfo["Message"].Value = Language.Instance["ADGroups2RoleChooseExistingRoleInfo", null, message];
                    nodeInfo["Duration"].Value = 2000;
                    ActiveEvents.Instance.RaiseActiveEvent(
                        this,
                        "ShowInformationMessage",
                        nodeInfo);

                    e.Params["Failure"].Value = true;
                    return;
                }
                a.RoleName = roleName;
            }
            a.Save();
        }
    }
}