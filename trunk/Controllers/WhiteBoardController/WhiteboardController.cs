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
using Ra.Brix.Data;
using Ra.Brix.Loader;
using SettingsRecords;
using WhiteBoardRecords;
using Ra.Brix.Types;
using System.Collections.Generic;

namespace WhiteBoardController
{
    [ActiveController]
    public class WhiteboardController
    {
        [ActiveEvent(Name = "ApplicationStartup")]
        protected static void ApplicationStartup(object sender, ActiveEventArgs e)
        {
            Language.Instance.SetDefaultValue("ButtonLists", "Lists");
            Language.Instance.SetDefaultValue("ButtonCreateNewList", "New List");
            Language.Instance.SetDefaultValue("ButtonViewAllLists", "View All");
            Language.Instance.SetDefaultValue("ButtonEditList", "Configure");
            Language.Instance.SetDefaultValue("ButtonViewList", "View");
        }

        [ActiveEvent(Name = "GetMenuItems")]
        protected void GetMenuItems(object sender, ActiveEventArgs e)
        {
            e.Params["ButtonAppl"]["ButtonWhiteboards"].Value = "Menu-Whiteboards";
            e.Params["ButtonAppl"]["ButtonWhiteboards"]["ButtonCreateWhiteboard"].Value = "Menu-CreateWhiteboard";

            List<Whiteboard> whiteboards = new List<Whiteboard>(ActiveType<Whiteboard>.Select());
            if (whiteboards.Count > 0)
            {
                e.Params["ButtonAppl"]["ButtonWhiteboards"]["ButtonViewAllWhiteboards"].Value =
                    "Menu-ViewAllWhiteboards";
                e.Params["ButtonAppl"]["ButtonWhiteboards"]["ButtonEditWhiteboard"].Value = "Menu-EditWhiteboard";
                e.Params["ButtonAppl"]["ButtonWhiteboards"]["ButtonViewWhiteboard"].Value = "Menu-ViewWhiteboard";
                foreach (Whiteboard idx in whiteboards)
                {
                    // First the edit parts...
                    e.Params["ButtonAppl"]["ButtonWhiteboards"]["ButtonEditWhiteboard"]
                        [Language.Instance["Edit", null, "Edit"] + " " + idx.Name].Value = "Menu-EditSpecificWhiteboard";
                    e.Params["ButtonAppl"]["ButtonWhiteboards"]["ButtonEditWhiteboard"]
                        [Language.Instance["Edit", null, "Edit"] + " " + idx.Name]["Params"].Value = idx.ID.ToString();

                    // Then the "view" parts
                    e.Params["ButtonAppl"]["ButtonWhiteboards"]["ButtonViewWhiteboard"]
                        [Language.Instance["View", null, "View"] + " " + idx.Name].Value = "Menu-ViewSpecificWhiteboard";
                    e.Params["ButtonAppl"]["ButtonWhiteboards"]["ButtonViewWhiteboard"]
                        [Language.Instance["View", null, "View"] + " " + idx.Name]["Params"].Value = idx.ID.ToString();
                }
            }
        }

        [ActiveEvent(Name = "Menu-ViewSpecificWhiteboard")]
        protected void ViewSpecificWhiteboard(object sender, ActiveEventArgs e)
        {
            int whiteboardId = int.Parse(e.Params["Params"].Get<string>());
            ViewWhiteboard(whiteboardId);
        }

