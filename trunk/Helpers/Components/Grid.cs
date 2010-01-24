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
using ASP = System.Web.UI;
using Ra.Widgets;
using WC = System.Web.UI.WebControls;
using Ra.Brix.Types;
using System.Collections.Generic;
using System.Web.UI.HtmlControls;
using Ra.Extensions.Widgets;
using System.ComponentModel;
using Ra.Behaviors;
using Ra.Effects;
using System.Globalization;
using Ra.Brix.Loader;

namespace Components
{
    /**
     * A DataGrid for displaying tabular data with complex logic.
     */
    [ToolboxData("<{0}:Grid runat=\"server\"></{0}:Grid>")]
    public class Grid : Panel
    {
        /**
         * EventArgs sent to CellEdited event.
         */
        public class GridEditEventArgs : EventArgs
        {
            private readonly string _id;
            private readonly string _key;
            private readonly object _newValue;

            internal GridEditEventArgs(string id, string key, object newValue)
            {
                _id = id;
                _key = key;
                _newValue = newValue;
                AcceptChange = true;
            }

            public string ID
            {
                get { return _id; }
            }

            public string Key
            {
                get { return _key; }
            }

            public object NewValue
            {
                get { return _newValue; }
            }

            public bool AcceptChange { get; set; }
        }

        /**
         * EventArgs sent to RowDeleted event.
         */
        public class GridActionEventArgs : EventArgs
        {
            private readonly string _id;
            private readonly string _columnName;

            internal GridActionEventArgs(string id, string columnName)
            {
                _id = id;
                _columnName = columnName;
            }

            public string ID
            {
                get { return _id; }
            }

            public string ColumnName
            { 
                get { return _columnName; }
            }
        }

        private Panel _lstWrappers;
        private Panel _ctrlWrappers;
        private Panel _filterWrapper;
        private TextBox _filter;
        private Button _fltBtn;
        private LinkButton _previous;
        private LinkButton _next;
        private Label _count;
        private bool _gridWasDatabinded;

        /**
         * Event sent after a Cell has been edited
         */
        public event EventHandler<GridEditEventArgs> CellEdited;

        /**
         * Event sent after a row has been deleted
         */
        public event EventHandler<GridActionEventArgs> RowDeleted;

        /**
         * Event sent after a link button o somethig similar has been clicked in a row
         */
        public event EventHandler<GridActionEventArgs> Action;

        /**
         * The DataSource for the Grid, needs to follow a schema that the grid expects.
         */
        public Node DataSource
        {
            get { return ViewState["DataSource"] as Node; }
            set
            {
                ViewState["DataSource"] = value;
                _gridWasDatabinded = true;
            }
        }

