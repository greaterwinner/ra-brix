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
using Ra.Brix.Loader;
using ASP = System.Web.UI.WebControls;
using Ra.Brix.Types;
using Ra.Extensions.Widgets;
using System.Collections.Generic;
using Doxygen.NET;
using System.IO;
using Ra.Widgets;
using Ra.Selector;
using Ra.Effects;
using ColorizerLibrary;

namespace DoxygentDotNetViewDocsModules
{
    [ActiveModule]
    public class ViewClass : System.Web.UI.UserControl, IModule
    {
        protected global::System.Web.UI.HtmlControls.HtmlGenericControl header;
        protected global::System.Web.UI.HtmlControls.HtmlGenericControl info;
        protected global::System.Web.UI.WebControls.Repeater repProperties;
        protected global::Ra.Widgets.Dynamic sampleDyn;
        protected global::Ra.Extensions.Widgets.TabControl tab;
        protected global::Ra.Widgets.Label code;
        protected global::Ra.Widgets.Label aspx;

        private Docs RaDocs
        {
            get
            {
                if (Application["RaDocs"] == null)
                    Application["RaDocs"] = new Docs(Server.MapPath("~/docs-xml"));
                return (Docs)Application["RaDocs"];
            }
        }

        public void InitialLoading(Node node)
        {
            Class cls = node["Class"].Get<Class>();
            Load +=
                delegate
                {
                    SetDescriptionAndHeader(cls);
                    SetMembers(cls);
                    LoadCodeFile(cls);
                };
            sampleDyn.Load +=
                delegate
                {
                    LoadSample(cls);
                };
        }

        private void LoadCodeFile(Class cls)
        {
            Node node = new Node();
            node["ClassName"].Value = cls.Name;
            ActiveEvents.Instance.RaiseActiveEvent(
                this,
                "DoxygenDotNetGetClassCodeFiles",
                node);
            if (node["Markup"].Value == null)
                aspx.Text = "";
            else
            {
                aspx.Text = node["Markup"].Get<string>();
            }
            if (node["Code"].Value == null)
                code.Text = "";
            else
            {
                code.Text = node["Code"].Get<string>();
            }
        }

        private void LoadSample(Class cls)
        {
            Node node = new Node();
            node["ClassOriginalName"].Value = cls.Name;
            ActiveEvents.Instance.RaiseActiveEvent(
                this,
                "DoxygenDotNetGetClassNameForSample",
                node);
            if (node["ClassName"].Value != null)
            {
                tab.Visible = true;
                sampleDyn.LoadControls(node["ClassName"].Get<string>());
            }
            else
            {
                tab.Visible = false;
            }
        }

        protected void sampleDyn_Reload(object sender, Dynamic.ReloadEventArgs e)
        {
            System.Web.UI.Control ctrl = PluginLoader.Instance.LoadControl(e.Key);
            ctrl.ID = "smpl" + e.Key.GetHashCode().ToString().Replace("-", "");
            if (e.FirstReload)
            {
                ctrl.Init +=
                    delegate
                    {
                        IModule module = ctrl as IModule;
                        if (module != null)
                        {
                            module.InitialLoading(new Node());
                        }
                    };
            }
            sampleDyn.Controls.Add(ctrl);
        }

        private void SetMembers(Class cls)
        {
            // Class members
            List<string> showOnly =
                new List<string>(new string[] { "function", "ctor", "property", "event" });
            List<Member> membersToShow = cls.Members.FindAll(delegate(Member m)
            {
                return showOnly.Contains(m.Kind) &&
                    m.AccessModifier == "public" &&
                    !string.IsNullOrEmpty(m.Description);
            });
            List<DocsItem> tmp = new List<DocsItem>();
            foreach (Member idx in membersToShow)
            {
                string signature = string.Empty;
                if (idx is Method)
                    signature = (idx as Method).Signature;
                else if (idx is Property)
                    signature = (idx as Property).Signature;
                DocsItem item = new DocsItem(idx.Name, idx.FullName, idx.Kind, signature);
                item.Kind = "xx_" + idx.Kind;
                tmp.Add(item);
            }
            tmp.Sort(
                delegate(DocsItem left, DocsItem right)
                {
                    if (right.Kind == left.Kind)
                        return left.Name.CompareTo(right.Name);
                    return right.Kind.CompareTo(left.Kind);
                });
            repProperties.DataSource = tmp;
            repProperties.DataBind();
        }

        private void SetDescriptionAndHeader(Class cls)
        {
            // Class name, inheritance and description
            header.InnerHtml = cls.FullName;
            if (cls.BaseTypes.Count > 0 && !string.IsNullOrEmpty(cls.BaseTypes[0]))
            {
                header.InnerHtml += " : " + RaDocs.GetTypeByID(cls.BaseTypes[0]).FullName;
            }
            info.InnerHtml = cls.Description;
        }

        protected void PropertyChosen(object sender, EventArgs e)
        {
            LinkButton btn = sender as LinkButton;
            Panel pnl = Selector.SelectFirst<Panel>(btn.Parent);
            Label lbl = Selector.SelectFirst<Label>(pnl);
            Label paramsLabel = Selector.FindControl<Label>(btn, "paramsLabel");
            Label returnTypeLabel = Selector.FindControl<Label>(btn, "returnTypeLabel");

            if (!pnl.Visible || pnl.Style["display"] == "none")
            {
                if (lbl.Text == "")
                {
                    Class c = RaDocs.GetTypeByName(btn.Xtra.Remove(btn.Xtra.LastIndexOf("."))) as Class;
                    Member selectedMemebr = c.Members.Find(delegate(Member m)
                    {
                        return m.FullName == btn.Xtra;
                    });

                    if (selectedMemebr != null)
                    {
                        lbl.Text = selectedMemebr.Description;
                    }
                }
                pnl.Visible = true;
                paramsLabel.Visible = true;
                returnTypeLabel.Visible = true;
                pnl.Style["display"] = "none";
                new EffectRollDown(pnl, 500)
                    .JoinThese(new EffectFadeIn())
                    .Render();
            }
            else
            {
                paramsLabel.Visible = false;
                returnTypeLabel.Visible = false;
                new EffectRollUp(pnl, 500)
                    .JoinThese(new EffectFadeOut())
                    .Render();
            }
        }
    }
}