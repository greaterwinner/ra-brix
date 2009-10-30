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
using LanguageRecords;
using NUnit.Core;
using NUnit.Framework;
using Ra.Brix.Data;
using System.Collections.Generic;
using Ra.Brix.Types;

namespace Ra.Brix.Tests.Data
{
    [TestFixture]
    public class LanguageTest : BaseTest
    {
        [Test]
        public void TestRetrieveDefault()
        {
            SetUp();
            Language.Reset();
            Assert.AreEqual("howdy", Language.Instance["howdy"]);
        }

        [Test]
        public void TestSaveAndRetrieve()
        {
            SetUp();
            Language.Reset();
            Language.Instance.ChangeValue("name", "en", "thomas");
            Assert.AreEqual("thomas", Language.Instance["name"]);
        }
    }
}