        private IEnumerable<Node> RowsFiltered
        {
            get
            {
                if (DataSource != null)
                {
                    foreach (Node idx in DataSource["Rows"])
                    {
                        if (!EnableFilter || _filter.Text == "")
                            yield return idx;
                        else
                        {
                            foreach (Node idxInner in idx)
                            {
                                if (idxInner.Value is string)
                                {
                                    if (_filter.Text.Contains(":"))
                                    {
                                        string[] splits = _filter.Text.Split(':');
                                        if (splits[0].ToLower() == idxInner.Name.ToLower())
                                        {
                                            if (idxInner.Get<string>().ToLower().Contains(splits[1].ToLower()))
                                            {
                                                yield return idx;
                                                break;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (idxInner.Get<string>().ToLower().Contains(_filter.Text.ToLower()))
                                        {
                                            yield return idx;
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    
                }
            }
        }

        /**
         * If true (default) then a TextBox for filtering items will be visible
         */
        [DefaultValue(true)]
        public bool EnableFilter
        {
            get { return ViewState["EnableFilter"] == null ? true : (bool)ViewState["EnableFilter"]; }
            set { ViewState["EnableFilter"] = value; }
        }

        /**
         * If true (default) the current row will be deleted when the deletion icon is clicked
         * in the grid. Notice that obviously the grid must allow deletion for this to have any meaning.
         */
        [DefaultValue(true)]
        public bool AutoDeleteRowOnDeletion
        {
            get { return ViewState["AutoDeleteRowOnDeletion"] == null ? true : (bool)ViewState["AutoDeleteRowOnDeletion"]; }
            set { ViewState["AutoDeleteRowOnDeletion"] = value; }
        }

        /**
         * If true (default) then the grid will display headers for columns 
         * with the given caption from the DataSource.
         */
        [DefaultValue(true)]
        public bool EnableHeaders
        {
            get { return ViewState["EnableHeaders"] == null ? true : (bool)ViewState["EnableHeaders"]; }
            set { ViewState["EnableHeaders"] = value; }
        }

        /**
         * If true (default) then items (rows) in the Grid can be deleted.
         */
        [DefaultValue(true)]
        public bool EnableDeletion
        {
            get { return ViewState["EnableDeletion"] == null ? true : (bool)ViewState["EnableDeletion"]; }
            set { ViewState["EnableDeletion"] = value; }
        }

        /**
         * The number of items to show in the grid before user needs to start paging back and forth.
         * Default value is 10.
         */
        [DefaultValue(10)]
        public int PageSize
        {
            get { return ViewState["PageSize"] == null ? 10 : (int)ViewState["PageSize"]; }
            set { ViewState["PageSize"] = value; }
        }

        /**
         * The current active page viewed, default is 0.
         */
        [DefaultValue(0)]
        public int CurrentPage
        {
            get { return ViewState["CurrentPage"] == null ? 0 : (int)ViewState["CurrentPage"]; }
            set { ViewState["CurrentPage"] = value; }
        }

        private string OldFilter
        {
            get { return ViewState["OldFilter"] as string; }
            set { ViewState["OldFilter"] = value; }
        }

        /**
         * Which column items are sorted according to.
         */
        public string SortColumn
        {
            get { return ViewState["SortColumn"] as string; }
            set { ViewState["SortColumn"] = value; }
        }

        /**
         * Shorthand for being able to show the last page, useful when adding new rows
         * and you want to display the newly added row.
         */
        public void PageToEnd()
        {
            CurrentPage = (DataSource["Rows"].Count - 1) / PageSize;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            EnsureChildControls();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (DataSource != null)
                DataBindGrid();
        }

        protected override void OnPreRender(EventArgs e)
        {
            if (PageSize == -1 && !EnableFilter)
                _ctrlWrappers.Visible = false;
            else
                _ctrlWrappers.Visible = true;

            _filter.Visible = EnableFilter;

            _previous.Visible = PageSize != -1;

            _next.Visible = PageSize != -1;

            if (PageSize != -1)
            {
                // Showing something like this; "1-10/38"
                List<Node> tmp = new List<Node>(RowsFiltered);
                _count.Text = string.Format("{1}-{2} / {0}",
                    tmp.Count,
                    CurrentPage * PageSize,
                    Math.Min((CurrentPage * PageSize) + PageSize, tmp.Count));
            }
            if (_gridWasDatabinded && EnableFilter)
            {
                new EffectFocusAndSelect(_filter)
                    .Render(); 
            }
            base.OnPreRender(e);
        }

        protected override void CreateChildControls()
        {
            base.CreateChildControls();

            _ctrlWrappers = new Panel {ID = "wrp", CssClass = "controlWrappers"};
            _ctrlWrappers.Style[Styles.opacity] = "0.3";
            Controls.Add(_ctrlWrappers);

            BehaviorUnveiler unveiler = new BehaviorUnveiler {ID = "unveiler"};
            _ctrlWrappers.Controls.Add(unveiler);

            _filterWrapper = new Panel {ID = "fltW", DefaultWidget = "fltB"};

            _filter = new TextBox {ID = "flt", CssClass = "filter"};

            _fltBtn = new Button {ID = "fltB"};
            _fltBtn.Style[Styles.marginLeft] = "-1000px";
            _fltBtn.Click += 
                delegate
                {
                    // Making sure we select all "filter text" to mak it easy to "re-filter"...
                    _filter.Focus();
                    _filter.Select();

                    // Checking for dead keys...
                    if (OldFilter != _filter.Text)
                    {
                        CurrentPage = 0;
                        DataBindGrid();
                        _lstWrappers.ReRender();
                        _lstWrappers.Style[Styles.display] = "none";
                        new EffectFadeIn(_lstWrappers, 200)
                            .Render();
                        OldFilter = _filter.Text;
                    }
                };
            _filterWrapper.Controls.Add(_filter);
            _filterWrapper.Controls.Add(_fltBtn);
            _ctrlWrappers.Controls.Add(_filterWrapper);

            _previous = new LinkButton {ID = "prev", Text = "&nbsp;", CssClass = "previous"};
            _previous.Text = "&lt;&lt;";
            _previous.Click +=
                delegate
                {
                    if (CurrentPage != 0)
                    {
                        CurrentPage -= 1;
                        DataBindGrid();
                        _lstWrappers.ReRender();
                        new EffectRollUp(_lstWrappers, 200)
                            .ChainThese(
                                new EffectFadeIn(_lstWrappers, 200))
                            .Render();
                    }
                };
            _ctrlWrappers.Controls.Add(_previous);

            _count = new Label {ID = "cnt", CssClass = "count"};
            _ctrlWrappers.Controls.Add(_count);

            _next = new LinkButton {ID = "next", Text = "&nbsp;", CssClass = "next"};
            _next.Text = "&gt;&gt;";
            _next.Click +=
                delegate
                {
                    if ((CurrentPage + 1) * PageSize < DataSource["Rows"].Count)
                    {
                        CurrentPage += 1;
                        DataBindGrid();
                        _lstWrappers.ReRender();
                        new EffectRollUp(_lstWrappers, 200)
                            .ChainThese(
                                new EffectFadeIn(_lstWrappers, 200))
                            .Render();
                    }
                };
            _ctrlWrappers.Controls.Add(_next);

            _lstWrappers = new Panel {ID = "lstWrp", CssClass = "gridWrapper"};
            Controls.Add(_lstWrappers);
        }

        private void DataBindGrid()
        {
            // Clearing any previously databindings...
            _lstWrappers.Controls.Clear();

            // Creating Headers...
            Label table = new Label {Tag = "table", ID = "tbl", CssClass = "table"};

            if (EnableHeaders)
                CreateHeaders(table);
            CreateRows(table);

            _lstWrappers.Controls.Add(table);
        }

        public void Rebind()
        {
            // Clearing any previously databindings...
            _lstWrappers.Controls.Clear();

            // Creating Headers...
            Label table = new Label {Tag = "table", ID = "tbl", CssClass = "table"};

            CreateHeaders(table);
            CreateRows(table);

            _lstWrappers.Controls.Add(table);
            _lstWrappers.ReRender();
        }

        private void CreateRows(RaWebControl table)
        {
            Node node = new Node();
            List<Node> tmp = new List<Node>(RowsFiltered);
            tmp.Sort(
                delegate(Node left, Node right)
                {
                    if (SortColumn == null)
                    {
                        // Defaulting to first "sane" sortable... (which is "ID" if given)
                        if (left[0].Name == "ID" && left[0].Value is int)
                            return ((int)left[0].Value).CompareTo((int)right[0].Value);
                        if (left[0].Value == null)
                            return right[0].Value == null ? 0 : - 1;
                        if (right[0].Value == null)
                            return 1;
                        return left[0].Value.ToString().CompareTo(right[0].Value.ToString());
                    }
                    bool isBackwards = SortColumn[0] == '-';
                    string sortCol = SortColumn.Replace("-", "");
                    if (left[sortCol].Value == null)
                        return right[sortCol].Value == null ? 0 : (isBackwards ? 1 : -1);
                    if (right[sortCol].Value == null)
                        return isBackwards ? -1 : 1;

                    object leftObj = left[sortCol].Value;
                    object rightObj = right[sortCol].Value;
                    if (leftObj == null)
                        return rightObj == null ? 0 : (isBackwards ? -1 : 1);
                    if (rightObj == null)
                        return isBackwards ? 1 : -1;
                    if (!isBackwards)
                        return ((IComparable)leftObj).CompareTo(rightObj);
                    return ((IComparable)rightObj).CompareTo(leftObj);
                });
            foreach (Node idx in tmp)
            {
                node.Add(idx);
            }

            // Settings previous and next buttons to disabled/enabled according to placement in
            // the datasource paging logic...
            _previous.CssClass = CurrentPage == 0 ? "previous-disabled" : "previous";
            _next.CssClass = (CurrentPage * PageSize) + PageSize >= node.Count ? "next-disabled" : "next";

            int idxNo = 0;
            int idxStartBinding = 0;
            foreach (Node idxRow in node)
            {
                if (PageSize == -1 || idxStartBinding >= CurrentPage * PageSize)
                {
                    // If PageSize == -1 we add ALL records...!!!!
                    Label row = new Label
                    {
                        Tag = "tr", 
                        CssClass = idxNo%2 == 0 ? "even" : "odd", 
                        ID = "row" + idxNo
                    };
                    table.Controls.Add(row);
                    int idxNoCell = 0;
                    foreach (Node idxCell in idxRow)
                    {
                        if (idxCell.Name == "ID")
                            continue;
                        HtmlTableCell cell = new HtmlTableCell
                        {
                            ID = "cell" + idxNo + "x" + idxNoCell
                        };
                        row.Controls.Add(cell);
                        string fieldType = DataSource["Columns"][idxCell.Name]["ControlType"].Get<string>();
                        switch (fieldType)
                        {
                            case "LinkButton":
                                {
                                    LinkButton lb = new LinkButton
                                    {
                                        ID = "ctrl" + idxNo + "x" + idxNoCell,
                                        Xtra = idxRow["ID"].Value + "|" + idxCell.Name,
                                        Text = idxCell.Get<string>()
                                    };
                                    if (Action != null)
                                    {
                                        lb.Click +=
                                            delegate(object sender, EventArgs e)
                                            {
                                                LinkButton rl = sender as LinkButton;
                                                if (rl == null)
                                                    return;
                                                string id = rl.Xtra.Split('|')[0];
                                                string columnName = rl.Xtra.Split('|')[1];
                                                Action(this, new GridActionEventArgs(id, columnName));
                                            }; 
                                    }
                                    cell.Controls.Add(lb);
                                } break;
                            case "InPlaceEdit":
                                {
                                    InPlaceEdit edit = new InPlaceEdit
                                    {
                                        ID = "ctrl" + idxNo + "x" + idxNoCell,
                                        CssClass = "edit",
                                        Xtra = idxRow["ID"].Value + "|" + idxCell.Name,
                                        Text = idxCell.Get<string>()
                                    };
                                    if (CellEdited != null)
                                    {
                                        edit.TextChanged +=
                                            delegate(object sender, EventArgs e)
                                            {
                                                InPlaceEdit ed = sender as InPlaceEdit;
                                                if (ed != null)
                                                {
                                                    string id = ed.Xtra.Split('|')[0];
                                                    string cellName = ed.Xtra.Split('|')[1];
                                                    GridEditEventArgs eIn = new GridEditEventArgs(id, cellName, ed.Text);
                                                    CellEdited(this, eIn);
                                                    if (!eIn.AcceptChange)
                                                        return;
                                                    Node rowNode = DataSource["Rows"].Find(
                                                        delegate(Node idxNode)
                                                        {
                                                            if (idxNode.Name != "ID")
                                                            {
                                                                return false;
                                                            }
                                                            return idxNode.Value.ToString() == id;
                                                        }).Parent;
                                                    Node cellNode = rowNode.Find(
                                                        delegate(Node idxNode)
                                                        {
                                                            return idxNode.Name == cellName;
                                                        });
                                                    cellNode.Value = eIn.NewValue.ToString();
                                                }
                                            };
                                    }
                                    cell.Controls.Add(edit);
                                } break;
                            case "List":
                                {
                                    SelectList edit = new SelectList
                                    {
                                        ID = "ctrl" + idxNo + "x" + idxNoCell + idxRow["ID"].Value,
                                        Xtra = idxRow["ID"].Value + "|" + idxCell.Name
                                    };
                                    string valueSelected = idxCell.Get<string>();
                                    foreach (Node idx in DataSource["Columns"][idxCell.Name]["Values"])
                                    {
                                        string idxValue = idx.Get<string>();
                                        ListItem l = new ListItem(idxValue, idxValue);
                                        if(idxValue == valueSelected)
                                            l.Selected = true;
                                        edit.Items.Add(l);
                                    }
                                    if (CellEdited != null)
                                    {
                                        edit.SelectedIndexChanged +=
                                            delegate(object sender, EventArgs e)
                                            {
                                                SelectList ed = sender as SelectList;
                                                if (ed != null)
                                                {
                                                    string id = ed.Xtra.Split('|')[0];
                                                    string cellName = ed.Xtra.Split('|')[1];
                                                    GridEditEventArgs eIn = new GridEditEventArgs(id, cellName, ed.SelectedItem.Value);
                                                    CellEdited(this, eIn);
                                                    if (!eIn.AcceptChange)
                                                        return;
                                                    Node rowNode = DataSource["Rows"].Find(
                                                        delegate(Node idxNode)
                                                        {
                                                            if (idxNode.Name != "ID")
                                                            {
                                                                return false;
                                                            }
                                                            return idxNode.Value.ToString() == id;
                                                        }).Parent;
                                                    Node cellNode = rowNode.Find(
                                                        delegate(Node idxNode)
                                                        {
                                                            return idxNode.Name == cellName;
                                                        });
                                                    cellNode.Value = eIn.NewValue.ToString();
                                                }
                                            };
                                    }
                                    cell.Controls.Add(edit);
                                } break;
                            case "Label":
                                if (idxCell.Value is string)
                                {
                                    cell.InnerHtml = idxCell.Get<string>();
                                }
                                else if (idxCell.Value is DateTime)
                                {
                                    cell.InnerHtml = idxCell.Get<DateTime>().ToString(
                                        idxCell["Format"].Get<string>(), CultureInfo.CurrentUICulture);
                                }
                                break;
                            case "Link":
                                {
                                    if (idxCell["target"].Value != null && 
                                        idxCell["target"].Get<string>() == "same")
                                        cell.InnerHtml =
                                            string.Format(@"<a href=""{0}"">{1}</a>",
                                                idxCell["href"].Value,
                                                idxCell.Get<string>());
                                    else
                                        cell.InnerHtml =
                                            string.Format(@"<a href=""{0}"" target=""_blank"">{1}</a>",
                                                idxCell["href"].Value,
                                                idxCell.Get<string>());
                                } break;
                            default:
                                {
                                    if (fieldType.IndexOf("TextAreaEdit") == 0)
                                    {
                                        int lengthOfText = -1;
                                        if (fieldType.Contains("["))
                                        {
                                            lengthOfText = int.Parse(fieldType.Split('[', ']')[1]);
                                        }
                                        TextAreaEdit edit = new TextAreaEdit();
                                        if (lengthOfText != -1)
                                        {
                                            edit.TextLength = lengthOfText;
                                        }
                                        edit.ID = "ctrl" + idxNo + "x" + idxNoCell;
                                        edit.CssClass = "edit";
                                        edit.Xtra = idxRow["ID"].Value + "|" + idxCell.Name;
                                        edit.Text = idxCell.Get<string>();
                                        if (CellEdited != null)
                                        {
                                            edit.TextChanged +=
                                                delegate(object sender, EventArgs e)
                                                {
                                                    TextAreaEdit ed = sender as TextAreaEdit;
                                                    if (ed == null) 
                                                        return;
                                                    string id = ed.Xtra.Split('|')[0];
                                                    string cellName = ed.Xtra.Split('|')[1];
                                                    GridEditEventArgs eIn = new GridEditEventArgs(id, cellName, ed.Text);
                                                    CellEdited(this, eIn);
                                                    if (eIn.AcceptChange)
                                                    {
                                                        Node rowNode = DataSource["Rows"].Find(
                                                            delegate(Node idxNode)
                                                                {
                                                                    if (idxNode.Name != "ID")
                                                                    {
                                                                        return false;
                                                                    }
                                                                    return idxNode.Get<string>() == id;
                                                                }).Parent;
                                                        Node cellNode = rowNode.Find(
                                                            delegate(Node idxNode)
                                                                {
                                                                    return idxNode.Name == cellName;
                                                                });
                                                        cellNode.Value = eIn.NewValue.ToString();
                                                    }
                                                };
                                        }
                                        cell.Controls.Add(edit);
                                    }
                                    else
                                    {
                                        Node getColType = new Node();
                                        getColType["ColumnType"].Value = DataSource["Columns"][idxCell.Name]["ControlType"].Get<string>();
                                        getColType["Row"].Value = row;
                                        getColType["Value"].Value = idxCell.Value;
                                        getColType["DataSource"].Value = DataSource;
                                        getColType["ID"].Value = idxRow["ID"].Value;
                                        getColType["CellName"].Value = idxCell.Name;
                                        ActiveEvents.Instance.RaiseActiveEvent(
                                            this,
                                            "GetGridColumnType",
                                            getColType);
                                        RaWebControl ctrl = getColType["Control"].Get<RaWebControl>();
                                        ctrl.ID = idxRow["ID"].Value + "x" + idxCell.Name.GetHashCode().ToString().Replace("|", "");
                                        ctrl.Xtra = string.Format("{0}|{1}", idxRow["ID"].Value, idxCell.Name);
                                        if (getColType["ExtraCssClass"].Value != null)
                                        {
                                            if(!table.CssClass.Contains(getColType["ExtraCssClass"].Get<string>()))
                                            {
                                                table.CssClass += " " + getColType["ExtraCssClass"].Get<string>();
                                            }
                                        }
                                        cell.Controls.Add(ctrl);
                                    }
                                } break;
                        }
                        idxNoCell += 1;
                    }

                    // Adding delete row (if we should)
                    if (RowDeleted != null)
                    {
                        HtmlTableCell delete = new HtmlTableCell {ID = "del" + idxNo};

                        LinkButton lb = new LinkButton
                        {
                            ID = "delBtn" + idxNo,
                            Text = "&nbsp;",
                            CssClass = "deleteBtn",
                            Xtra = idxRow["ID"].Value.ToString(),
                            Visible = EnableDeletion
                        };
                        lb.Click +=
                            delegate(object sender, EventArgs e)
                            {
                                LinkButton bt = sender as LinkButton;
                                if (bt != null)
                                {
                                    RowDeleted(this, new GridActionEventArgs(bt.Xtra, null));
                                    if (AutoDeleteRowOnDeletion)
                                    {
                                        foreach (Node idx in DataSource["Rows"])
                                        {
                                            if (idx["ID"].Value.ToString() == bt.Xtra)
                                            {
                                                DataSource["Rows"].Remove(idx);
                                                DataBindGrid();
                                                _lstWrappers.ReRender();
                                                break;
                                            }
                                        }
                                    }
                                }
                            };

                        delete.Controls.Add(lb);
                        row.Controls.Add(delete);
                    }
                }
                idxNo += 1;
                idxStartBinding += 1;
                if (idxNo == (PageSize * CurrentPage) + PageSize)
                    break;
            }
        }

        private void CreateHeaders(Control table)
        {
            Label row = new Label {Tag = "tr", ID = "head"};
            foreach (Node idx in DataSource["Columns"])
            {
                Label cell = new Label
                {
                    Tag = "td",
                    ID = idx["Caption"].Get<string>().Replace("|", "").Replace(" ", "")
                };

                ExtButton btn = new ExtButton
                {
                    ID = "b" + idx["Caption"].Get<string>().Replace("|", "").Replace(" ", ""),
                    Text = idx["Caption"].Get<string>(),
                    Xtra = idx.Name
                };
                if (SortColumn == btn.Xtra)
                {
                    btn.CssClass = "button headerButton clicked";
                }
                else if (SortColumn == "-" + btn.Xtra)
                {
                    btn.CssClass = "button headerButton clicked-backwards";
                }
                else
                {
                    btn.CssClass = "button headerButton";
                }
                btn.Click +=
                    delegate(object sender, EventArgs e)
                    {
                        ExtButton bt = sender as ExtButton;
                        if (bt != null)
                        {
                            bool backwards = SortColumn == bt.Xtra;
                            SortColumn = backwards ? "-" + bt.Xtra : bt.Xtra;
                        }
                        DataBindGrid();
                        _lstWrappers.ReRender();
                    };
                cell.Controls.Add(btn);

                cell.Style["width"] = (100 / DataSource["Columns"].Count) + "%";
                row.Controls.Add(cell);
            }
            table.Controls.Add(row);
        }
    }
}
















