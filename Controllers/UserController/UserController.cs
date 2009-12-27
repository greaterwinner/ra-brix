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
using Ra.Brix.Data;
using System.Web;
using SettingsRecords;
using UserRecords;

namespace UserController
{
    [ActiveController]
    public class UserController
    {
        [ActiveEvent(Name = "ApplicationStartup")]
        protected static void ApplicationStartup2(object sender, ActiveEventArgs e)
        {
            Language.Instance.SetDefaultValue("ButtonUsers", "Users");
            Language.Instance.SetDefaultValue("ButtonViewAllUsers", "View All");
            Language.Instance.SetDefaultValue("ButtonCreateNewUser", "Create New");
        }

        [ActiveEvent(Name = "GetMenuItems")]
        protected void GetMenuItems(object sender, ActiveEventArgs e)
        {
            if (HttpContext.Current.Request.Params["test"] != null)
            {
                e.Params["ButtonCreateUsersTest"].Value = "Menu-CreateManyUsers";
            }
            e.Params["ButtonAdmin"]["ButtonUsers"].Value = "Menu-AdminUsers";
            e.Params["ButtonAdmin"]["ButtonUsers"]["ButtonViewAllUsers"].Value = "Menu-ViewAllUsers";
            e.Params["ButtonAdmin"]["ButtonUsers"]["ButtonCreateNewUser"].Value = "Menu-CreateNewUser";
        }

        [ActiveEvent(Name="Menu-CreateManyUsers")]
        protected void CreateManyUsers(object sender, ActiveEventArgs e)
        {
            ActiveEvents.Instance.RaiseLoadControl(
                "UsersModules.CreateNewUsers",
                "dynPopup");
        }

        [ActiveEvent(Name = "EditUser")]
        protected void EditUser(object sender, ActiveEventArgs e)
        {
            User user = ActiveType<User>.SelectByID(int.Parse(e.Params["UserID"].Get<string>()));
            if (user == null)
            {
                Node nodeDoesntExist = new Node();
                nodeDoesntExist["Message"].Value = Language.Instance["UserNotExistInfo", null, @"That user doesn't exist...!"];
                nodeDoesntExist["Duration"].Value = 5000;
                ActiveEvents.Instance.RaiseActiveEvent(
                    this,
                    "ShowInformationMessage",
                    nodeDoesntExist);
            }
            else
            {
                Node init = new Node();
                init["TabCaption"].Value = Language.Instance["UserEditCaption", null, "Editing user; "] + user.Username;
                init["ModuleSettings"]["Username"].Value = user.Username;
                init["ModuleSettings"]["LastLoggedIn"].Value = user.LastLoggedIn;
                init["ModuleSettings"]["Email"].Value = user.Email;
                init["ModuleSettings"]["Phone"].Value = user.Phone;

                int idxNo = 0;
                foreach (Role idxRole in user.Roles)
                {
                    init["ModuleSettings"]["Roles"]["Role" + idxNo].Value = idxRole.Name;
                    idxNo += 1;
                }

                idxNo = 0;
                foreach (Role idxRole in ActiveType<Role>.Select())
                {
                    init["ModuleSettings"]["ExistingRoles"]["Role" + idxNo].Value = idxRole.Name;
                    idxNo += 1;
                }

                ActiveEvents.Instance.RaiseLoadControl(
                    "UsersModules.EditUser",
                    "dynMid",
                    init);
            }
        }

        [ActiveEvent(Name = "CreateNewUser")]
        protected void CreateNewUser(object sender, ActiveEventArgs e)
        {
            // Making sure our Viewport is made in-visible
            ActiveEvents.Instance.RaiseClearControls("dynPopup");

            // Checking to see if username already is taken...
            string username = e.Params["Username"].Get<string>();
            User oldExisting = ActiveType<User>.SelectFirst(Criteria.Eq("Username", username));
            if (oldExisting != null)
            {
                // A user with that username already exists
                Node nodeDoesntExist = new Node();
                nodeDoesntExist["Message"].Value =
                    Language.Instance["UserAlreadyTakenInfo", null, @"Sorry, but that username is already taken..."];
                nodeDoesntExist["Duration"].Value = 5000;
                ActiveEvents.Instance.RaiseActiveEvent(
                    this,
                    "ShowInformationMessage",
                    nodeDoesntExist);
            }
            else
            {
                // No existing user with that username, and hence we can create a new one...
                User user = new User {Username = username};
                user.Save();
                Node userInit = new Node();
                userInit["UserID"].Value = user.ID.ToString();
                ActiveEvents.Instance.RaiseActiveEvent(
                    this,
                    "EditUser",
                    userInit);
            }
        }

        [ActiveEvent(Name = "DeleteUser")]
        protected void DeleteUser(object sender, ActiveEventArgs e)
        {
            int id = int.Parse(e.Params["UserID"].Get<string>());
            User user = ActiveType<User>.SelectByID(id);
            user.Delete();
        }

