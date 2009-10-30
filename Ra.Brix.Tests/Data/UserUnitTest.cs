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
using UserRecords;

namespace Ra.Brix.Tests.Data
{
    [TestFixture]
    public class UserUnitTest : BaseTest
    {
        [Test]
        [ExpectedException]
        public void VerifyTwoSimilarUsernameThrows()
        {
            SetUp();

            User user = new User();
            user.Username = "thomas";
            user.Save();

            User user2 = new User();
            user2.Username = "thomas";
            user2.Save();
        }
    }
}
