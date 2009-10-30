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

namespace Ra.Brix.Tests.Types
{
    [TestFixture]
    public class NodeTest
    {
        [Test]
        public void SimpleContruction()
        {
            Node n = new Node("Customers");
            n["Customer1"].Value = "id1";
            n["Customer1"]["Name"].Value = "Thomas Hansen";
            n["Customer1"]["Name"]["First"].Value = "Thomas";
            n["Customer1"]["Name"]["Last"].Value = "Hansen";
            n["Customer2"].Value = "id2";
            n["Customer2"]["Name"].Value = "Kariem Ali";
            n["Customer2"]["Name"]["First"].Value = "Kariem";
            n["Customer2"]["Name"]["Last"].Value = "Ali";

            Assert.AreEqual("Customers", n.Name);
            Assert.AreEqual(null, n.Value);
            Assert.AreEqual("", n.DNA);

            Assert.AreEqual("id1", n["Customer1"].Value);
            Assert.AreEqual("id2", n["Customer2"].Value);

            Assert.AreEqual("000", n["Customer1"].DNA);
            Assert.AreEqual("001", n["Customer2"].DNA);

            Assert.AreEqual("Thomas Hansen", n["Customer1"]["Name"].Value);
            Assert.AreEqual("Kariem Ali", n["Customer2"]["Name"].Value);

            Assert.AreEqual("000-000", n["Customer1"]["Name"].DNA);
            Assert.AreEqual("001-000", n["Customer2"]["Name"].DNA);

            Assert.AreEqual("Thomas", n["Customer1"]["Name"]["First"].Value);
            Assert.AreEqual("Kariem", n["Customer2"]["Name"]["First"].Value);

            Assert.AreEqual("Hansen", n["Customer1"]["Name"]["Last"].Value);
            Assert.AreEqual("Ali", n["Customer2"]["Name"]["Last"].Value);

            Assert.AreEqual("000-000-000", n["Customer1"]["Name"]["First"].DNA);
            Assert.AreEqual("001-000-000", n["Customer2"]["Name"]["First"].DNA);

            Assert.AreEqual("000-000-001", n["Customer1"]["Name"]["Last"].DNA);
            Assert.AreEqual("001-000-001", n["Customer2"]["Name"]["Last"].DNA);

            // Verifying that inserted node now gets correct DNA
            Assert.AreEqual("001-000-002", n["Customer2"]["Name"]["LastINTENTIONAL_SPELLING_ERROR"].DNA);

            // Verifying we can reach our node from a given dna...
            Node kariemLastName = n.Find("001-000-001");
            Assert.AreEqual("Ali", kariemLastName.Value);
        }
    }
}



