        [ActiveEvent(Name = "Menu-ViewAllWhiteboards")]
        protected void ViewAllWhiteboards(object sender, ActiveEventArgs e)
        {
            Node init = new Node();
            init["TabCaption"].Value = Language.Instance["WhiteboardAllCaption", null, "All Whiteboards"];
            init["ModuleSettings"]["Grid"]["Columns"]["View"]["Caption"].Value =
                Language.Instance["ViewList", null, "View List"];
            init["ModuleSettings"]["Grid"]["Columns"]["View"]["ControlType"].Value = "LinkButton";
            init["ModuleSettings"]["Grid"]["Columns"]["Edit"]["Caption"].Value =
                Language.Instance["EditList", null, "Edit List"];
            init["ModuleSettings"]["Grid"]["Columns"]["Edit"]["ControlType"].Value = "LinkButton";

            int idxNo = 0;
            foreach (Whiteboard idx in ActiveType<Whiteboard>.Select())
            {
                init["ModuleSettings"]["Grid"]["Rows"]["Row" + idxNo]["ID"].Value = idx.ID;
                init["ModuleSettings"]["Grid"]["Rows"]["Row" + idxNo]["View"].Value = Language.Instance[idx.Name];
                init["ModuleSettings"]["Grid"]["Rows"]["Row" + idxNo]["Edit"].Value = Language.Instance[idx.Name];
                idxNo += 1;
            }
            ActiveEvents.Instance.RaiseLoadControl(
                "WhiteboardModules.ViewAllWhiteboards",
                "dynMid",
                init);
        }

        [ActiveEvent(Name = "WhiteboardSelectedForViewing")]
        protected void WhiteboardSelectedForViewing(object sender, ActiveEventArgs e)
        {
            int whiteboardId = int.Parse(e.Params["ID"].Get<string>());
            ViewWhiteboard(whiteboardId);
        }

        [ActiveEvent(Name = "WhiteboardSelectedForEditing")]
        protected void WhiteboardSelectedForEditing(object sender, ActiveEventArgs e)
        {
            int whiteboardId = int.Parse(e.Params["ID"].Get<string>());
            EditWhiteboard(whiteboardId);
        }

        [ActiveEvent(Name = "Menu-CreateWhiteboard")]
        protected void CreateWhiteboard(object sender, ActiveEventArgs e)
        {
            Node init = new Node();
            init["TabCaption"].Value = Language.Instance["ListCaption", null, "Create new List"];
            init["Width"].Value = 350;
            init["Height"].Value = 150;
            ActiveEvents.Instance.RaiseLoadControl(
                "WhiteboardModules.CreateNewWhiteboard",
                "dynPopup",
                init);
        }

        [ActiveEvent(Name = "Menu-EditSpecificWhiteboard")]
        protected void OpenSpecificWhiteboard(object sender, ActiveEventArgs e)
        {
            EditWhiteboard(int.Parse(e.Params["Params"].Get<string>()));
        }

        [ActiveEvent(Name = "WhiteboardRowValueEdited")]
        protected void WhiteboardRowValueEdited(object sender, ActiveEventArgs e)
        {
            int whiteboardId = int.Parse(e.Params["WhiteboardID"].Get<string>());
            int rowId = int.Parse(e.Params["RowID"].Get<string>());
            string columnName = e.Params["ColumnName"].Get<string>();
            string value = e.Params["Value"].Get<string>();

            Whiteboard w = ActiveType<Whiteboard>.SelectByID(whiteboardId);
            Whiteboard.Row row = w.Rows.Find(
                delegate(Whiteboard.Row idx)
                {
                    return idx.ID == rowId;
                });
            Whiteboard.Cell cell = row.Cells.Find(
                delegate(Whiteboard.Cell idx)
                {
                    return idx.Column.Caption == columnName;
                });
            cell.Value = value;
            w.Save();
        }

        [ActiveEvent(Name = "GetWhiteboardColumnTypes")]
        protected void GetWhiteboardColumnTypes(object sender, ActiveEventArgs e)
        {
            e.Params["Types"]["Label"].Value = true;
            e.Params["Types"]["InPlaceEdit"].Value = true;
            e.Params["Types"]["TextAreaEdit"].Value = true;
        }

        [ActiveEvent("GetHelpContents")]
        protected static void GetHelpContents(object sender, ActiveEventArgs e)
        {
            e.Params[Language.Instance["WhiteboardCreateHelpLabel", null, "About the Create New Whiteboard"]].Value = "Help-AboutTheCreateNewWhiteboard";
        }

