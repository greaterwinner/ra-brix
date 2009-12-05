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
using System.Web;
using System.Web.UI;
using LanguageRecords;
using MigraDoc.DocumentObjectModel;
using MigraDoc.Rendering;
using Ra.Brix.Data;
using Ra.Brix.Loader;
using UserRecords;
using WhiteBoardRecords;
using Ra.Brix.Types;
using Ra.Widgets;
using Ra.Effects;
using Ra.Extensions.Widgets;
using System.Globalization;
using Ra.Selector;
using Styles=Ra.Widgets.Styles;
using PdfSharp.Pdf;
using Ra;

namespace WhiteboardPluginsController
{
    [ActiveController]
    public class WhiteboardTypes
    {
        [ActiveEvent(Name = "GetWhiteboardColumnTypes")]
        protected void GetWhiteboardColumnTypes(object sender, ActiveEventArgs e)
        {
            e.Params["Types"]["Boolean"].Value = true;
            e.Params["Types"]["Color"].Value = true;
            e.Params["Types"]["User"].Value = true;
            e.Params["Types"]["Strike"].Value = true;
            e.Params["Types"]["Date"].Value = true;
            e.Params["Types"]["ViewDetails"].Value = true;
            e.Params["Types"]["ToPDF"].Value = true;
        }

        [ActiveEvent(Name = "GetGridColumnType")]
        protected void GetGridColumnType(object sender, ActiveEventArgs e)
        {
            string controlType = e.Params["ColumnType"].Get<string>();
            if (controlType == "Boolean")
            {
                CreateBoolean(e);
            }
            else if (controlType == "Color")
            {
                CreateColor(e);
            }
            else if (controlType == "User")
            {
                CreateUser(e);
            }
            else if (controlType == "Strike")
            {
                CreateStrike(e);
            }
            else if (controlType == "Date")
            {
                CreateDate(e);
            }
            else if (controlType == "ViewDetails")
            {
                CreateViewDetails(e);
            }
            else if (controlType == "ToPDF")
            {
                CreateToPDF(e);
            }
        }

        [ActiveEvent(Name = "GetTipOfToday")]
        protected static void GetTipOfToday(object sender, ActiveEventArgs e)
        {
            const string tmp = @"
You can filter by a Date column in your Lists by writing in an ISO certified 
date pattern which is supposed to be constructed with the given pattern; yyyy.MM.dd HH:mm .
yyyy means 4 digits year value, MM means months with two digits, dd is days (two digits)
HH is hours in military display (e.g. 23 == 11PM) and mm is two digits minuts. 

So if you have a column named 'DateOfBirth' which you want to find out everyone who's born 
on the 23rd of December, 11:45PM in 1986 you'd write this into the Filter textbox; 
'DateOfBirth:1986.12.23 23:45'.
Or to find out anyone who is born in the February 2008 you'd write; 'DateOfBirth:2008.02' 
and nothing more.
";
            e.Params["Tip"]["TipOfWhiteBoardFilterByDateColumns"].Value = 
                Language.Instance["TipOfWhiteBoardFilterByDateColumns", null, tmp];
        }

        private void CreateToPDF(ActiveEventArgs e)
        {
            LinkButton btn = new LinkButton();
            btn.Text = Language.Instance["Save as PDF", null, "Save PDF..."];
            btn.Click +=
                delegate(object sender, EventArgs e2)
                {
                    LinkButton bt = (LinkButton)sender;
                    string[] xtra = bt.Xtra.Split('|');
                    int rowId = int.Parse(xtra[0]);
                    CreateAndRedirectToPDF(rowId);
                };
            e.Params["Control"].Value = btn;
        }

        private void CreateAndRedirectToPDF(int rowId)
        {
            Document document = new Document();

            // Creating header
            Section section = document.AddSection();
            Paragraph paragraph = section.AddParagraph();
            paragraph.Format.Font.Color = Color.FromCmyk(100, 30, 20, 50);
            paragraph.AddFormattedText(
                Language.Instance["WhiteboardRowDetails", null, "Whiteboard Row Details"], 
                TextFormat.Bold);

            Whiteboard.Row row = ActiveType<Whiteboard.Row>.SelectByID(rowId);

            foreach(Whiteboard.Cell idx in row.Cells)
            {
                RenderCellToPdf(section, idx);
            }

            // Rendering document back to client
            PdfDocumentRenderer pdfRenderer = new PdfDocumentRenderer(true, PdfFontEmbedding.Always);
            pdfRenderer.Document = document;
            pdfRenderer.RenderDocument();
            string filename = HttpContext.Current.Server.MapPath("~/TemporaryFiles/mumbo-jumbo.pdf");
            pdfRenderer.PdfDocument.Save(filename);
            AjaxManager.Instance.WriterAtBack.Write("window.open('TemporaryFiles/mumbo-jumbo.pdf');");
        }

