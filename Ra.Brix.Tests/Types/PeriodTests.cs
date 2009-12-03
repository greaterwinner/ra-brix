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
    public class PeriodTests
    {
        [Test]
        [ExpectedException]
        public void SameDatesThrows()
        {
            DateTime now = DateTime.Now;
            Period left = new Period(now, now);
        }

        [Test]
        [ExpectedException]
        public void NegativeLengthThrows()
        {
            DateTime now = DateTime.Now;
            Period left = new Period(now, now.AddMilliseconds(-1));
        }

        [Test]
        public void TinyPeriod()
        {
            DateTime now = DateTime.Now;
            Period left = new Period(now, now.AddMilliseconds(1));
        }

        [Test]
        public void OverlapFirstLess()
        {
            Period left = new Period(DateTime.Now, DateTime.Now.AddHours(3));
            Period right = new Period(DateTime.Now.AddHours(1), DateTime.Now.AddHours(4));
            Assert.IsTrue(Period.Intersects(left, right));
        }

        [Test]
        public void OverlapFirstMore()
        {
            Period left = new Period(DateTime.Now.AddHours(1), DateTime.Now.AddHours(4));
            Period right = new Period(DateTime.Now, DateTime.Now.AddHours(3));
            Assert.IsTrue(Period.Intersects(left, right));
        }

        [Test]
        public void OverlapFirstContains()
        {
            Period left = new Period(DateTime.Now.AddHours(0), DateTime.Now.AddHours(4));
            Period right = new Period(DateTime.Now.AddHours(1), DateTime.Now.AddHours(3));
            Assert.IsTrue(Period.Intersects(left, right));
        }

        [Test]
        public void OverlapSecondContains()
        {
            Period left = new Period(DateTime.Now.AddHours(1), DateTime.Now.AddHours(3));
            Period right = new Period(DateTime.Now.AddHours(0), DateTime.Now.AddHours(4));
            Assert.IsTrue(Period.Intersects(left, right));
        }

        [Test]
        public void NotOverlapFirstLess()
        {
            Period left = new Period(DateTime.Now.AddHours(0), DateTime.Now.AddHours(2));
            Period right = new Period(DateTime.Now.AddHours(3), DateTime.Now.AddHours(4));
            Assert.IsFalse(Period.Intersects(left, right));
        }

        [Test]
        public void NotOverlapFirstMore()
        {
            Period left = new Period(DateTime.Now.AddHours(3), DateTime.Now.AddHours(4));
            Period right = new Period(DateTime.Now.AddHours(0), DateTime.Now.AddHours(2));
            Assert.IsFalse(Period.Intersects(left, right));
        }

        [Test]
        public void NotOverlapFirstContinues()
        {
            Period left = new Period(DateTime.Now.AddHours(0), DateTime.Now.AddHours(4));
            Period right = new Period(left.End, left.End.AddHours(2));
            Assert.IsFalse(Period.Intersects(left, right));
        }

        [Test]
        public void NotOverlapSecondContinues()
        {
            Period left = new Period(DateTime.Now.AddHours(0), DateTime.Now.AddHours(4));
            Period right = new Period(left.End, left.End.AddHours(2));
            Assert.IsFalse(Period.Intersects(right, left));
        }

        [Test]
        public void ContinuesFirst()
        {
            Period left = new Period(DateTime.Now.AddHours(0), DateTime.Now.AddHours(4));
            Period right = new Period(left.End, left.End.AddHours(2));
            Assert.IsTrue(Period.Continues(left, right));
        }

        [Test]
        public void ContinuesSecond()
        {
            Period left = new Period(DateTime.Now.AddHours(0), DateTime.Now.AddHours(4));
            Period right = new Period(left.End, left.End.AddHours(2));
            Assert.IsTrue(Period.Continues(right, left));
        }

        [Test]
        public void ORBothEqual()
        {
            Period left = new Period(DateTime.Now.AddHours(0), DateTime.Now.AddHours(4));
            Period right = new Period(left.Start, left.End);
            Period or = Period.OR(left, right);
            Assert.IsTrue(Period.Equals(or, left));
        }

        [Test]
        public void ORFirstLessSameEnd()
        {
            Period left = new Period(DateTime.Now.AddHours(0), DateTime.Now.AddHours(4));
            Period right = new Period(DateTime.Now.AddHours(1), DateTime.Now.AddHours(4));
            Period or = Period.OR(left, right);
            Assert.IsTrue(Period.Equals(or, left));
        }

        [Test]
        public void ORSecondLessSameEnd()
        {
            Period left = new Period(DateTime.Now.AddHours(1), DateTime.Now.AddHours(4));
            Period right = new Period(DateTime.Now.AddHours(0), DateTime.Now.AddHours(4));
            Period or = Period.OR(left, right);
            Assert.IsTrue(Period.Equals(or, right));
        }

        [Test]
        public void ORFirstLessFirstEndLess()
        {
            Period left = new Period(DateTime.Now.AddHours(0), DateTime.Now.AddHours(3));
            Period right = new Period(DateTime.Now.AddHours(1), DateTime.Now.AddHours(4));
            Period or = Period.OR(left, right);
            Assert.IsTrue(Period.Equals(or, new Period(left.Start, right.End)));
        }

        [Test]
        public void ORFirstSuperset()
        {
            Period left = new Period(DateTime.Now.AddHours(0), DateTime.Now.AddHours(4));
            Period right = new Period(DateTime.Now.AddHours(1), DateTime.Now.AddHours(2));
            Period or = Period.OR(left, right);
            Assert.IsTrue(Period.Equals(or, new Period(left.Start, left.End)));
        }

        [Test]
        public void ORFirstSubset()
        {
            Period left = new Period(DateTime.Now.AddHours(1), DateTime.Now.AddHours(3));
            Period right = new Period(DateTime.Now.AddHours(0), DateTime.Now.AddHours(4));
            Period or = Period.OR(left, right);
            Assert.IsTrue(Period.Equals(or, new Period(right.Start, right.End)));
        }

        [Test]
        [ExpectedException]
        public void ORNotIntersectingThrows()
        {
            Period left = new Period(DateTime.Now.AddHours(1), DateTime.Now.AddHours(3));
            Period right = new Period(DateTime.Now.AddHours(4), DateTime.Now.AddHours(5));
            Period or = Period.OR(left, right);
        }

        [Test]
        public void ANDFirstLessSameEnd()
        {
            Period left = new Period(DateTime.Now.AddHours(0), DateTime.Now.AddHours(2));
            Period right = new Period(DateTime.Now.AddHours(1), DateTime.Now.AddHours(2));
            Period and = Period.AND(left, right);
            Assert.IsTrue(Period.Equals(and, new Period(right.Start, right.End)));
        }

        [Test]
        public void ANDFirstMoreSameEnd()
        {
            Period left = new Period(DateTime.Now.AddHours(1), DateTime.Now.AddHours(2));
            Period right = new Period(DateTime.Now.AddHours(0), DateTime.Now.AddHours(2));
            Period and = Period.AND(left, right);
            Assert.IsTrue(Period.Equals(and, new Period(left.Start, right.End)));
        }

        [Test]
        public void ANDFirstMoreSecondEndMore()
        {
            Period left = new Period(DateTime.Now.AddHours(1), DateTime.Now.AddHours(2));
            Period right = new Period(DateTime.Now.AddHours(0), DateTime.Now.AddHours(3));
            Period and = Period.AND(left, right);
            Assert.IsTrue(Period.Equals(and, new Period(left.Start, left.End)));
        }

        [Test]
        public void ANDFirstMoreSecondEndLess()
        {
            Period left = new Period(DateTime.Now.AddHours(1), DateTime.Now.AddHours(3));
            Period right = new Period(DateTime.Now.AddHours(0), DateTime.Now.AddHours(2));
            Period and = Period.AND(left, right);
            Assert.IsTrue(Period.Equals(and, new Period(left.Start, right.End)));
        }

        [Test]
        public void ANDFirstLessSecondEndLess()
        {
            Period left = new Period(DateTime.Now.AddHours(0), DateTime.Now.AddHours(3));
            Period right = new Period(DateTime.Now.AddHours(1), DateTime.Now.AddHours(2));
            Period and = Period.AND(left, right);
            Assert.IsTrue(Period.Equals(and, new Period(right.Start, right.End)));
        }

        [Test]
        public void ANDFirstLessFirstEndLess()
        {
            Period left = new Period(DateTime.Now.AddHours(0), DateTime.Now.AddHours(2));
            Period right = new Period(DateTime.Now.AddHours(1), DateTime.Now.AddHours(3));
            Period and = Period.AND(left, right);
            Assert.IsTrue(Period.Equals(and, new Period(right.Start, left.End)));
        }

        [Test]
        [ExpectedException]
        public void ANDNotIntersectingThrows()
        {
            Period left = new Period(DateTime.Now.AddHours(0), DateTime.Now.AddHours(2));
            Period right = new Period(DateTime.Now.AddHours(3), DateTime.Now.AddHours(5));
            Period and = Period.AND(left, right);
        }

        [Test]
        public void XORNotIntersecting()
        {
            Period left = new Period(DateTime.Now.AddHours(0), DateTime.Now.AddHours(2));
            Period right = new Period(DateTime.Now.AddHours(3), DateTime.Now.AddHours(5));
            Tuple<Period, Period> xor = Period.XOR(left, right);
            Assert.IsTrue(Period.Equals(xor.Left, left));
            Assert.IsTrue(Period.Equals(xor.Right, right));
        }

        [Test]
        public void XORNotIntersectingSecondLess()
        {
            Period left = new Period(DateTime.Now.AddHours(4), DateTime.Now.AddHours(6));
            Period right = new Period(DateTime.Now.AddHours(1), DateTime.Now.AddHours(3));
            Tuple<Period, Period> xor = Period.XOR(left, right);
            Assert.IsTrue(Period.Equals(xor.Left, left));
            Assert.IsTrue(Period.Equals(xor.Right, right));
        }

        [Test]
        public void XORIntersectingFirstLess()
        {
            Period left = new Period(DateTime.Now.AddHours(1), DateTime.Now.AddHours(3));
            Period right = new Period(DateTime.Now.AddHours(2), DateTime.Now.AddHours(5));
            Tuple<Period, Period> xor = Period.XOR(left, right);
            Assert.IsTrue(Period.Equals(xor.Left, new Period(left.Start, right.Start)));
            Assert.IsTrue(Period.Equals(xor.Right, new Period(left.End, right.End)));
        }

        [Test]
        public void XORIntersectingFirstMore()
        {
            Period left = new Period(DateTime.Now.AddHours(2), DateTime.Now.AddHours(5));
            Period right = new Period(DateTime.Now.AddHours(1), DateTime.Now.AddHours(3));
            Tuple<Period, Period> xor = Period.XOR(left, right);
            Assert.IsTrue(Period.Equals(xor.Left, new Period(right.Start, left.Start)));
            Assert.IsTrue(Period.Equals(xor.Right, new Period(right.End, left.End)));
        }

        [Test]
        public void XORIntersectingFirstSubset()
        {
            Period left = new Period(DateTime.Now.AddHours(2), DateTime.Now.AddHours(3));
            Period right = new Period(DateTime.Now.AddHours(1), DateTime.Now.AddHours(5));
            Tuple<Period, Period> xor = Period.XOR(left, right);
            Assert.IsTrue(Period.Equals(xor.Left, new Period(right.Start, left.Start)));
            Assert.IsTrue(Period.Equals(xor.Right, new Period(left.End, right.End)));
        }

        [Test]
        public void XORIntersectingFirstSuperset()
        {
            Period left = new Period(DateTime.Now.AddHours(1), DateTime.Now.AddHours(5));
            Period right = new Period(DateTime.Now.AddHours(2), DateTime.Now.AddHours(3));
            Tuple<Period, Period> xor = Period.XOR(left, right);
            Assert.IsTrue(Period.Equals(xor.Left, new Period(left.Start, right.Start)));
            Assert.IsTrue(Period.Equals(xor.Right, new Period(right.End, left.End)));
        }

        [Test]
        [ExpectedException]
        public void XOREqualsThrows()
        {
            Period left = new Period(DateTime.Now.AddHours(1), DateTime.Now.AddHours(5));
            Period right = new Period(DateTime.Now.AddHours(1), DateTime.Now.AddHours(5));
            Tuple<Period, Period> xor = Period.XOR(left, right);
        }

        [Test]
        [ExpectedException]
        public void XORSameStartThrows()
        {
            Period left = new Period(DateTime.Now.AddHours(1), DateTime.Now.AddHours(5));
            Period right = new Period(DateTime.Now.AddHours(1), DateTime.Now.AddHours(3));
            Tuple<Period, Period> xor = Period.XOR(left, right);
        }

        [Test]
        [ExpectedException]
        public void XORSameEndThrows()
        {
            Period left = new Period(DateTime.Now.AddHours(2), DateTime.Now.AddHours(3));
            Period right = new Period(DateTime.Now.AddHours(1), DateTime.Now.AddHours(3));
            Tuple<Period, Period> xor = Period.XOR(left, right);
        }
    }
}



















