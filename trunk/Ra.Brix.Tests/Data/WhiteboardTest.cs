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
using NUnit.Core;
using NUnit.Framework;
using Ra.Brix.Data;
using System.Collections.Generic;
using Ra.Brix.Types;
using WhiteBoardRecords;

namespace Ra.Brix.Tests.Data
{
    [TestFixture]
    public class WhiteboardTest : BaseTest
    {
        [Test]
        public void TestSaveWhiteboard()
        {
            SetUp();
            Whiteboard w = CreateStructure();

            Whiteboard w2 = Whiteboard.SelectByID(w.ID);
            Assert.AreEqual(true, w2.EnableDeletion);
            Assert.AreEqual(true, w2.EnableFiltering);
            Assert.AreEqual(false, w2.EnableHeaders);

            Assert.AreEqual("Whiteboard1", w2.Name);
            Assert.AreEqual(8, w2.PageSize);

            Assert.AreEqual(2, w2.Columns.Count);
            Assert.AreEqual("Patient", w2.Columns[0].Caption);
            Assert.AreEqual("Doctor", w2.Columns[1].Caption);
            Assert.AreEqual("InPlaceEdit", w2.Columns[0].Type);
            Assert.AreEqual("InPlaceEdit", w2.Columns[1].Type);
        }

        [Test]
        public void TestSaveAndUpdateWhiteboard()
        {
            SetUp();
            CreateStructure();

            Whiteboard w = Whiteboard.SelectFirst();
            Whiteboard.Row row = new Whiteboard.Row();

            Whiteboard.Cell c1 = new Whiteboard.Cell();
            c1.Column = w.Columns[0];
            c1.Value = "howdy1";
            row.Cells.Add(c1);

            Whiteboard.Cell c2 = new Whiteboard.Cell();
            c2.Column = w.Columns[1];
            c2.Value = "howdy2";
            row.Cells.Add(c2);

            w.Rows.Add(row);
            w.Save();

            Whiteboard w1 = Whiteboard.SelectByID(w.ID);
            Assert.AreEqual(1, w1.Rows.Count);
            Assert.AreEqual(2, w1.Rows[0].Cells.Count);
            Assert.AreEqual("howdy1", w1.Rows[0].Cells[0].Value);
            Assert.AreEqual("howdy2", w1.Rows[0].Cells[1].Value);

            Assert.AreEqual("Patient", w1.Rows[0].Cells[0].Column.Caption);
            Assert.AreEqual("Doctor", w1.Rows[0].Cells[1].Column.Caption);

            // Trying to update cell and see if it works correctly...
            w1.Rows[0].Cells[0].Value = "howdy1-2";
            w1.Save();

            w1 = Whiteboard.SelectByID(w.ID);
            Assert.AreEqual(1, w1.Rows.Count);
            Assert.AreEqual(2, w1.Rows[0].Cells.Count);
            Assert.AreEqual("howdy1-2", w1.Rows[0].Cells[0].Value);
            Assert.AreEqual("howdy2", w1.Rows[0].Cells[1].Value);

            Assert.AreEqual("Patient", w1.Rows[0].Cells[0].Column.Caption);
            Assert.AreEqual("Doctor", w1.Rows[0].Cells[1].Column.Caption);

            w1.Rows.RemoveAt(0);
            w1.Save();
        }

        private static Whiteboard CreateStructure()
        {
            Whiteboard w = new Whiteboard();
            w.EnableDeletion = true;
            w.EnableFiltering = true;
            w.EnableHeaders = false;
            w.Name = "Whiteboard1";
            w.PageSize = 8;

            Whiteboard.Column c1 = new Whiteboard.Column();
            c1.Caption = "Patient";
            c1.Position = 0;
            c1.Type = "InPlaceEdit";
            w.Columns.Add(c1);

            Whiteboard.Column c2 = new Whiteboard.Column();
            c2.Caption = "Doctor";
            c2.Position = 1;
            c2.Type = "InPlaceEdit";
            w.Columns.Add(c2);

            w.Save();
            return w;
        }
    }
}
