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
using NUnit.Framework;
using Ra.Brix.Types;

namespace Ra.Brix.Tests.Types
{
    [TestFixture]
    public class PeriodCollectionTests
    {
        [Test]
        public void InsertionWorks()
        {
            PeriodCollection col = new PeriodCollection();
            DateTime now = DateTime.Now;
            col.Add(new Period(now, now.AddHours(1)));
            col.Add(new Period(now.AddHours(2), now.AddHours(3)));
            Assert.AreEqual(2, col.Count);
            Assert.AreEqual(new Period(now, now.AddHours(1)), col[0]);
            Assert.AreEqual(new Period(now.AddHours(2), now.AddHours(3)), col[1]);
        }

        [Test]
        public void SortWorks()
        {
            PeriodCollection col = new PeriodCollection();
            DateTime now = DateTime.Now;
            col.Add(new Period(now.AddHours(2), now.AddHours(3)));
            col.Add(new Period(now, now.AddHours(1)));
            col.Sort();
            Assert.AreEqual(2, col.Count);
            Assert.AreEqual(new Period(now, now.AddHours(1)), col[0]);
            Assert.AreEqual(new Period(now.AddHours(2), now.AddHours(3)), col[1]);
        }

        [Test]
        public void NormalizationWorks()
        {
            DateTime now = DateTime.Now;

            PeriodCollection col = new PeriodCollection();

            col.Add(new Period(now.AddHours(2), now.AddHours(5)));
            col.Add(new Period(now, now.AddHours(1)));
            col.Add(new Period(now.AddHours(4), now.AddHours(7)));

            col.Normalize();

            Assert.AreEqual(2, col.Count);
            Assert.AreEqual(new Period(now, now.AddHours(1)), col[0]);
            Assert.AreEqual(new Period(now.AddHours(2), now.AddHours(7)), col[1]);
        }

        [Test]
        public void AdvancedNormalizationWorks1()
        {
            DateTime now = DateTime.Now;

            PeriodCollection col = new PeriodCollection();

            col.Add(new Period(now.AddHours(31), now.AddHours(34)));
            col.Add(new Period(now.AddHours(16), now.AddHours(18)));
            col.Add(new Period(now.AddHours(2), now.AddHours(5)));
            col.Add(new Period(now.AddHours(27), now.AddHours(28)));
            col.Add(new Period(now, now.AddHours(1)));
            col.Add(new Period(now.AddHours(4), now.AddHours(7)));
            col.Add(new Period(now.AddHours(3), now.AddHours(4)));
            col.Add(new Period(now.AddHours(9), now.AddHours(11)));
            col.Add(new Period(now.AddHours(24), now.AddHours(26)));
            col.Add(new Period(now.AddHours(2), now.AddHours(5)));
            col.Add(new Period(now.AddHours(11), now.AddHours(13)));
            col.Add(new Period(now.AddHours(25), now.AddHours(32)));

            col.Normalize();

            Assert.AreEqual(5, col.Count);
            Assert.AreEqual(new Period(now, now.AddHours(1)), col[0]);
            Assert.AreEqual(new Period(now.AddHours(2), now.AddHours(7)), col[1]);
            Assert.AreEqual(new Period(now.AddHours(9), now.AddHours(13)), col[2]);
            Assert.AreEqual(new Period(now.AddHours(16), now.AddHours(18)), col[3]);
            Assert.AreEqual(new Period(now.AddHours(24), now.AddHours(34)), col[4]);
        }

        [Test]
        public void ANDSimple()
        {
            DateTime now = DateTime.Now;

            PeriodCollection left = new PeriodCollection();
            left.Add(new Period(now.AddHours(1), now.AddHours(3)));
            left.Add(new Period(now.AddHours(4), now.AddHours(7)));

            PeriodCollection right = new PeriodCollection();
            right.Add(new Period(now.AddHours(2), now.AddHours(3)));
            right.Add(new Period(now.AddHours(5), now.AddHours(8)));

            PeriodCollection res = PeriodCollection.AND(left, right);
            Assert.AreEqual(2, res.Count);
            Assert.AreEqual(new Period(now.AddHours(2), now.AddHours(3)), res[0]);
            Assert.AreEqual(new Period(now.AddHours(5), now.AddHours(7)), res[1]);
        }

