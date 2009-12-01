/*
 * Ra-Brix - A Modular-based Framework for building 
 * Web Applications Copyright 2009 - Thomas Hansen 
 * thomas@ra-ajax.org. Unless permission is 
 * explicitly given this code is licensed under the 
 * GNU Affero GPL version 3 which can be found in the 
 * license.txt file on disc.
 * 
 */

using System;
using HelperGlobals;
using LanguageRecords;
using Ra.Brix.Loader;
using System.Collections.Generic;
using Ra.Brix.Types;
using Ra.Brix.Data;
using UserRecords;

namespace RolesController
{
    [ActiveController]
    public class RoleController
    {
        private List<AccessEntity> _accessEntities;

        [ActiveEvent(Name = "ApplicationStartup")]
        protected static void ApplicationStartup(object sender, ActiveEventArgs e)
        {
            // Checking to see if our default roles exists, and if not we create them...
            if (ActiveType<Role>.Count == 0)
            {
                // Creating default roles
                Role admin = new Role {Name = "Administrator"};
                admin.Save();

                Role user = new Role {Name = "User"};
                user.Save();

                Role view = new Role {Name = "View"};
                view.Save();

                Role blocked = new Role {Name = "Blocked"};
                blocked.Save();

                Role noRole = new Role {Name = "Everyone"};
                noRole.Save();
            }

            // Settings default language values for module...
            Language.Instance.SetDefaultValue("ButtonViewAllRoles", "View All Roles");
            Language.Instance.SetDefaultValue("ButtonCreateRole", "Create Role");
            Language.Instance.SetDefaultValue("ButtonAccessControl", "Access Control");
        }

        [ActiveEvent(Name = "GetMenuItems")]
        protected void GetMenuItems(object sender, ActiveEventArgs e)
        {
            Language.Instance.SetDefaultValue("ButtonRoles", "Roles");
            e.Params["ButtonAdmin"]["ButtonRoles"].Value = "Menu-AdministrateRoles";
            e.Params["ButtonAdmin"]["ButtonRoles"]["ButtonViewAllRoles"].Value = "Menu-ViewAllRoles";
            e.Params["ButtonAdmin"]["ButtonRoles"]["ButtonCreateRole"].Value = "Menu-CreateRole";
            e.Params["ButtonAdmin"]["ButtonRoles"]["ButtonAccessControl"].Value = "Menu-AccessControl";
        }

        [ActiveEvent(Name = "DefaultCMSContentCreated")]
        protected static void DefaultCMSContentCreated(object sender, ActiveEventArgs e)
        {
            AccessEntity a = new AccessEntity();
            a.MenuValue = "url:~/";
            a.RoleName = "Everyone";
            a.Save();
        }

        [ActiveEvent(Name = "CheckAccessToMenuItem")]
        protected void CheckAccessToMenuItem(object sender, ActiveEventArgs e)
        {
            _accessEntities = new List<AccessEntity>(ActiveType<AccessEntity>.Select(Criteria.Eq("MenuValue", e.Params["MenuValue"].Value)));

            User current = null;
            if (!string.IsNullOrEmpty(Users.LoggedInUserName))
            {
                current = ActiveType<User>.SelectFirst(Criteria.Eq("Username", Users.LoggedInUserName));
            }

            bool hasAccess;
            if (current == null)
            {
                hasAccess = _accessEntities.Exists(
                    delegate(AccessEntity idx)
                        {
                            return idx.RoleName == "Everyone";
                        });
            }
            else
            {
                hasAccess = current.InRole("Administrator") || _accessEntities.Exists(
                    delegate(AccessEntity idx)
                        {
                            return current.InRole(idx.RoleName);
                        });
            }

            // Returning access back...
            e.Params["DeniedAccess"].Value = !hasAccess;
        }

        [ActiveEvent(Name = "FilterMenuItems")]
        protected void FilterMenuItems(object sender, ActiveEventArgs e)
        {
            User current = null;
            if (!string.IsNullOrEmpty(Users.LoggedInUserName))
            {
                current = ActiveType<User>.SelectFirst(Criteria.Eq("Username", Users.LoggedInUserName));
            }

            // We do NOT filter menu items at all if user is in Administrator role...
            if (current != null && current.InRole("Administrator"))
                return;

            _accessEntities = new List<AccessEntity>(ActiveType<AccessEntity>.Select());
            List<string> dnaCodesToRemove = new List<string>();
            foreach (Node idx in e.Params)
            {
                FilterMenuItems(idx, current, dnaCodesToRemove);
            }
            dnaCodesToRemove.Sort(
                delegate(string left, string right)
                    {
                        return right.CompareTo(left);
                    });
            foreach (string idxDna in dnaCodesToRemove)
            {
                Node tmp = e.Params.Find(idxDna);
                if (tmp != null)
                    tmp.Parent.Remove(tmp);
            }
        }

