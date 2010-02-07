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
using ArticlePublisherRecords;
using UserRecords;

namespace Ra.Brix.Tests.Data
{
    [TestFixture]
    public class ArticleTests : BaseTest
    {
        [Test]
        public void VerifySavingParentParentObjectPreservesDeepestChildren()
        {
            SetUp();
            User user = new User();
            user.Username = "thomas";
            Role role = new Role();
            role.Name = "admin";
            user.Roles.Add(role);
            user.Save();

            user = User.SelectFirst();
            Assert.AreEqual(1, user.Roles.Count);
            Assert.AreEqual("admin", user.Roles[0].Name);

            Article a = new Article();
            a.Header = "sdfihsdf";
            a.Body = "sdfiojhsdf";
            a.Followers.Add(user);
            a.Author = user;
            a.Save();

            user = User.SelectFirst();
            Assert.AreEqual(1, user.Roles.Count);
            Assert.AreEqual("admin", user.Roles[0].Name);
        }
    }
}
