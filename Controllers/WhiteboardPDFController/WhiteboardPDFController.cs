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
using LanguageRecords;
using MigraDoc.DocumentObjectModel;
using MigraDoc.Rendering;
using PdfSharp.Pdf;
using Ra;
using Ra.Brix.Data;
using Ra.Brix.Loader;
using Ra.Widgets;
using SettingsRecords;
using WhiteBoardRecords;

namespace WhiteboardPDFController
{
    [ActiveController]
    public class WhiteboardPDFController
    {
        [ActiveEvent(Name = "GetTipOfToday")]
        protected static void GetTipOfToday(object sender, ActiveEventArgs e)
        {
            const string tmp = @"
Did you know that you can use the ControlType of ""ToPDF"" to create a column where
you can click a button to export rows from the Whiteboard to PDF documents?

Just add up a Whiteboard column to your favorite Whiteboard and make sure that its type
is ""ToPDF"" and view your whiteboard. Then click the ""Save PDF..."" link in any one
of your Whiteboard records, and your PDF will be downloaded containing every cell
from your Whiteboard record.
";
            e.Params["Tip"]["TipOfWhiteBoardPDFExport"].Value =
                Language.Instance["TipOfWhiteBoardPDFExport", null, tmp];
        }

        [ActiveEvent(Name = "GetTipOfToday")]
        protected static void GetTipOfToday2(object sender, ActiveEventArgs e)
        {
            const string tmp = @"
Did you know that you can save your Whiteboard PDF files to any folder?

By changing the setting ""FolderToSavePDFsTo"" to whatever folder you wish your PDFs
will be saved to thet folder instead of the ""Resources"" folder which is the default.

If you choose a folder which is underneath your ""Resources"" folder on your server, you
will make it possible to embed PDF documents you create this way as resources and
browse them using the Resource explorer etc.
";
            e.Params["Tip"]["TipOfWhiteBoardPDFSaveFilesToAnyFolder"].Value =
                Language.Instance["TipOfWhiteBoardPDFSaveFilesToAnyFolder", null, tmp];
        }

        [ActiveEvent(Name = "GetTipOfToday")]
        protected static void GetTipOfToday3(object sender, ActiveEventArgs e)
        {
            const string tmp = @"
Did you know that you can choose to save your Whiteboard PDF files without the date portion 
of the filename. By choosing to do so anytime you save a new version of your PDF files then
this will overwrite any previously saved versions.

The name of this setting is ""SaveWhiteboardPDFWithDate"". Set this value to ""False""
and you won't use today's date or time as part of the filename of your saved PDFs.
";
            e.Params["Tip"]["TipOfWhiteBoardPDFSaveFilesWithoutDate"].Value =
                Language.Instance["TipOfWhiteBoardPDFSaveFilesWithoutDate", null, tmp];
        }

        [ActiveEvent(Name = "GetWhiteboardColumnTypes")]
        protected void GetWhiteboardColumnTypes(object sender, ActiveEventArgs e)
        {
            e.Params["Types"]["ToPDF"].Value = true;
        }

        [ActiveEvent(Name = "GetGridColumnType")]
        protected void GetGridColumnType(object sender, ActiveEventArgs e)
        {
            string controlType = e.Params["ColumnType"].Get<string>();
            if (controlType == "ToPDF")
            {
                CreateToPDF(e);
            }
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
            // Retrieving Whiteboard and Whiteboard Column...
            Whiteboard board = ActiveType<Whiteboard>.SelectFirst(Criteria.HasChild(rowId));
            Whiteboard.Row row = board.Rows.Find(
                delegate(Whiteboard.Row idx)
                {
                    return idx.ID == rowId;
                });

            // Creating global information...
            string header = "Whiteboard " + board.Name + " row " + row.ID;
            string fileNameToSaveTo = "whiteboard-" + board.Name.Replace(" ", "-") + "-" + "row-" + row.ID;
            if(Settings.Instance.Get("SaveWhiteboardPDFWithDate", true))
            {
                fileNameToSaveTo += "-" + DateTime.Now.ToString("yyyy.MM.dd-HH.mm") + ".pdf";
            }
            else
            {
                fileNameToSaveTo += ".pdf";
            }

            // Creating document with META information
            Document document = new Document();
            document.Info.Title = header;

            // Creating header
            Section section = document.AddSection();
            Paragraph paragraph = section.AddParagraph();
            paragraph.Format.Font.Color = Color.FromCmyk(100, 30, 20, 50);
            paragraph.AddFormattedText(
                header,
                TextFormat.Bold);

            foreach (Whiteboard.Cell idx in row.Cells)
            {
                RenderCellToPdf(section, idx);
            }

            // Rendering document back to client
            PdfDocumentRenderer pdfRenderer = new PdfDocumentRenderer(true, PdfFontEmbedding.Always);
            pdfRenderer.Document = document;
            pdfRenderer.RenderDocument();
            string folderToSaveTo = Settings.Instance.Get("FolderToSavePDFsTo", "TemporaryFiles");
            string filename = HttpContext.Current.Server.MapPath("~/" + folderToSaveTo + "/" + fileNameToSaveTo);
            pdfRenderer.PdfDocument.Save(filename);
            AjaxManager.Instance.WriterAtBack.Write("window.open('" + folderToSaveTo + "/{0}');", fileNameToSaveTo);
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
    }
}
