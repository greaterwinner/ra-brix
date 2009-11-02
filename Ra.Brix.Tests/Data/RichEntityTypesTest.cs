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

namespace Ra.Brix.Tests.Data
{
    [TestFixture]
    public class RichEntityTypesTest : BaseTest
    {
        [ActiveType]
        internal class AllProperties : ActiveType<AllProperties>
        {
            [ActiveField]
            public int IntBugger { get; set; }

            [ActiveField]
            public string StringBugger { get; set; }

            [ActiveField]
            public DateTime DateBugger { get; set; }

            [ActiveField]
            public bool BoolBugger { get; set; }

            [ActiveField]
            public decimal DecimalBugger { get; set; }

            [ActiveField]
            public byte[] BLOBBugger { get; set; }

        }

        [Test]
        public void TestEntityTypeSingleSave()
        {
            SetUp();
            //Database.Instance.RegisterEntityType<AllProperties>();
            AllProperties a = new AllProperties();
            a.BLOBBugger = new byte[] { 3, 7, 1 };
            a.BoolBugger = true;
            a.DateBugger = new DateTime(2009, 02, 15, 23, 11, 52);
            a.DecimalBugger = 56789.54M;
            a.IntBugger = 6543;
            a.StringBugger = "qwerty";
            a.Save();

            int aID = a.ID;

            AllProperties a2 = AllProperties.SelectByID(aID);
            Assert.IsNotNull(a2);
            Assert.AreEqual(3, a2.BLOBBugger.Length);
            Assert.AreEqual(3, a2.BLOBBugger[0]);
            Assert.AreEqual(7, a2.BLOBBugger[1]);
            Assert.AreEqual(1, a2.BLOBBugger[2]);
            Assert.AreEqual(true, a2.BoolBugger);
            Assert.AreEqual(new DateTime(2009, 02, 15, 23, 11, 52), a2.DateBugger);
            Assert.AreEqual(56789.54M, a2.DecimalBugger);
            Assert.AreEqual(6543, a2.IntBugger);
            Assert.AreEqual("qwerty", a2.StringBugger);
        }

        [Test]
        public void TestEntityTypeSaneDefaultsAndChanged()
        {
            SetUp();
            //Database.Instance.RegisterEntityType<AllProperties>();
            AllProperties a = new AllProperties();
            a.Save();

            int aID = a.ID;

            AllProperties a2 = AllProperties.SelectByID(aID);
            Assert.IsNotNull(a2);
            Assert.AreEqual(null, a2.BLOBBugger);
            Assert.AreEqual(false, a2.BoolBugger);
            Assert.AreEqual(DateTime.MinValue, a2.DateBugger);
            Assert.AreEqual(0.0M, a2.DecimalBugger);
            Assert.AreEqual(0, a2.IntBugger);
            Assert.AreEqual(null, a2.StringBugger);

            a2.BLOBBugger = new byte[] { 3, 7, 1 };
            a2.BoolBugger = true;
            a2.DateBugger = new DateTime(2009, 02, 15, 23, 11, 52);
            a2.DecimalBugger = 56789.54M;
            a2.IntBugger = 6543;
            a2.StringBugger = "qwerty";
            a2.Save();

            a2 = AllProperties.SelectByID(aID);
            Assert.IsNotNull(a2);
            Assert.AreEqual(3, a2.BLOBBugger.Length);
            Assert.AreEqual(3, a2.BLOBBugger[0]);
            Assert.AreEqual(7, a2.BLOBBugger[1]);
            Assert.AreEqual(1, a2.BLOBBugger[2]);
            Assert.AreEqual(true, a2.BoolBugger);
            Assert.AreEqual(new DateTime(2009, 02, 15, 23, 11, 52), a2.DateBugger);
            Assert.AreEqual(56789.54M, a2.DecimalBugger);
            Assert.AreEqual(6543, a2.IntBugger);
            Assert.AreEqual("qwerty", a2.StringBugger);
        }

        [Test]
        public void TestEntityTypeHUGE_BLOB()
        {
            int noItems = 50000;
            SetUp();
            AllProperties a = new AllProperties();
            byte[] bytes = new byte[noItems];
            for (int idx = 0; idx < noItems; idx++)
            {
                bytes[idx] = (byte)(idx % 250);
            }
            a.BLOBBugger = bytes;
            a.Save();

            int aID = a.ID;

            AllProperties a2 = AllProperties.SelectByID(aID);
            Assert.IsNotNull(a2);
            Assert.AreEqual(noItems, a2.BLOBBugger.Length);
            for (int idx = 0; idx < noItems; idx++)
            {
                Assert.AreEqual((byte)(idx % 250), a2.BLOBBugger[idx]);
            }
        }

        [Test]
        public void TestEntityTypeSetValuesAndResetToNull()
        {
            SetUp();
            //Database.Instance.RegisterEntityType<AllProperties>();
            AllProperties a = new AllProperties();
            a.BLOBBugger = new byte[] { 3, 7, 1 };
            a.BoolBugger = true;
            a.DateBugger = new DateTime(2009, 02, 15, 23, 11, 52);
            a.DecimalBugger = 56789.54M;
            a.IntBugger = 6543;
            a.StringBugger = "qwerty";
            a.Save();

            int aID = a.ID;

            AllProperties a2 = AllProperties.SelectByID(aID);
            a2 = AllProperties.SelectByID(aID);
            Assert.IsNotNull(a2);
            Assert.AreEqual(3, a2.BLOBBugger.Length);
            Assert.AreEqual(3, a2.BLOBBugger[0]);
            Assert.AreEqual(7, a2.BLOBBugger[1]);
            Assert.AreEqual(1, a2.BLOBBugger[2]);
            Assert.AreEqual(true, a2.BoolBugger);
            Assert.AreEqual(new DateTime(2009, 02, 15, 23, 11, 52), a2.DateBugger);
            Assert.AreEqual(56789.54M, a2.DecimalBugger);
            Assert.AreEqual(6543, a2.IntBugger);
            Assert.AreEqual("qwerty", a2.StringBugger);

            a2.BLOBBugger = null;
            a2.StringBugger = null;
            a2.Save();

            a2 = AllProperties.SelectByID(aID);
            Assert.IsNotNull(a2);
            Assert.AreEqual(null, a2.BLOBBugger);
            Assert.AreEqual(null, a2.StringBugger);
        }
    }
}
