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

namespace Ra.Brix.Tests.Data
{
    [TestFixture]
    public class SelectByCriteriaTest : BaseTest
    {
        private DateTime date1 = new DateTime(1800, 1, 1, 23, 59, 37);
        private DateTime date2 = new DateTime(1954, 11, 24, 21, 37, 31);

        [ActiveRecord]
        public class Dummy : ActiveRecord<Dummy>
        {
            [ActiveField]
            public int IntValue { get; set; }

            [ActiveField]
            public string StringValue { get; set; }

            [ActiveField]
            public DateTime DateValue1 { get; set; }

            [ActiveField]
            public DateTime DateValue2 { get; set; }
        }

        [Test]
        public void SaveAndSelectByStringEquals()
        {
            SetUp();
            Dummy d = new Dummy();
            d.StringValue = "thomas";
            d.Save();

            Dummy d2 = Dummy.SelectFirst(Criteria.Eq("StringValue", "thomas"));
            Assert.IsNotNull(d2);
            Assert.AreEqual("thomas", d2.StringValue);
        }

        [Test]
        public void SaveAndSelectByStringNotEquals()
        {
            SetUp();
            Dummy d = new Dummy();
            d.StringValue = "thomas";
            d.Save();

            Dummy d2 = Dummy.SelectFirst(Criteria.Ne("StringValue", "thomas"));
            Assert.IsNull(d2);
        }

        [Test]
        public void SaveAndSelectByStringLike()
        {
            SetUp();
            Dummy d = new Dummy();
            d.StringValue = "thomas";
            d.Save();

            Dummy d2 = Dummy.SelectFirst(Criteria.Like("StringValue", "%oma%"));
            Assert.IsNotNull(d2);
            Assert.AreEqual("thomas", d2.StringValue);
        }

        [Test]
        public void SaveAndSelectByStringLikeNoMatch()
        {
            SetUp();
            Dummy d = new Dummy();
            d.StringValue = "thomas";
            d.Save();

            Dummy d2 = Dummy.SelectFirst(Criteria.Eq("StringValue", "qwe"));
            Assert.IsNull(d2);
        }

        [Test]
        public void SaveAndSelectByStringMoreThen()
        {
            SetUp();
            Dummy d = new Dummy();
            d.StringValue = "thomas";
            d.Save();

            Dummy d2 = Dummy.SelectFirst(Criteria.Mt("StringValue", "abc"));
            Assert.IsNotNull(d2);
            Assert.AreEqual("thomas", d2.StringValue);
        }

        [Test]
        public void SaveAndSelectByStringMoreThenNoMatch()
        {
            SetUp();
            Dummy d = new Dummy();
            d.StringValue = "thomas";
            d.Save();

            Dummy d2 = Dummy.SelectFirst(Criteria.Mt("StringValue", "xys"));
            Assert.IsNull(d2);
        }

        [Test]
        public void SaveAndSelectByStringLessThen()
        {
            SetUp();
            Dummy d = new Dummy();
            d.StringValue = "thomas";
            d.Save();

            Dummy d2 = Dummy.SelectFirst(Criteria.Lt("StringValue", "xys"));
            Assert.IsNotNull(d2);
            Assert.AreEqual("thomas", d2.StringValue);
        }

        [Test]
        public void SaveAndSelectByStringLessThenNoMatch()
        {
            SetUp();
            Dummy d = new Dummy();
            d.StringValue = "thomas";
            d.Save();

            Dummy d2 = Dummy.SelectFirst(Criteria.Lt("StringValue", "abc"));
            Assert.IsNull(d2);
        }

        [Test]
        public void SaveAndSelectByInt()
        {
            SetUp();
            Dummy d = new Dummy();
            d.IntValue = 100;
            d.Save();

            Dummy d2 = Dummy.SelectFirst(Criteria.Eq("IntValue", 100));
            Assert.IsNotNull(d2);
            Assert.AreEqual(100, d2.IntValue);
        }

        [Test]
        public void SaveAndSelectByIntNoMatch()
        {
            SetUp();
            Dummy d = new Dummy();
            d.IntValue = 100;
            d.Save();

            Dummy d2 = Dummy.SelectFirst(Criteria.Eq("IntValue", 99));
            Assert.IsNull(d2);
        }

        [Test]
        public void SaveAndSelectByIntLessThen()
        {
            SetUp();
            Dummy d = new Dummy();
            d.IntValue = 100;
            d.Save();

            Dummy d2 = Dummy.SelectFirst(Criteria.Lt("IntValue", 101));
            Assert.IsNotNull(d2);
        }

        [Test]
        public void SaveAndSelectByIntLessThenNoMatch()
        {
            SetUp();
            Dummy d = new Dummy();
            d.IntValue = 100;
            d.Save();

            Dummy d2 = Dummy.SelectFirst(Criteria.Lt("IntValue", 99));
            Assert.IsNull(d2);
        }

        [Test]
        public void SaveAndSelectByIntMoreThen()
        {
            SetUp();
            Dummy d = new Dummy();
            d.IntValue = 100;
            d.Save();

            Dummy d2 = Dummy.SelectFirst(Criteria.Mt("IntValue", 99));
            Assert.IsNotNull(d2);
        }

