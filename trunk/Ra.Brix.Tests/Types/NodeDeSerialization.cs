/*
 * Ra-Brix - A Modular-based Framework for building 
 * Web Applications Copyright 2009 - Thomas Hansen 
 * thomas@ra-ajax.org. Unless permission is 
 * explicitly given this code is licensed under the 
 * GNU Affero GPL version 3 which can be found in the 
 * license.txt file on disc.
 * 
 */

using NUnit.Framework;
using Ra.Brix.Types;

namespace Ra.Brix.Tests.Types
{
    [TestFixture]
    public class NodeDeSerializationTest
    {
        [Test]
        public void SimpleDeSerializationTest()
        {
            Node orig = new Node("SomeNode");
            orig["One"].Value = "1";
            string str = orig.ToJSONString();

            Node deSer = Node.FromJSONString(str);
            Assert.AreEqual(orig.Name, deSer.Name);
            Assert.AreEqual(orig.Value, deSer.Value);
            Assert.AreEqual(1, deSer.Count);
            Assert.AreEqual(orig.Count, deSer.Count);
            Assert.AreEqual(orig["One"].Value, deSer["One"].Value);
        }

        [Test]
        public void SimpleDeSerializationTestNoChildren()
        {
            Node orig = new Node("SomeNode");
            string str = orig.ToJSONString();
            Node deSer = Node.FromJSONString(str);
            string after = deSer.ToJSONString();
            Assert.AreEqual(str, after);
        }

        [Test]
        public void SimpleDeSerializationTestOnlyChildren()
        {
            Node orig = new Node();
            orig["One"].Value = "1";
            string str = orig.ToJSONString();
            Node deSer = Node.FromJSONString(str);
            string after = deSer.ToJSONString();
            Assert.AreEqual(str, after);
        }

        [Test]
        public void SimpleDeSerializationTestMultipleChildren()
        {
            Node orig = new Node();
            orig["One"].Value = "1";
            orig["Two"].Value = "2";
            orig["Three"].Value = "3";
            string str = orig.ToJSONString();
            Node deSer = Node.FromJSONString(str);
            string after = deSer.ToJSONString();
            Assert.AreEqual(str, after);
        }

        [Test]
        public void SimpleDeSerializationTestRecursiveChildren()
        {
            Node orig = new Node();
            orig["One"].Value = "1";
            orig["One"]["Two"].Value = "2";
            orig["One"]["Two"]["Three"].Value = "3";
            string str = orig.ToJSONString();
            Node deSer = Node.FromJSONString(str);
            string after = deSer.ToJSONString();
            Assert.AreEqual(str, after);
        }
    }
}



