        private void RenderCellToPdf(Section section, Whiteboard.Cell cell)
        {
            // Header
            Paragraph paragraph = section.AddParagraph();
            paragraph.Format.SpaceBefore = new Unit(10, UnitType.Point);
            paragraph.Format.LeftIndent = new Unit(10, UnitType.Point);
            paragraph.Format.Font.Color = Color.FromCmyk(100, 30, 20, 50);
            paragraph.AddFormattedText(
                cell.Column.Caption,
                TextFormat.Bold);
            
            // Value
            string value = cell.Value ?? "";
            paragraph = section.AddParagraph();
            paragraph.Format.LeftIndent = new Unit(20, UnitType.Point);
            paragraph.Format.Font.Color = Color.FromCmyk(100, 120, 120, 120);
            paragraph.AddFormattedText(value);
        }

        private void CreateViewDetails(ActiveEventArgs e)
        {
            LinkButton btn = new LinkButton();
            btn.Text = Language.Instance["ViewDetails", null, "View..."];
            btn.Click +=
                delegate(object sender, EventArgs e2)
                {
                    LinkButton bt = (LinkButton)sender;
                    string[] xtra = bt.Xtra.Split('|');
                    int rowId = int.Parse(xtra[0]);
                    Node node = new Node();
                    node["RowID"].Value = rowId;
                    ActiveEvents.Instance.RaiseActiveEvent(
                        this,
                        "ShowWhiteBoardColumnDetailsView",
                        node);
                };
            e.Params["Control"].Value = btn;
        }

        [ActiveEvent(Name = "ShowWhiteBoardColumnDetailsView")]
        protected void ShowWhiteBoardColumnDetailsView(object sender, ActiveEventArgs e)
        {
            Whiteboard.Row board = ActiveType<Whiteboard.Row>.SelectByID(e.Params["RowID"].Get<int>());
            Node node = new Node();
            node["ModuleSettings"]["Header"].Value = 
                Language.Instance["WhiteboardRowDetails", null, "Whiteboard Row Details"];
            int idxNo = 0;
            foreach (Whiteboard.Cell idx in board.Cells)
            {
                if (idx.Column.Type != "ViewDetails")
                {
                    node["ModuleSettings"]["Cells"]["Cell" + idxNo]["Caption"].Value = idx.Column.Caption;
                    node["ModuleSettings"]["Cells"]["Cell" + idxNo]["Value"].Value = idx.Value;
                    node["ModuleSettings"]["Cells"]["Cell" + idxNo]["Type"].Value = idx.Column.Type;
                    node["ModuleSettings"]["Cells"]["Cell" + idxNo]["Position"].Value = idx.Column.Position;
                    idxNo += 1;
                }
            }
            ActiveEvents.Instance.RaiseLoadControl(
                "WhiteboardModules.ViewWhiteboardDetails",
                "dynMaxi",
                node);
        }