        [Test]
        public void ANDTwoSimilarLists()
        {
            DateTime now = DateTime.Now;

            PeriodCollection left = new PeriodCollection();
            left.Add(new Period(now.AddHours(1), now.AddHours(3)));
            left.Add(new Period(now.AddHours(4), now.AddHours(7)));

            PeriodCollection right = new PeriodCollection();
            right.Add(new Period(now.AddHours(1), now.AddHours(3)));
            right.Add(new Period(now.AddHours(4), now.AddHours(7)));

            PeriodCollection res = PeriodCollection.AND(left, right);
            Assert.AreEqual(2, res.Count);
            Assert.AreEqual(new Period(now.AddHours(1), now.AddHours(3)), res[0]);
            Assert.AreEqual(new Period(now.AddHours(4), now.AddHours(7)), res[1]);
        }

        [Test]
        public void ANDTwoNonOverlappingLists()
        {
            DateTime now = DateTime.Now;

            PeriodCollection left = new PeriodCollection();
            left.Add(new Period(now.AddHours(1), now.AddHours(3)));
            left.Add(new Period(now.AddHours(4), now.AddHours(7)));

            PeriodCollection right = new PeriodCollection();
            right.Add(new Period(now.AddHours(9), now.AddHours(11)));
            right.Add(new Period(now.AddHours(12), now.AddHours(13)));

            PeriodCollection res = PeriodCollection.AND(left, right);
            Assert.AreEqual(0, res.Count);
        }

        [Test]
        public void ANDTwoLongLists()
        {
            DateTime now = DateTime.Now.Date;

            PeriodCollection left = new PeriodCollection();
            left.Add(new Period(now.AddHours(1), now.AddHours(3)));
            left.Add(new Period(now.AddHours(4), now.AddHours(7)));
            left.Add(new Period(now.AddHours(1), now.AddHours(3)));
            left.Add(new Period(now.AddHours(5), now.AddHours(6)));
            left.Add(new Period(now.AddHours(7), now.AddHours(8)));
            left.Add(new Period(now.AddHours(11), now.AddHours(13)));
            left.Add(new Period(now.AddHours(11), now.AddHours(12)));
            left.Add(new Period(now.AddHours(15), now.AddHours(16)));
            left.Add(new Period(now.AddHours(16), now.AddHours(17)));
            left.Add(new Period(now.AddHours(17), now.AddHours(20)));

            PeriodCollection right = new PeriodCollection();
            right.Add(new Period(now.AddHours(1), now.AddHours(3)));
            right.Add(new Period(now.AddHours(5), now.AddHours(6)));
            right.Add(new Period(now.AddHours(7), now.AddHours(10)));
            right.Add(new Period(now.AddHours(12), now.AddHours(15)));
            right.Add(new Period(now.AddHours(17), now.AddHours(25)));

            PeriodCollection res = PeriodCollection.AND(left, right);
            Assert.AreEqual(5, res.Count);
            Assert.AreEqual(new Period(now.AddHours(1), now.AddHours(3)), res[0]);
            Assert.AreEqual(new Period(now.AddHours(5), now.AddHours(6)), res[1]);
            Assert.AreEqual(new Period(now.AddHours(7), now.AddHours(8)), res[2]);
            Assert.AreEqual(new Period(now.AddHours(12), now.AddHours(13)), res[3]);
            Assert.AreEqual(new Period(now.AddHours(17), now.AddHours(20)), res[4]);
        }

        [Test]
        public void ANDOneLongListAndOneBigSpan()
        {
            DateTime now = DateTime.Now.Date;

            PeriodCollection left = new PeriodCollection();
            left.Add(new Period(now.AddHours(1), now.AddHours(3)));
            left.Add(new Period(now.AddHours(4), now.AddHours(7)));
            left.Add(new Period(now.AddHours(1), now.AddHours(3)));
            left.Add(new Period(now.AddHours(5), now.AddHours(6)));
            left.Add(new Period(now.AddHours(7), now.AddHours(8)));
            left.Add(new Period(now.AddHours(11), now.AddHours(13)));
            left.Add(new Period(now.AddHours(11), now.AddHours(12)));
            left.Add(new Period(now.AddHours(15), now.AddHours(16)));
            left.Add(new Period(now.AddHours(16), now.AddHours(17)));
            left.Add(new Period(now.AddHours(17), now.AddHours(20)));
            PeriodCollection expected = new PeriodCollection(left);
            expected.Normalize();

            PeriodCollection right = new PeriodCollection();
            right.Add(new Period(now.AddHours(1), now.AddHours(30)));

            PeriodCollection res = PeriodCollection.AND(left, right);
            Assert.AreEqual(expected.Count, res.Count);
            for (int idx = 0; idx < expected.Count; idx++)
            {
                Assert.AreEqual(expected[idx], res[idx]);
            }
        }

