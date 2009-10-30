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
    public class SelectByCriteriaParentTest : BaseTest
    {
        private DateTime date1 = new DateTime(1900, 1, 1, 23, 59, 37);
        private DateTime date2 = new DateTime(1954, 11, 24, 21, 37, 31);

        [ActiveRecord]
        public class Dummy : ActiveRecord<Dummy>
        {
            public Dummy()
            {
                Children = new LazyList<Dummy>();
            }

            [ActiveField]
            public string StringValue { get; set; }

            [ActiveField]
            public int IntValue { get; set; }

            [ActiveField]
            public LazyList<Dummy> Children { get; set; }
        }

        [Test]
        public void SaveAndSelectByParentID()
        {
            Dummy d = new Dummy();
            d.StringValue = "first";
            Dummy d2 = new Dummy();
            d2.StringValue = "howdy";
            d.Children.Add(d2);
            d.Save();

            Dummy res = Dummy.SelectFirst(Criteria.ParentId(d.ID));
            Assert.IsNotNull(res);
            Assert.AreEqual("howdy", res.StringValue);
        }

        [Test]
        public void SaveAndSelectMultipleByParentID()
        {
            Dummy d = new Dummy();
            d.StringValue = "first";
            Dummy d2 = new Dummy();
            d2.StringValue = "howdy";
            d.Children.Add(d2);
            Dummy d3 = new Dummy();
            d3.StringValue = "howdy2";
            d.Children.Add(d3);
            d.Save();

            List<Dummy> res = new List<Dummy>(Dummy.Select(Criteria.ParentId(d.ID)));
            Assert.AreEqual(2, res.Count);
            Assert.AreEqual("howdy", res[0].StringValue);
            Assert.AreEqual("howdy2", res[1].StringValue);
        }

        [Test]
        public void SaveAndSelectMultipleByParentIDAndStringLike()
        {
            Dummy d = new Dummy();
            d.StringValue = "first";
            Dummy d2 = new Dummy();
            d2.StringValue = "howdy";
            d.Children.Add(d2);
            Dummy d3 = new Dummy();
            d3.StringValue = "howdy2";
            d.Children.Add(d3);
            d.Save();

            List<Dummy> res = new List<Dummy>(
                Dummy.Select(
                    Criteria.ParentId(d.ID),
                    Criteria.Like("StringValue", "howdy%")));
            Assert.AreEqual(2, res.Count);
            Assert.AreEqual("howdy", res[0].StringValue);
            Assert.AreEqual("howdy2", res[1].StringValue);
        }

        [Test]
        public void SaveAndSelectMultipleByParentIDAndStringLikeAndIntEquals()
        {
            Dummy d = new Dummy();
            d.StringValue = "first";
            d.IntValue = 5;
            Dummy d2 = new Dummy();
            d2.StringValue = "howdy";
            d2.IntValue = 5;
            d.Children.Add(d2);
            Dummy d3 = new Dummy();
            d3.StringValue = "howdy2";
            d3.IntValue = 5;
            d.Children.Add(d3);
            d.Save();

            List<Dummy> res = new List<Dummy>(
                Dummy.Select(
                    Criteria.ParentId(d.ID),
                    Criteria.Like("StringValue", "howdy%"),
                    Criteria.Eq("IntValue", 5)));
            Assert.AreEqual(2, res.Count);
            Assert.AreEqual("howdy", res[0].StringValue);
            Assert.AreEqual("howdy2", res[1].StringValue);
        }

        [Test]
        public void SaveAndSelectMultipleByParentIDAndStringLikeAndIntEqualsOneMiss()
        {
            Dummy d = new Dummy();
            d.StringValue = "first";
            d.IntValue = 6;
            Dummy d2 = new Dummy();
            d2.StringValue = "howdy";
            d2.IntValue = 6;
            d.Children.Add(d2);
            Dummy d3 = new Dummy();
            d3.StringValue = "howdy2";
            d3.IntValue = 5;
            d.Children.Add(d3);
            d.Save();

            List<Dummy> res = new List<Dummy>(
                Dummy.Select(
                    Criteria.ParentId(d.ID),
                    Criteria.Like("StringValue", "howdy%"),
                    Criteria.Eq("IntValue", 6)));
            Assert.AreEqual(1, res.Count);
            Assert.AreEqual("howdy", res[0].StringValue);
        }
    }
}




















