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
using System.Web.UI;
using LanguageRecords;
using Ra.Brix.Loader;
using Ra.Brix.Types;
using System.IO;
using Ra.Extensions.Widgets;
using Components;
using Ra.Effects;
using Ra.Widgets;
using System.Globalization;

namespace ResourcesModules
{
    [ActiveModule]
    public class Explorer : System.Web.UI.UserControl, IModule
    {
        protected global::Ra.Extensions.Widgets.Tree tree;
        protected global::Ra.Extensions.Widgets.TreeNodes root;
        protected global::Ra.Widgets.Panel grdWrapper;
        protected global::Components.Grid grd;
        protected global::System.Web.UI.WebControls.Button btnSaveFile;
        protected global::System.Web.UI.WebControls.FileUpload fileUpload;

        protected void Page_Load(object sender, EventArgs e)
        {
            btnSaveFile.DataBind();
            BuildTree();
        }

        private void BuildTree()
        {
            string baseDir = Server.MapPath("~/Resources/");
            BuildFromDirectory(baseDir, root);
        }

        private static void BuildFromDirectory(string baseDir, Control level)
        {
            foreach (string idxDir in Directory.GetDirectories(baseDir))
            {
                string dir = idxDir.Replace(baseDir, "");

                // Skipping .svn and similar directories...
                if (dir.IndexOf(".") == 0)
                {
                    continue;
                }
                TreeNode n = new TreeNode {ID = level.ID + dir, Xtra = idxDir, Text = dir};
                level.Controls.Add(n);
                string[] childDirectories = Directory.GetDirectories(idxDir);
                if (childDirectories.Length > 0)
                {
                    // Skipping .svn and similar directories...
                    if (childDirectories.Length == 1)
                    {
                        string tmpDir = childDirectories[0];
                        tmpDir = tmpDir.Substring(tmpDir.LastIndexOf("\\") + 1);
                        if (tmpDir.IndexOf(".") == 0)
                        {
                            continue;
                        }
                    }
                    TreeNodes children = new TreeNodes {ID = "ch" + n.ID};
                    n.Controls.Add(children);
                    BuildFromDirectory(idxDir + "\\", children);
                }
            }
        }

        protected void tree_SelectedNodeChanged(object sender, EventArgs e)
        {
            if (tree.SelectedNodes.Length == 0)
            {
                return;
            }
            BindFilesGrid();
        }

        private string CurrentDirectory
        {
            get { return ViewState["CurrentDirectory"] as string; }
            set { ViewState["CurrentDirectory"] = value; }
        }

        private void BindFilesGrid()
        {
            string directory = tree.SelectedNodes[0].Xtra;
            CurrentDirectory = directory;
            grdWrapper.Visible = true;
            grdWrapper.Style[Styles.display] = "none";
            new EffectFadeIn(grdWrapper, 200).Render();

            Node data = new Node();
            data["Grid"]["Columns"]["FileName"]["Caption"].Value =
                Language.Instance["FileName", null, "File name"];
            data["Grid"]["Columns"]["FileName"]["ControlType"].Value = SelectMode ?
                "LinkButton" :
                "Link";
            data["Grid"]["Columns"]["FileDate"]["Caption"].Value =
                Language.Instance["Date", null, "Date"];
            data["Grid"]["Columns"]["FileDate"]["ControlType"].Value = "Label";
            int idxNo = 0;
            foreach (string idxFile in Directory.GetFiles(directory))
            {
                string baseDir = Server.MapPath("~/");
                string fileName = idxFile.Substring(idxFile.LastIndexOf("\\") + 1);
                string fileHref = idxFile.Replace(baseDir, "");
                string fileDate = File.GetCreationTime(idxFile).ToString("yyyy.MM.dd HH:mm", CultureInfo.InvariantCulture);
                data["Grid"]["Rows"]["Row" + idxNo]["ID"].Value = fileHref;
                data["Grid"]["Rows"]["Row" + idxNo]["FileName"].Value = fileName;
                data["Grid"]["Rows"]["Row" + idxNo]["FileDate"].Value = fileDate;

                fileHref = fileHref.Replace("\\", "/");
                data["Grid"]["Rows"]["Row" + idxNo]["FileName"]["href"].Value = fileHref;
                idxNo += 1;
            }
            grd.SortColumn = "FileDate";
            grd.DataSource = data["Grid"];
            grd.Rebind();
        }

        protected void grid_RowDeleted(object sender, Grid.GridActionEventArgs e)
        {
            string fileName = e.ID;
            File.Delete(fileName);
        }

        protected void grd_Action(object sender, Grid.GridActionEventArgs e)
        {
            string fileName = e.ID;
            Node node = new Node();
            node["File"].Value = fileName;
            ActiveEvents.Instance.RaiseActiveEvent(
                this,
                "FileExplorerFileChosen",
                node);
        }

        protected void btnSaveFile_Click(object sender, EventArgs e)
        {
            fileUpload.SaveAs(CurrentDirectory + "/" + fileUpload.FileName);
            BindFilesGrid();
        }

        private bool SelectMode
        {
            get { return ViewState["SelectMode"] == null ? false : (bool)ViewState["SelectMode"]; }
            set { ViewState["SelectMode"] = value; }
        }

        public void InitialLoading(Node node)
        {
            Load +=
                delegate
                {
                    SelectMode = node["Mode"].Value != null ? true : false;
                };
        }

        public string GetCaption()
        {
            return "";
        }
    }
}