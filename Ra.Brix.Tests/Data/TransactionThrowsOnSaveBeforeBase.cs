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
    public class TransactionThrowsOnSaveBeforeBase : BaseTest
    {
        [ActiveType]
        internal class User : ActiveType<User>
        {
            [ActiveField]
            public string Name { get; set; }

            public override void Save()
            {
                throw new ApplicationException("Intentional");
            }
        }

        [Test]
        public void CreateParentChildEditChildSaveParent()
        {
            SetUp();
            User u = new User();
            bool failure = true;
            try
            {
                u.Save();
            }
            catch
            {
                failure = false;
            }
            Assert.IsFalse(failure);
            Assert.AreEqual(0, User.Count);
        }
    }
}
