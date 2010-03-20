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
using LanguageRecords;
using Ra.Widgets;
using Ra.Brix.Loader;
using ASP = System.Web.UI.WebControls;
using Ra.Brix.Types;
using Ra.Extensions.Widgets;

namespace UsersModules
{
    [ActiveModule]
    public class EditUser : System.Web.UI.UserControl, IModule
    {
        protected global::Ra.Widgets.Image imgGravatar;
        protected global::Ra.Widgets.Label lblUsername;
        protected global::Ra.Widgets.Label lblLastLoggedIn;
        protected global::Ra.Widgets.Panel repWrp;
        protected global::Ra.Widgets.Panel roleEditing;
        protected global::Ra.Extensions.Widgets.InPlaceEdit email;
        protected global::Ra.Extensions.Widgets.InPlaceEdit phone;
        protected global::System.Web.UI.WebControls.Repeater rep;
        protected global::System.Web.UI.HtmlControls.HtmlGenericControl userData;
        protected global::Ra.Widgets.SelectList selectRole;
        protected global::Ra.Extensions.Widgets.RichEdit editor;

        public void InitialLoading(Node node)
        {
            imgGravatar.DataBind();
            userData.DataBind();
            selectRole.Items.Add(new ListItem(Language.Instance["EditUserSelectRoleListItem0", null, "Select role..."],""));
            lblUsername.Text = node["Username"].Value.ToString();
            lblLastLoggedIn.Text = Language.Instance["EditUserLastLoggedInLabel", null, "Last logged in; "] + node["LastLoggedIn"].Get<DateTime>().ToString("dddd d MMMM yyyy HH:mm");
            email.Text = node["Email"].Get<string>();
            GetGravatar();
            phone.Text = node["Phone"].Get<string>();
            roleEditing.Visible = node["AllowRoleEditing"].Get<bool>();
            if (node["Biography"].Value == null)
            {
                editor.Text = Language.Instance["TypeInYourProfileHere", null, "<p>Type in your biography here...</p>"];
            }
            else
            {
                editor.Text = node["Biography"].Get<string>();
            }

            // Bindig roles
            Session["UsersModules.EditUser.Roles"] = node["Roles"];

            // Binding Roles SelectList...
            foreach (Node idx in node["ExistingRoles"])
            {
                ListItem i = new ListItem(idx.Get<string>(), idx.Get<string>());
                selectRole.Items.Add(i);
            }
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

        protected void submit_Click(object sender, EventArgs e)
        {
            SaveBiography();
        }

        private void SaveBiography()
        {
            Node node = new Node();
            node["Biography"].Value = editor.Text;
            node["Username"].Value = lblUsername.Text;
            ActiveEvents.Instance.RaiseActiveEvent(
                this,
                "SaveUserBiography",
                node);
        }

        protected void editor_CtrlKeys(object sender, RichEdit.CtrlKeysEventArgs e)
        {
            if (e.Key == 's')
            {
                SaveBiography();
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            BindRepeater();
        }

        protected void selectRole_SelectedIndexChanged(object sender, EventArgs e)
        {
            string roleName = selectRole.SelectedItem.Value;

            Node node = new Node();
            node["RoleToAdd"].Value = roleName;
            node["Username"].Value = lblUsername.Text;
            ActiveEvents.Instance.RaiseActiveEvent(
                this,
                "AddRoleToUser",
                node);

            // Rebinding Repeater
            Session["UsersModules.EditUser.Roles"] = node["Roles"];
            BindRepeater();
            repWrp.ReRender();
            selectRole.SelectedIndex = 0;
        }

        private void BindRepeater()
        {
            rep.DataSource = Session["UsersModules.EditUser.Roles"];
            rep.DataBind();
        }

        protected void RoleDeleted(object sender, EventArgs e)
        {
            LinkButton btn = sender as LinkButton;
            if (btn == null)
            {
                return;
            }
            string roleName = btn.Xtra;

            Node node = new Node();
            node["RoleToDelete"].Value = roleName;
            node["Username"].Value = lblUsername.Text;
            ActiveEvents.Instance.RaiseActiveEvent(
                this,
                "DeleteRoleFromUser", 
                node);

            Session["UsersModules.EditUser.Roles"] = node["Roles"];
            BindRepeater();
            repWrp.ReRender();
        }

        private void GetGravatar()
        {
            imgGravatar.ImageUrl = string.Format(
                "http://www.gravatar.com/avatar/{0}?s=64&d=identicon",
                Helpers.MD5Hash(email.Text));
        }

        protected void email_TextChanged(object sender, EventArgs e)
        {
            GetGravatar();
            UpdateUser();
        }

        protected void phone_TextChanged(object sender, EventArgs e)
        {
            UpdateUser();
        }

        private void UpdateUser()
        {
            Node node = new Node();
            node["Username"].Value = lblUsername.Text;
            node["Email"].Value = email.Text;
            node["Phone"].Value = phone.Text;
            ActiveEvents.Instance.RaiseActiveEvent(
                this,
                "UpdateUser",
                node);
        }
    }
}