        [ActiveEvent(Name = "AddRoleToUser")]
        protected void AddRoleToUser(object sender, ActiveEventArgs e)
        {
            string username = e.Params["Username"].Get<string>();
            string roleNameToAdd = e.Params["RoleToAdd"].Get<string>();

            User user = ActiveType<User>.SelectFirst(Criteria.Eq("Username", username));
            if (user.InRole(roleNameToAdd))
            {
                Node information = new Node();
                information["Message"].Value = Language.Instance["UserAlreadyInRoleInfo", null, @"User already belongs to that Role"];
                information["Duration"].Value = 5000;
                ActiveEvents.Instance.RaiseActiveEvent(
                    this,
                    "ShowInformationMessage",
                    information);
            }
            else
            {
                user.Roles.Add(ActiveType<Role>.SelectFirst(Criteria.Eq("Name", roleNameToAdd)));
                user.Save();
            }

            // Sending Roles for user back again
            int idxNo = 0;
            foreach (Role idxRole in user.Roles)
            {
                e.Params["Roles"]["Role" + idxNo].Value = idxRole.Name;
                idxNo += 1;
            }
        }

        [ActiveEvent(Name = "DeleteRoleFromUser")]
        protected void DeleteRole(object sender, ActiveEventArgs e)
        {
            string roleName = e.Params["RoleToDelete"].Value.ToString();
            string username = e.Params["Username"].Value.ToString();

            User user = ActiveType<User>.SelectFirst(Criteria.Eq("Username", username));

            if (roleName == "Administrator" && username == Settings.Instance["PowerUser"])
            {
                Node information = new Node();
                information["Message"].Value = Language.Instance["UserNotRemoveAdminInfo", null, @"You cannot remove the administrator role 
from the registered power user"];
                information["Duration"].Value = 5000;
                ActiveEvents.Instance.RaiseActiveEvent(
                    this,
                    "ShowInformationMessage",
                    information);
                int idxNo = 0;
                foreach (Role idxRole in user.Roles)
                {
                    e.Params["Roles"]["Role" + idxNo].Value = idxRole.Name;
                    idxNo += 1;
                }
            }
            else
            {
                Role toDelete = null;
                foreach (Role idx in user.Roles)
                {
                    if (idx.Name == roleName)
                    {
                        toDelete = idx;
                        break;
                    }
                }
                user.Roles.Remove(toDelete);
                user.Save();

                int idxNo = 0;
                foreach (Role idxRole in user.Roles)
                {
                    e.Params["Roles"]["Role" + idxNo].Value = idxRole.Name;
                    idxNo += 1;
                }

                // Showing information
                Node information = new Node();
                information["Message"].Value = Language.Instance["UserDeleteRoleNoticeInfo", null, @"Please notice that when user logs on the next time, 
any roles might be overridden by the Active Directory groups if user is an Active Directory user..."];
                information["Duration"].Value = 5000;
                ActiveEvents.Instance.RaiseActiveEvent(
                    this,
                    "ShowInformationMessage",
                    information);
            }
        }

        [ActiveEvent(Name = "UpdateUser")]
        protected void UpdateUser(object sender, ActiveEventArgs e)
        {
            User user = ActiveType<User>.SelectFirst(Criteria.Eq("Username", e.Params["Username"].Get<string>()));
            user.Email = e.Params["Email"].Get<string>();
            user.Phone = e.Params["Phone"].Get<string>();
            user.Save();
        }

        [ActiveEvent(Name = "Menu-CreateNewUser")]
        protected void MenuClicked_CreateNewUser(object sender, ActiveEventArgs e)
        {
            Node init = new Node();
            init["TabCaption"].Value = Language.Instance["UserCreateNewCaption", null, "Create New User"];
            init["Width"].Value = 250;
            init["Height"].Value = 130;
            ActiveEvents.Instance.RaiseLoadControl(
                "UsersModules.CreateNewUser",
                "dynPopup",
                init);
        }

        [ActiveEvent(Name = "Menu-ViewAllUsers")]
        protected void ViewAllUsers(object sender, ActiveEventArgs e)
        {
            Node init = new Node();
            init["TabCaption"].Value = Language.Instance["UserViewAllCaption", null, "All Users"];
            init["ModuleSettings"]["Grid"]["Columns"]["Username"]["Caption"].Value = Language.Instance["Username"];
            init["ModuleSettings"]["Grid"]["Columns"]["Username"]["ControlType"].Value = "LinkButton";
            init["ModuleSettings"]["Grid"]["Columns"]["LastLoggedIn"]["Caption"].Value = Language.Instance["LastLoggedIn", null, "Last Activity"];
            init["ModuleSettings"]["Grid"]["Columns"]["LastLoggedIn"]["ControlType"].Value = "Label";
            init["ModuleSettings"]["Grid"]["Columns"]["RolesString"]["Caption"].Value = Language.Instance["RolesString", null, "Belongs to"];
            init["ModuleSettings"]["Grid"]["Columns"]["RolesString"]["ControlType"].Value = "Label";
            int idxNo = 0;
            foreach (User idx in ActiveType<User>.Select())
            {
                init["ModuleSettings"]["Grid"]["Rows"]["Row" + idxNo]["ID"].Value = idx.ID;
                init["ModuleSettings"]["Grid"]["Rows"]["Row" + idxNo]["Username"].Value = idx.Username;
                init["ModuleSettings"]["Grid"]["Rows"]["Row" + idxNo]["LastLoggedIn"].Value = idx.LastLoggedIn.ToString("dddd d MMM yyyy HH:mm");
                init["ModuleSettings"]["Grid"]["Rows"]["Row" + idxNo]["RolesString"].Value = idx.GetRolesString();
                idxNo += 1;
            }

            ActiveEvents.Instance.RaiseLoadControl(
                "UsersModules.Users",
                "dynMid",
                init);
        }
    }
}