        private void FilterMenuItems(Node node, User current, ICollection<string> dnaCodeToRemove)
        {
            if (node.Name == "Params")
                return;
            string menuValue = node.Get<string>();
            bool hasAccess = true;
            if (!string.IsNullOrEmpty(menuValue))
            {
                hasAccess = false;
                foreach (AccessEntity idx in _accessEntities)
                {
                    if (idx.MenuValue == menuValue && idx.RoleName == "Everyone")
                    {
                        hasAccess = true;
                        break;
                    }
                    if (idx.MenuValue != menuValue)
                        continue;
                    if (current == null || !current.InRole(idx.RoleName))
                        continue;
                    if ((node.Count > 0 &&
                         node[0].Name == "Params" &&
                         node[0].Get<string>() != idx.Params))
                        continue;
                    hasAccess = true;
                    break;
                }
            }
            if (!hasAccess)
            {
                // Removing node OUT from menu collection...!
                dnaCodeToRemove.Add(node.DNA);
            }
            foreach (Node idx in node)
            {
                FilterMenuItems(idx, current, dnaCodeToRemove);
            }
        }

        [ActiveEvent(Name = "CreateNewRole")]
        protected void CreateNewRole(object sender, ActiveEventArgs e)
        {
            string roleName = e.Params["RoleName"].Get<string>();
            Role role = new Role {Name = roleName};
            role.Save();

            ActiveEvents.Instance.RaiseClearControls("dynPopup");
            ActiveEvents.Instance.RaiseActiveEvent(
                this, 
                "Menu-ViewAllRoles");
        }

        [ActiveEvent(Name = "DeleteRole")]
        protected void DeleteRole(object sender, ActiveEventArgs e)
        {
            int roleId = int.Parse(e.Params["ID"].Get<string>());
            Role role = ActiveType<Role>.SelectByID(roleId);
            role.Delete();
        }

        [ActiveEvent(Name = "Menu-AccessControl")]
        protected void AccessControl(object sender, ActiveEventArgs e)
        {
            Node node = new Node();
            node["TabCaption"].Value = Language.Instance["AccessControlCaption", null, "Access Control"];
            int idxNo = 0;
            foreach (Role idx in ActiveType<Role>.Select())
            {
                node["ModuleSettings"]["Roles"]["Role" + idxNo].Value = idx.Name;
                idxNo += 1;
            }
            ActiveEvents.Instance.RaiseActiveEvent(
                this,
                "GetMenuItems",
                node["ModuleSettings"]["MenuItems"]);

            idxNo = 0;
            foreach (AccessEntity idx in ActiveType<AccessEntity>.Select())
            {
                node["ModuleSettings"]["Access"]["Access" + idxNo]["MenuValue"].Value = idx.MenuValue;
                node["ModuleSettings"]["Access"]["Access" + idxNo]["RoleName"].Value = idx.RoleName;
                node["ModuleSettings"]["Access"]["Access" + idxNo]["Params"].Value = idx.Params;
                idxNo += 1;
            }

            ActiveEvents.Instance.RaiseLoadControl(
                "RolesModules.AccessControl",
                "dynMid",
                node);
        }

        [ActiveEvent(Name = "DeleteRoleMenuMapping")]
        protected void DeleteRoleMenuMapping(object sender, ActiveEventArgs e)
        {
            string roleName = e.Params["RoleName"].Get<string>();
            string menuValue = e.Params["MenuValue"].Get<string>();

            AccessEntity a = ActiveType<AccessEntity>.SelectFirst(
                Criteria.Eq("RoleName", roleName),
                Criteria.Eq("MenuValue", menuValue));
            a.Delete();

            // Then we need to traverse all the MenuItem which are CHILDREN of this one
            // and delete the Access object for the given role to all those too...!
            // Since when a node is not accessible for a Role, then no child menu node
            // should be accessible either...!
            Node menuItems = new Node();
            ActiveEvents.Instance.RaiseActiveEvent(
                this,
                "GetMenuItems",
                menuItems);
            Node menuValueNodeToRemove = menuItems.Find(
                delegate(Node idx)
                    {
                        return (string)idx.Value == menuValue;
                    });
            foreach (Node idx in menuValueNodeToRemove)
            {
                DeleteRoleFromChild(idx, roleName);
            }

            // Sending back updates...
            int idxNo = 0;

            // Dummy to make sure the "Access" node actually is there...!
            e.Params["Access"].Value = true;
            foreach (AccessEntity idx in ActiveType<AccessEntity>.Select())
            {
                e.Params["Access"]["Access" + idxNo]["MenuValue"].Value = idx.MenuValue;
                e.Params["Access"]["Access" + idxNo]["RoleName"].Value = idx.RoleName;
                idxNo += 1;
            }
        }