        [Test]
        public void SaveAndSelectByIntMoreThenNoMatch()
        {
            SetUp();
            Dummy d = new Dummy();
            d.IntValue = 100;
            d.Save();

            Dummy d2 = Dummy.SelectFirst(Criteria.Mt("IntValue", 101));
            Assert.IsNull(d2);
        }

        [Test]
        public void SaveAndSelectByDateEquals()
        {
            SetUp();
            Dummy d = new Dummy();
            d.DateValue1 = date1;
            d.Save();

            Dummy d2 = Dummy.SelectFirst(Criteria.Eq("DateValue1", date1));
            Assert.IsNotNull(d2);
        }

        [Test]
        public void SaveAndSelectByDateEqualsNoMatch()
        {
            SetUp();
            Dummy d = new Dummy();
            d.DateValue1 = date1;
            d.Save();

            Dummy d2 = Dummy.SelectFirst(Criteria.Eq("DateValue1", date1.AddSeconds(1)));
            Assert.IsNull(d2);
        }

        [Test]
        public void SaveAndSelectByDateLessThen()
        {
            SetUp();
            Dummy d = new Dummy();
            d.DateValue1 = date1;
            d.Save();

            Dummy d2 = Dummy.SelectFirst(Criteria.Lt("DateValue1", date1.AddSeconds(1)));
            Assert.IsNotNull(d2);
        }

        [Test]
        public void SaveAndSelectByDateLessThenNoMatch()
        {
            SetUp();
            Dummy d = new Dummy();
            d.DateValue1 = date1;
            d.Save();

            Dummy d2 = Dummy.SelectFirst(Criteria.Lt("DateValue1", date1));
            Assert.IsNull(d2);
        }

        [Test]
        public void SaveAndSelectByDateMoreThen()
        {
            SetUp();
            Dummy d = new Dummy();
            d.DateValue1 = date1;
            d.Save();

            Dummy d2 = Dummy.SelectFirst(Criteria.Mt("DateValue1", date1.AddSeconds(-1)));
            Assert.IsNotNull(d2);
        }

        [Test]
        public void SaveAndSelectByDateMoreThenNoMatch()
        {
            SetUp();
            Dummy d = new Dummy();
            d.DateValue1 = date1;
            d.Save();

            Dummy d2 = Dummy.SelectFirst(Criteria.Mt("DateValue1", date1));
            Assert.IsNull(d2);
        }

        [Test]
        public void SaveAndSelectByDateHugeDifference()
        {
            SetUp();
            Dummy d = new Dummy();
            d.DateValue1 = date1;
            d.Save();

            Dummy d2 = Dummy.SelectFirst(Criteria.Mt("DateValue1", date1.AddYears(-15)));
            Assert.IsNotNull(d2);
        }

        [Test]
        public void SaveAndSelectByDateHugeDifferenceNoMatch()
        {
            SetUp();
            Dummy d = new Dummy();
            d.DateValue1 = date1;
            d.Save();

            Dummy d2 = Dummy.SelectFirst(Criteria.Mt("DateValue1", date1.AddYears(15)));
            Assert.IsNull(d2);
        }

        [Test]
        public void SaveAndSelectByTwoDates()
        {
            SetUp();
            Dummy d = new Dummy();
            d.DateValue1 = date1;
            d.DateValue2 = date2;
            d.Save();

            Dummy d2 = Dummy.SelectFirst(
                Criteria.Eq("DateValue1", date1),
                Criteria.Eq("DateValue2", date2));
            Assert.IsNotNull(d2);
        }

        [Test]
        public void SaveAndSelectByTwoDatesOneWrongNoMatch()
        {
            SetUp();
            Dummy d = new Dummy();
            d.DateValue1 = date1;
            d.DateValue2 = date2;
            d.Save();

            Dummy d2 = Dummy.SelectFirst(
                Criteria.Eq("DateValue1", date1),
                Criteria.Eq("DateValue2", date2.AddSeconds(1)));
            Assert.IsNull(d2);
        }

        [Test]
        public void SaveAndSelectByTwoDatesOneLessOneMore()
        {
            SetUp();
            Dummy d = new Dummy();
            d.DateValue1 = date1;
            d.DateValue2 = date2;
            d.Save();

            Dummy d2 = Dummy.SelectFirst(
                Criteria.Lt("DateValue1", date1.AddHours(1)),
                Criteria.Mt("DateValue2", date2.AddHours(-1)));
            Assert.IsNotNull(d2);
        }

        [Test]
        public void SaveAndSelectThreeByDateTwoMatches()
        {
            SetUp();
            Dummy d = new Dummy();
            d.DateValue1 = date1;
            d.DateValue2 = date2;
            d.Save();

            Dummy d2 = new Dummy();
            d2.DateValue1 = date1.AddYears(1);
            d2.DateValue2 = date2.AddYears(2);
            d2.Save();

            Dummy d3 = new Dummy();
            d3.DateValue1 = date1.AddYears(3);
            d3.DateValue2 = date2.AddYears(16);
            d3.Save();

            List<Dummy> ls = new List<Dummy>(
                Dummy.Select(
                    Criteria.Mt("DateValue1", date1.AddMonths(-1)),
                    Criteria.Lt("DateValue2", date2.AddYears(10))));
            Assert.IsNotNull(ls);
            Assert.AreEqual(2, ls.Count);
            Assert.AreEqual(ls[0].DateValue1, date1);
            Assert.AreEqual(ls[1].DateValue2, date2.AddYears(2));
        }
    }
}




