        private static void CreateDate(ActiveEventArgs e)
        {
            string curValue = e.Params["Value"].Get<string>();

            Panel pnl = new Panel();

            // Creating LinkButton to open DateTimePicker
            LinkButton btn = new LinkButton();
            btn.Click +=
                delegate(object sender, EventArgs e2)
                    {
                        DateTimePicker pick = 
                            Selector.SelectFirst<DateTimePicker>(((Control) sender).Parent);
                        new EffectFadeIn(pick, 200)
                            .Render();
                    };
            pnl.Controls.Add(btn);

            // Creating DateTimePicker
            DateTimePicker picker = new DateTimePicker();
            picker.Style[Styles.display] = "none";
            picker.StartsOn = DayOfWeek.Sunday;
            picker.Style[Styles.width] = "200px";
            picker.Style[Styles.position] = "absolute";
            pnl.Controls.Add(picker);
            picker.Visible = false;
            if (!string.IsNullOrEmpty(curValue))
            {
                picker.Value = DateTime.ParseExact(curValue, "yyyy.MM.dd HH:mm", CultureInfo.InvariantCulture);
                btn.Text =
                    DateTime.ParseExact(curValue, "yyyy.MM.dd HH:mm", CultureInfo.InvariantCulture)
                        .ToString("ddd d.MMM yy HH:mm", CultureInfo.InvariantCulture);
            }
            else
            {
                btn.Text = DateTime.Now.ToString("ddd d.MMM yy HH:mm", CultureInfo.InvariantCulture);
            }
            picker.DateClicked +=
                delegate(object sender, EventArgs e2)
                    {
                        DateTimePicker pick = sender as DateTimePicker;
                        new EffectFadeOut(pick, 200)
                            .Render();
                        if (pick != null)
                        {
                            Panel bt = pick.Parent as Panel;
                            if (bt != null)
                            {
                                string[] xtra = bt.Xtra.Split('|');
                                int rowId = int.Parse(xtra[0]);
                                Whiteboard.Row row = ActiveType<Whiteboard.Row>.SelectByID(rowId);
                                Whiteboard.Cell cell = row.Cells.Find(
                                    delegate(Whiteboard.Cell idx)
                                        {
                                            return idx.Column.Caption == xtra[1];
                                        });
                                btn.Text =
                                    pick.Value.ToString("ddd d.MMM yy HH:mm", CultureInfo.InvariantCulture);
                                cell.Value = pick.Value.ToString("yyyy.MM.dd HH:mm", CultureInfo.InvariantCulture);
                                cell.Save();
                                UpdateGridValue(e.Params["DataSource"].Value as Node, xtra[0], xtra[1], cell.Value);
                            }
                        }
                    };

            // Sending back our Control
            e.Params["Control"].Value = pnl;
        }

        private static void CreateStrike(ActiveEventArgs e)
        {
            LinkButton btn = new LinkButton {Text = "X"};
            btn.Click +=
                delegate(object sender, EventArgs e2)
                    {
                        LinkButton bt = sender as LinkButton;
                        if (bt == null)
                            return;
                        string[] xtra = bt.Xtra.Split('|');
                        int rowId = int.Parse(xtra[0]);
                        Whiteboard.Row row = ActiveType<Whiteboard.Row>.SelectByID(rowId);
                        Whiteboard.Cell cell = row.Cells.Find(
                            delegate(Whiteboard.Cell idx)
                                {
                                    return idx.Column.Caption == xtra[1];
                                });
                        cell.Value = cell.Value == "striked" ? "" : "striked";
                        RaWebControl ctrl2 = ((RaWebControl)bt.Parent.Parent);
                        if (cell.Value == "striked")
                        {
                            ctrl2.CssClass += " strike";
                        }
                        else
                        {
                            ctrl2.CssClass = ctrl2.CssClass.Replace(" strike", "");
                        }
                        cell.Save();
                        UpdateGridValue(e.Params["DataSource"].Value as Node, xtra[0], xtra[1], cell.Value);
                    };
            string curValue = e.Params["Value"].Get<string>();
            RaWebControl ctrl = e.Params["Row"].Get<RaWebControl>();
            if (curValue == "striked")
            {
                ctrl.CssClass += " strike";
            }

            // Sending back our Control
            e.Params["Control"].Value = btn;
        }

        private static void CreateUser(ActiveEventArgs e)
        {
            SelectList list = new SelectList {CssClass = "smallSelectList"};

            // Adding first static item...
            ListItem top = new ListItem(Language.Instance["ChooseUser", null, "Choose User..."], "0");
            list.Items.Add(top);

            string curValue = e.Params["Value"].Get<string>();
            foreach (User idxUser in ActiveType<User>.Select())
            {
                ListItem i = new ListItem(idxUser.Username, idxUser.Username);
                list.Items.Add(i);
                if (curValue == idxUser.Username)
                {
                    i.Selected = true;
                }
            }

            list.SelectedIndexChanged +=
                delegate(object sender, EventArgs e2)
                    {
                        SelectList lst = sender as SelectList;
                        if (lst == null)
                            return;
                        string[] xtra = lst.Xtra.Split('|');
                        int rowId = int.Parse(xtra[0]);
                        Whiteboard.Row row = ActiveType<Whiteboard.Row>.SelectByID(rowId);
                        Whiteboard.Cell cell = row.Cells.Find(
                            delegate(Whiteboard.Cell idx)
                                {
                                    return idx.Column.Caption == xtra[1];
                                });
                        cell.Value = lst.SelectedItem.Value;
                        cell.Save();
                        UpdateGridValue(e.Params["DataSource"].Value as Node, xtra[0], xtra[1], cell.Value);
                    };

            // Sending back our Control
            e.Params["Control"].Value = list;
        }