        private static void DeleteRoleFromChild(Node idx, string roleName)
        {
            AccessEntity a = ActiveType<AccessEntity>.SelectFirst(
                Criteria.Eq("RoleName", roleName),
                Criteria.Eq("MenuValue", idx.Value));
            if (a != null)
            {
                a.Delete();
            }
            foreach (Node idx2 in idx)
            {
                DeleteRoleFromChild(idx2, roleName);
            }
        }

        [ActiveEvent(Name = "CreateNewRoleMenuMapping")]
        protected void CreateNewRoleMenuMapping(object sender, ActiveEventArgs e)
        {
            string roleName = e.Params["RoleName"].Get<string>();
            string menuValue = e.Params["MenuValue"].Get<string>();
            string pars = e.Params["Params"].Get<string>();

            List<Criteria> crit = new List<Criteria>
            {
                Criteria.Eq("MenuValue", menuValue),
                Criteria.Eq("RoleName", roleName)
            };
            if( pars != null)
                crit.Add(Criteria.Eq("Params", pars));

            if (string.IsNullOrEmpty(menuValue))
            {
                Node node = new Node();
                node["Message"].Value = Language.Instance["RoleYouCanNotInfo", null, @"You cannot change the accessibility of that 
item since it has no 'Value' property"];
                node["Duration"].Value = 2000;
                ActiveEvents.Instance.RaiseActiveEvent(
                    this,
                    "ShowInformationMessage",
                    node);
            }
            else if (ActiveType<AccessEntity>.CountWhere(crit.ToArray()) > 0)
            {
                Node node = new Node();
                node["Message"].Value = String.Format(Language.Instance["RoleAlreadyAssociatedInfo", null, @"Role; '{0}' already associated with item..."], roleName);
                node["Duration"].Value = 2000;
                ActiveEvents.Instance.RaiseActiveEvent(
                    this,
                    "ShowInformationMessage",
                    node);
            }
            else
            {
                AccessEntity a = new AccessEntity
                {
                    MenuValue = menuValue, 
                    RoleName = roleName, 
                    Params = pars
                };
                a.Save();

                // Running upwards in the hierarchy and creating new access objects for all
                // parents recursively, since otherwise there's no point at all in adding
                // the access to the menu item...
                Node parent = e.Params["Parents", false];
                while (parent != null)
                {
                    string parentRoleName = parent["RoleName"].Get<string>();
                    string parentMenuValue = parent["MenuValue"].Get<string>();
                    if (parentMenuValue == null)
                        break;
                    if (ActiveType<AccessEntity>.CountWhere(
                            Criteria.Eq("RoleName", parentRoleName),
                            Criteria.Eq("MenuValue", parentMenuValue)) == 0)
                    {
                        AccessEntity aParent = new AccessEntity
                        {
                            RoleName = parentRoleName,
                            MenuValue = parentMenuValue
                        };
                        aParent.Save();
                    }
                    parent = parent["Parents", false];
                }

                // Sending back updates...
                int idxNo = 0;
                foreach (AccessEntity idx in ActiveType<AccessEntity>.Select())
                {
                    e.Params["Access"]["Access" + idxNo]["MenuValue"].Value = idx.MenuValue;
                    e.Params["Access"]["Access" + idxNo]["RoleName"].Value = idx.RoleName;
                    e.Params["Access"]["Access" + idxNo]["Params"].Value = idx.Params;
                    idxNo += 1;
                }
            }
        }

        [ActiveEvent(Name = "Menu-CreateRole")]
        protected void CreateRole(object sender, ActiveEventArgs e)
        {
            Node init = new Node();
            init["TabCaption"].Value = Language.Instance["RoleCreateCaption", null, "Create New Role"];
            init["Width"].Value = 250;
            init["Height"].Value = 130;
            ActiveEvents.Instance.RaiseLoadControl(
                "RolesModules.CreateNewRole",
                "dynPopup",
                init);
        }

        [ActiveEvent(Name = "Menu-ViewAllRoles")]
        protected void ViewAllRoles(object sender, ActiveEventArgs e)
        {
            Node init = new Node();
            init["TabCaption"].Value = Language.Instance["RoleViewAllCaption", null, "View all Roles"];

            init["ModuleSettings"]["Grid"]["Columns"]["Role"]["Caption"].Value = Language.Instance["Role"];
            init["ModuleSettings"]["Grid"]["Columns"]["Role"]["ControlType"].Value = "Label";

            int idxNo = 0;
            foreach (Role idx in ActiveType<Role>.Select())
            {
                init["ModuleSettings"]["Grid"]["Rows"]["Row" + idxNo]["ID"].Value = idx.ID;
                init["ModuleSettings"]["Grid"]["Rows"]["Row" + idxNo]["Role"].Value = idx.Name;
                idxNo += 1;
            }
            ActiveEvents.Instance.RaiseLoadControl(
                "RolesModules.ViewAllRoles",
                "dynMid",
                init);
        }
    }
}