        [ActiveEvent("Help-AboutTheCreateNewWhiteboard")]
        protected static void Help_AboutTheCreateNewWhiteboard(object sender, ActiveEventArgs e)
        {
            Node colTypes = new Node();
            ActiveEvents.Instance.RaiseActiveEvent(
                null,
                "GetWhiteboardColumnTypes",
                colTypes);
            string list = "<ul class=\"list\">";
            foreach (Node idx in colTypes["Types"])
            {
                list += "<li>" + idx.Name + "</li>";
            }
            list += "</ul>";
            const string aboutCreateNewWhiteboard = @"
In the Create New Whiteboard menu command you can create a new whiteboard which you can later configure in
your whiteboard configuration manager.

Allowed types for columns may vary according to your installation and whiteboard-plugins, 
but currently installed plugins allows;

{0}
";
            e.Params["Text"].Value =
                string.Format(Language.Instance["WhiteboardCreateHelp", null, aboutCreateNewWhiteboard],
                    list);
        }

        [ActiveEvent(Name = "WhiteboardColumnEdited")]
        protected void WhiteboardColumnEdited(object sender, ActiveEventArgs e)
        {
            int whiteBoardId = int.Parse(e.Params["WhiteboardID"].Get<string>());
            int id = int.Parse(e.Params["ID"].Get<string>());
            string colName = e.Params["Column"].Get<string>();
            string value = e.Params["Value"].Get<string>();
            Whiteboard w = ActiveType<Whiteboard>.SelectByID(whiteBoardId);
            Whiteboard.Column col = w.Columns.Find(
                delegate(Whiteboard.Column idx)
                {
                    return idx.ID == id;
                });
            switch (colName)
            {
                case "Name":
                    {
                        col.Caption = value;
                    } break;
                case "Position":
                    {
                        col.Position = int.Parse(value);
                    } break;
                case "ShowInSummary":
                    {
                        col.ShowInSummary = value.ToLowerInvariant() == "true";
                    } break;
                case "Type":
                    {
                        Node colTypes = new Node();
                        ActiveEvents.Instance.RaiseActiveEvent(
                            this,
                            "GetWhiteboardColumnTypes",
                            colTypes);
                        bool legal = false;
                        foreach (Node idx in colTypes["Types"])
                        {
                            if (value.IndexOf(idx.Name) == 0)
                            {
                                legal = true;
                                break;
                            }
                        }
                        if (legal)
                        {
                            col.Type = value;
                        }
                        else
                        {
                            Node info = new Node();
                            info["Message"].Value = Language.Instance["WhiteboardInvalidColumnInfo", null, @"That type of Column is NOT supported, nothing was saved...<br/>"];
                            info["Message"].Value += Language.Instance["WhiteboardLegalTypesInfo", null, "Installed (end legal) types are; "];
                            info["Message"].Value += "<ul class=\"list\">";
                            foreach (Node idx in colTypes["Types"])
                            {
                                info["Message"].Value += "<li>" + idx.Name + "</li>";
                            }
                            info["Message"].Value += "</ul>";
                            info["Duration"].Value = 2000;
                            ActiveEvents.Instance.RaiseActiveEvent(
                                this,
                                "ShowInformationMessage",
                                info);
                        }
                    } break;
            }
            w.Save();
        }