        private static void CreateBoolean(ActiveEventArgs e)
        {
            CheckBox ch = new CheckBox
            {
                Text = Language.Instance["ClickToChange", null, ""]
            };
            if (e.Params["Value"].Get<string>() == "checked")
                ch.Checked = true;
            ch.CheckedChanged +=
                delegate(object sender2, EventArgs e2)
                    {
                        CheckBox bx = sender2 as CheckBox;
                        if (bx == null)
                            return;
                        string[] xtra = bx.Xtra.Split('|');
                        int rowId = int.Parse(xtra[0]);
                        Whiteboard.Row row = ActiveType<Whiteboard.Row>.SelectByID(rowId);
                        Whiteboard.Cell cell = row.Cells.Find(
                            delegate(Whiteboard.Cell idx)
                                {
                                    return idx.Column.Caption == xtra[1];
                                });
                        cell.Value = bx.Checked ? "checked" : "unchecked";
                        cell.Save();
                        UpdateGridValue(e.Params["DataSource"].Value as Node, xtra[0], xtra[1], cell.Value);
                    };
            e.Params["Control"].Value = ch;
        }

        private static void CreateColor(ActiveEventArgs e)
        {
            // Creating "button" Panel
            Panel lb = new Panel
            {
                CssClass = "colorLabel " + e.Params["Value"].Get<string>()
            };
            lb.Click +=
                delegate(object sender, EventArgs e2)
                    {
                        Panel s = sender as Panel;
                        if (s == null)
                            return;
                        Panel fader = s.Controls[1] as Panel;
                        new EffectRollDown(fader, 400)
                            .JoinThese(
                            new EffectFadeIn())
                            .Render();
                    };

            // Dummy literal to make sure the panel is visible
            System.Web.UI.WebControls.Literal lit = new System.Web.UI.WebControls.Literal
            {
                Text = "&nbsp;"
            };
            lb.Controls.Add(lit);

            // Our "context sensitive menu" Panel - to choose new Color
            Window pnl = new Window {CssClass = "light-window colorPanel"};
            pnl.Style[Styles.display] = "none";
            pnl.Click +=
                delegate(object sender, EventArgs e2)
                    {
                        Panel pl = sender as Panel;
                        new EffectRollUp(pl, 400)
                            .JoinThese(
                            new EffectFadeOut())
                            .Render();
                    };
            foreach (string idx in new[] { "Gray", "Red", "Blue", "Green", "Yellow" })
            {
                Label cur = new Label
                {
                    Text = "&nbsp;", CssClass = "colorButton " + idx, Xtra = idx
                };
                int rowId;
                cur.Click +=
                    delegate(object sender, EventArgs e2)
                        {
                            Label clicked = sender as Label;
                            if (clicked == null) 
                                return;
                            Panel bx = clicked.Parent.Parent.Parent.Parent.Parent.Parent as Panel;
                            if (bx == null) 
                                return;
                            string[] xtra = bx.Xtra.Split('|');
                            rowId = int.Parse(xtra[0]);
                            Whiteboard.Row row = ActiveType<Whiteboard.Row>.SelectByID(rowId);
                            Whiteboard.Cell cell = row.Cells.Find(
                                delegate(Whiteboard.Cell idxCell)
                                    {
                                        return idxCell.Column.Caption == xtra[1];
                                    });
                            cell.Value = clicked.Xtra;
                            cell.Save();
                            bx.CssClass = "colorLabel " + clicked.Xtra;
                            new EffectRollUp(clicked.Parent.Parent.Parent.Parent.Parent, 400)
                                .JoinThese(
                                new EffectFadeOut())
                                .Render();
                            UpdateGridValue(e.Params["DataSource"].Value as Node, xtra[0], xtra[1], clicked.Xtra);
                        };
                pnl.SurfaceControl.Style[Styles.overflow] = "auto";
                pnl.Content.Add(cur);
            }
            lb.Controls.Add(pnl);

            // Sending back our Control
            e.Params["Control"].Value = lb;
        }

        private static void UpdateGridValue(Node datasource, string idOfRow, string nameOfColumn, string newValue)
        {
            Node rowNode = datasource["Rows"].Find(
                delegate(Node idxNode)
                    {
                        if (idxNode.Name != "ID")
                        {
                            return false;
                        }
                        return idxNode.Value.ToString() == idOfRow;
                    }).Parent;
            Node cellNode = rowNode.Find(
                delegate(Node idxNode)
                    {
                        return idxNode.Name == nameOfColumn;
                    });
            cellNode.Value = newValue;
        }
    }
}