/*
 * Ra-Brix - A Modular-based Framework for building 
 * Web Applications Copyright 2009 - Thomas Hansen 
 * thomas@ra-ajax.org. Unless permission is 
 * explicitly given this code is licensed under the 
 * GNU Affero GPL version 3 which can be found in the 
 * license.txt file on disc.
 * 
 */

using LanguageRecords;
using NUnit.Framework;

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