        [ActiveEvent(Name = "UpdateWhiteboard")]
        protected void UpdateWhiteboard(object sender, ActiveEventArgs e)
        {
            int id = int.Parse(e.Params["WhiteboardID"].Get<string>());
            Whiteboard w = ActiveType<Whiteboard>.SelectByID(id);
            if (e.Params["EnableFiltering", false] != null)
                w.EnableFiltering = e.Params["EnableFiltering"].Get<bool>();
            if (e.Params["EnableHeaders", false] != null)
                w.EnableHeaders = e.Params["EnableHeaders"].Get<bool>();
            if (e.Params["EnableDeletion", false] != null)
                w.EnableDeletion = e.Params["EnableDeletion"].Get<bool>();
            if (e.Params["PageSize", false] != null)
            {
                int nValue;
                if (int.TryParse(e.Params["PageSize"].Get<string>(), out nValue))
                {
                    w.PageSize = nValue;
                }
                else
                {
                    Node info = new Node();
                    info["Message"].Value = Language.Instance["WhiteboardInfoFilterErrorMessage", null, @"
You must type in an integer value, nothing was saved..."];
                    info["Duration"].Value = 5000;
                    ActiveEvents.Instance.RaiseActiveEvent(
                        this,
                        "ShowInformationMessage",
                        info);
                    return;
                }
            }
            w.Save();
        }

        [ActiveEvent(Name = "DeleteWhiteboard")]
        protected void DeleteWhiteboard(object sender, ActiveEventArgs e)
        {
            int id = int.Parse(e.Params["ID"].Get<string>());
            Whiteboard w = ActiveType<Whiteboard>.SelectByID(id);
            w.Delete();

            RefreshMenu();

            ActiveEvents.Instance.RaiseActiveEvent(
                this,
                "WhiteboardWasDeleted");
        }

        private static void EditWhiteboard(int id)
        {
            Whiteboard board = ActiveType<Whiteboard>.SelectByID(id);

            Node init = new Node();
            init["TabCaption"].Value = Language.Instance["WhiteboardEditCaption", null, "Editing Whiteboard; "] + board.Name;
            init["ModuleSettings"]["Whiteboard"]["Name"].Value = board.Name;
            init["ModuleSettings"]["Whiteboard"]["ID"].Value = board.ID.ToString();
            init["ModuleSettings"]["Whiteboard"]["EnableFiltering"].Value = board.EnableFiltering;
            init["ModuleSettings"]["Whiteboard"]["PageSize"].Value = board.PageSize;
            init["ModuleSettings"]["Whiteboard"]["EnableHeaders"].Value = board.EnableHeaders;
            init["ModuleSettings"]["Whiteboard"]["EnableDeletion"].Value = board.EnableDeletion;

            int idxNo = 0;
            foreach (Whiteboard.Column idx in board.Columns)
            {
                init["ModuleSettings"]["Whiteboard"]["Columns"]["Columns" + idxNo]["ID"].Value = idx.ID.ToString();
                init["ModuleSettings"]["Whiteboard"]["Columns"]["Columns" + idxNo]["Caption"].Value = idx.Caption;
                init["ModuleSettings"]["Whiteboard"]["Columns"]["Columns" + idxNo]["Type"].Value = idx.Type;
                init["ModuleSettings"]["Whiteboard"]["Columns"]["Columns" + idxNo]["Position"].Value = idx.Position.ToString();
                init["ModuleSettings"]["Whiteboard"]["Columns"]["Columns" + idxNo]["ShowInSummary"].Value = idx.ShowInSummary;
                idxNo += 1;
            }

            ActiveEvents.Instance.RaiseLoadControl(
                "WhiteboardModules.EditWhiteboard",
                "dynMid",
                init);
        }

        private static void ViewWhiteboard(int id)
        {
            Whiteboard board = ActiveType<Whiteboard>.SelectByID(id);

            Node init = new Node();
            init["TabCaption"].Value = "Whiteboard; " + board.Name;
            init["ModuleSettings"]["Whiteboard"]["Name"].Value = board.Name;
            init["ModuleSettings"]["Whiteboard"]["ID"].Value = board.ID.ToString();
            init["ModuleSettings"]["Whiteboard"]["EnableFiltering"].Value = board.EnableFiltering;
            init["ModuleSettings"]["Whiteboard"]["PageSize"].Value = board.PageSize;
            init["ModuleSettings"]["Whiteboard"]["EnableHeaders"].Value = board.EnableHeaders;
            init["ModuleSettings"]["Whiteboard"]["EnableDeletion"].Value = board.EnableDeletion;

            // Sorting it according to the "Position" value...
            board.Columns.Sort(
                delegate(Whiteboard.Column left, Whiteboard.Column right)
                {
                    return left.Position.CompareTo(right.Position);
                });

            // Creating our Columns
            foreach (Whiteboard.Column idx in board.Columns)
            {
                if (idx.ShowInSummary)
                {
                    init["ModuleSettings"]["Whiteboard"]["Columns"][idx.Caption]["Caption"].Value = idx.Caption;
                    init["ModuleSettings"]["Whiteboard"]["Columns"][idx.Caption]["ControlType"].Value = idx.Type;
                }
            }

            int idxNo = 0;
            foreach (Whiteboard.Row idxRow in board.Rows)
            {
                init["ModuleSettings"]["Whiteboard"]["Rows"]["Row" + idxNo]["ID"].Value = idxRow.ID.ToString();
                idxRow.Cells.Sort(
                    delegate(Whiteboard.Cell left, Whiteboard.Cell right)
                    {
                        return left.Column.Position.CompareTo(right.Column.Position);
                    });
                foreach (Whiteboard.Cell idxCell in idxRow.Cells)
                {
                    if (idxCell.Column.ShowInSummary)
                    {
                        init["ModuleSettings"]["Whiteboard"]["Rows"]["Row" + idxNo][idxCell.Column.Caption].Value = idxCell.Value;
                    }
                }
                idxNo += 1;
            }

            ActiveEvents.Instance.RaiseLoadControl(
                "WhiteboardModules.ViewWhiteboard",
                (Settings.Instance.Get<bool>("FullScreenWhiteboards") ? "dynMaxi" : "dynMid"),
                init);
        }

        [ActiveEvent(Name = "DeleteWhiteboardRow")]
        protected void DeleteWhiteboardRow(object sender, ActiveEventArgs e)
        {
            int whiteboardId = int.Parse(e.Params["WhiteboardID"].Get<string>());
            int rowId = int.Parse(e.Params["RowID"].Get<string>());
            Whiteboard w = ActiveType<Whiteboard>.SelectByID(whiteboardId);

            w.Rows.RemoveAll(
                delegate(Whiteboard.Row idx)
                {
                    return idx.ID == rowId;
                });
            w.Save();
        }

        [ActiveEvent(Name = "AddRowToWhiteboard")]
        protected void AddRowToWhiteboard(object sender, ActiveEventArgs e)
        {
            int id = int.Parse(e.Params["WhiteboardID"].Get<string>());
            Whiteboard w = ActiveType<Whiteboard>.SelectByID(id);
            w.AddRow();

            // Creating our Columns
            foreach (Whiteboard.Column idx in w.Columns)
            {
                if (idx.ShowInSummary)
                {
                    e.Params["Whiteboard"]["Columns"][idx.Caption]["Caption"].Value = idx.Caption;
                    e.Params["Whiteboard"]["Columns"][idx.Caption]["ControlType"].Value = idx.Type;
                }
            }

            int idxNo = 0;
            foreach (Whiteboard.Row idxRow in w.Rows)
            {
                e.Params["Whiteboard"]["Rows"]["Row" + idxNo]["ID"].Value = idxRow.ID.ToString();
                idxRow.Cells.Sort(
                    delegate(Whiteboard.Cell left, Whiteboard.Cell right)
                    {
                        return left.Column.Position.CompareTo(right.Column.Position);
                    });
                foreach (Whiteboard.Cell idxCell in idxRow.Cells)
                {
                    if (idxCell.Column.ShowInSummary)
                    {
                        e.Params["Whiteboard"]["Rows"]["Row" + idxNo][idxCell.Column.Caption].Value = idxCell.Value;
                    }
                }
                idxNo += 1;
            }
        }

        [ActiveEvent(Name = "AddColumnToEditedWhiteboard")]
        protected void AddColumnToEditedWhiteboard(object sender, ActiveEventArgs e)
        {
            Node init = new Node();
            init["TabCaption"].Value = Language.Instance["WhiteboardAddColumnCaption", null, "Create New Column"];
            init["Width"].Value = 350;
            init["Height"].Value = 130;
            init["ModuleSettings"]["WhiteboardID"].Value = e.Params["WhiteboardID"].Value;
            ActiveEvents.Instance.RaiseLoadControl(
                "WhiteboardModules.CreateNewColumn",
                "dynPopup",
                init);
        }

        [ActiveEvent(Name = "CreateNewWhiteboardColumn")]
        protected void CreateNewWhiteboardColumn(object sender, ActiveEventArgs e)
        {
            string columnName = e.Params["ColumnName"].Get<string>();
            int id = int.Parse(e.Params["WhiteboardID"].Get<string>());

            Whiteboard board = ActiveType<Whiteboard>.SelectByID(id);
            Whiteboard.Column col = new Whiteboard.Column {Type = "Label", Caption = columnName};
            board.Columns.Add(col);
            board.Save();

            Node init = new Node();
            init["Whiteboard"]["Name"].Value = board.Name;
            init["Whiteboard"]["ID"].Value = board.ID.ToString();

            int idxNo = 0;
            foreach (Whiteboard.Column idx in board.Columns)
            {
                init["Whiteboard"]["Columns"]["Columns" + idxNo]["ID"].Value = idx.ID.ToString();
                init["Whiteboard"]["Columns"]["Columns" + idxNo]["Caption"].Value = idx.Caption;
                init["Whiteboard"]["Columns"]["Columns" + idxNo]["Type"].Value = idx.Type;
                init["Whiteboard"]["Columns"]["Columns" + idxNo]["Position"].Value = idx.Position.ToString();
                init["Whiteboard"]["Columns"]["Columns" + idxNo]["ShowInSummary"].Value = idx.ShowInSummary;
                idxNo += 1;
            }
            ActiveEvents.Instance.RaiseActiveEvent(
                this,
                "NewWhiteboardColumnAdded",
                init);
            ActiveEvents.Instance.RaiseClearControls("dynPopup");
        }

        [ActiveEvent(Name = "DeleteWhiteboardColumn")]
        protected void DeleteWhiteboardColumn(object sender, ActiveEventArgs e)
        {
            int whiteboardId = int.Parse(e.Params["WhiteboardID"].Get<string>());
            int columnId = int.Parse(e.Params["ColumnID"].Get<string>());

            // Getting new Whiteboard data...
            Whiteboard board = ActiveType<Whiteboard>.SelectByID(whiteboardId);
            board.Columns.RemoveAll(
                delegate(Whiteboard.Column idx)
                {
                    return idx.ID == columnId;
                });
            board.Save();
            int idxNo = 0;
            foreach (Whiteboard.Column idx in board.Columns)
            {
                e.Params["Whiteboard"]["Columns"]["Columns" + idxNo]["ID"].Value = idx.ID.ToString();
                e.Params["Whiteboard"]["Columns"]["Columns" + idxNo]["Caption"].Value = idx.Caption;
                e.Params["Whiteboard"]["Columns"]["Columns" + idxNo]["Type"].Value = idx.Type;
                e.Params["Whiteboard"]["Columns"]["Columns" + idxNo]["Position"].Value = idx.Position.ToString();
                idxNo += 1;
            }
        }

        [ActiveEvent(Name = "ChangeNameOfSpecificWhiteboard")]
        protected void ChangeNameOfSpecificWhiteboard(object sender, ActiveEventArgs e)
        {
            string name = e.Params["Name"].Get<string>();
            Whiteboard w = ActiveType<Whiteboard>.SelectByID(int.Parse(e.Params["ID"].Get<string>()));
            w.Name = name;
            w.Save();
        }

        [ActiveEvent(Name = "CreateNewWhiteboard")]
        protected void CreateNewWhiteboard(object sender, ActiveEventArgs e)
        {
            // Closing window...
            ActiveEvents.Instance.RaiseClearControls("dynPopup");

            string name = e.Params["WhiteboardName"].Get<string>();
            Whiteboard w = new Whiteboard {Name = name};
            w.Save();

            // Open whiteboard that was just created...
            EditWhiteboard(w.ID);

            RefreshMenu();

            ActiveEvents.Instance.RaiseActiveEvent(
                this,
                "WhiteboardWasCreated");
        }

        private void RefreshMenu()
        {
            // Refreshing menu...
            ActiveEvents.Instance.RaiseActiveEvent(
                this,
                "RefreshMenu");

            // Notifying user of that menu was refreshed...
            Node nodeMenuRefresh = new Node();
            nodeMenuRefresh["Message"].Value = Language.Instance["WhiteboardOutOfSyncInfo", null, @"The Menu was being refreshed since 
it was modified and hence out of sync in regards to its data..."];
            nodeMenuRefresh["Duration"].Value = 2000;
            ActiveEvents.Instance.RaiseActiveEvent(
                this,
                "ShowInformationMessage",
                nodeMenuRefresh);
        }
    }
}
