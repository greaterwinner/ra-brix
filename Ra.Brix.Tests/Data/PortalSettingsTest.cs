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
using SettingsRecords;

namespace Ra.Brix.Tests.Data
{
    [TestFixture]
    public class PortalSettingsTest : BaseTest
    {
        [Test]
        public void VerifySaveWorks()
        {
            Settings.Reload();
            SetUp();
            Settings.Instance["username"] = "thomas";

            Assert.AreEqual("thomas", Settings.Instance["username"]);
        }

        [Test]
        public void VerifyEnumerationWorks()
        {
            Settings.Reload();
            SetUp();
            Settings.Instance["username1"] = "thomas";
            Settings.Instance["username2"] = "kariem";

            Assert.AreEqual("thomas", Settings.Instance["username1"]);
            Assert.AreEqual("kariem", Settings.Instance["username2"]);

            Assert.AreEqual(2, Settings.Instance.Count);
            int idxNo = 0;
            foreach (Settings.Setting idx in Settings.Instance)
            {
                if (idxNo == 0)
                {
                    Assert.AreEqual("username1", idx.Name);
                    Assert.AreEqual("thomas", idx.Value);
                }
                else
                {
                    Assert.AreEqual("username2", idx.Name);
                    Assert.AreEqual("kariem", idx.Value);
                }
                idxNo += 1;
            }
        }

        [Test]
        public void VerifyRemoveWorks()
        {
            Settings.Reload();
            SetUp();
            Settings.Instance["username1"] = "thomas";
            Settings.Instance["username2"] = "kariem";

            Assert.AreEqual("thomas", Settings.Instance["username1"]);
            Assert.AreEqual("kariem", Settings.Instance["username2"]);

            Settings.Instance.Remove(Settings.Instance[0]);
            Assert.AreEqual("kariem", Settings.Instance[0].Value);
            Assert.AreEqual("username2", Settings.Instance[0].Name);
        }

        [Test]
        public void VerifyRemoveSecondWorks()
        {
            Settings.Reload();
            SetUp();
            Settings.Instance["username1"] = "thomas";
            Settings.Instance["username2"] = "kariem";

            Assert.AreEqual("thomas", Settings.Instance["username1"]);
            Assert.AreEqual("kariem", Settings.Instance["username2"]);

            Settings.Instance.Remove(Settings.Instance[1]);
            Assert.AreEqual("thomas", Settings.Instance[0].Value);
            Assert.AreEqual("username1", Settings.Instance[0].Name);
        }
    }
}