        [Test]
        public void ANDOneBigSpanAndLargeList()
        {
            DateTime now = DateTime.Now.Date;

            PeriodCollection left = new PeriodCollection();
            left.Add(new Period(now.AddHours(22), now.AddHours(27)));
            left.Add(new Period(now.AddHours(1), now.AddHours(3)));
            left.Add(new Period(now.AddHours(4), now.AddHours(7)));
            left.Add(new Period(now.AddHours(11), now.AddHours(13)));
            left.Add(new Period(now.AddHours(11), now.AddHours(12)));
            left.Add(new Period(now.AddHours(17), now.AddHours(20)));
            left.Add(new Period(now.AddHours(21), now.AddHours(23)));
            left.Add(new Period(now.AddHours(22), now.AddHours(23)));
            left.Add(new Period(now.AddHours(15), now.AddHours(16)));
            left.Add(new Period(now.AddHours(16), now.AddHours(17)));
            left.Add(new Period(now.AddHours(1), now.AddHours(3)));
            left.Add(new Period(now.AddHours(5), now.AddHours(6)));
            left.Add(new Period(now.AddHours(7), now.AddHours(8)));
            left.Add(new Period(now.AddHours(16), now.AddHours(17)));
            PeriodCollection expected = new PeriodCollection(left);
            expected.Normalize();

            PeriodCollection right = new PeriodCollection();
            right.Add(new Period(now.AddHours(1), now.AddHours(30)));

            PeriodCollection res = PeriodCollection.AND(right, left);
            Assert.AreEqual(expected.Count, res.Count);
            for (int idx = 0; idx < expected.Count; idx++)
            {
                Assert.AreEqual(expected[idx], res[idx]);
            }
        }

        [Test]
        public void NOTTest()
        {
            DateTime now = DateTime.Now.Date;

            PeriodCollection left = new PeriodCollection();
            left.Add(new Period(now.AddHours(22), now.AddHours(27)));
            left.Add(new Period(now.AddHours(1), now.AddHours(3)));
            left.Add(new Period(now.AddHours(4), now.AddHours(7)));
            left.Add(new Period(now.AddHours(11), now.AddHours(13)));
            left.Add(new Period(now.AddHours(11), now.AddHours(12)));
            left.Add(new Period(now.AddHours(17), now.AddHours(20)));
            left.Add(new Period(now.AddHours(21), now.AddHours(23)));
            left.Add(new Period(now.AddHours(22), now.AddHours(23)));
            left.Add(new Period(now.AddHours(15), now.AddHours(16)));
            left.Add(new Period(now.AddHours(16), now.AddHours(17)));
            left.Add(new Period(now.AddHours(1), now.AddHours(3)));
            left.Add(new Period(now.AddHours(5), now.AddHours(6)));
            left.Add(new Period(now.AddHours(7), now.AddHours(8)));
            left.Add(new Period(now.AddHours(16), now.AddHours(17)));
            PeriodCollection expected = new PeriodCollection(left);
            expected.Normalize();

            PeriodCollection res = left.NOT();

            Assert.AreEqual(expected.Count + 1, res.Count);

            PeriodCollection res2 = res.NOT();
            Assert.AreEqual(expected.Count, res2.Count);
            for (int idx = 0; idx < res2.Count; idx++)
            {
                Assert.AreEqual(expected[idx], res2[idx]);
            }
        }

        [Test]
        public void XORTest()
        {
            DateTime now = DateTime.Now.Date;

            PeriodCollection left = new PeriodCollection();
            left.Add(new Period(now.AddHours(1), now.AddHours(5)));
            left.Add(new Period(now.AddHours(7), now.AddHours(9)));
            left.Add(new Period(now.AddHours(11), now.AddHours(20)));

            PeriodCollection right = new PeriodCollection();
            right.Add(new Period(now.AddHours(0), now.AddHours(2)));
            right.Add(new Period(now.AddHours(4), now.AddHours(6)));
            right.Add(new Period(now.AddHours(8), now.AddHours(9)));
            right.Add(new Period(now.AddHours(13), now.AddHours(14)));
            right.Add(new Period(now.AddHours(17), now.AddHours(20)));

            PeriodCollection XOR = PeriodCollection.XOR(left, right);
            Assert.AreEqual(6, XOR.Count);
            Assert.AreEqual(new Period(now.AddHours(0), now.AddHours(1)), XOR[0]);
            Assert.AreEqual(new Period(now.AddHours(2), now.AddHours(4)), XOR[1]);
            Assert.AreEqual(new Period(now.AddHours(5), now.AddHours(6)), XOR[2]);
            Assert.AreEqual(new Period(now.AddHours(7), now.AddHours(8)), XOR[3]);
            Assert.AreEqual(new Period(now.AddHours(11), now.AddHours(13)), XOR[4]);
            Assert.AreEqual(new Period(now.AddHours(14), now.AddHours(17)), XOR[5]);
        }
    }
}



















