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
    public class ParentChild : BaseTest
    {
        [ActiveType]
        internal class User : ActiveType<User>
        {
            public User()
            {
                Roles = new LazyList<Role>();
            }

            [ActiveField]
            public string Username { get; set; }

            [ActiveField]
            public string Password { get; set; }

            [ActiveField]
            public LazyList<Role> Roles { get; set; }
        }

        [ActiveType]
        internal class Role : ActiveType<Role>
        {
            [ActiveField]
            public string Name { get; set; }
        }

        [Test]
        public void CreateParentChildEditChildSaveParent()
        {
            SetUp();
            User u = new User();
            u.Username = "uname";
            u.Password = "pwd";
            Role r1 = new Role();
            r1.Name = "admin";
            u.Roles.Add(r1);
            u.Save();

            User u2 = User.SelectByID(u.ID);
            Assert.IsNotNull(u2);
            Assert.IsNotNull(u2.Roles);
            Assert.AreEqual(1, u2.Roles.Count);
            Assert.AreEqual("admin", u2.Roles[0].Name);

            u2.Roles[0].Name = "admin2";
            u2.Roles[0].Save();

            User u3 = User.SelectByID(u.ID);
            Assert.IsNotNull(u3);
            Assert.IsNotNull(u3.Roles);
            Assert.AreEqual(1, u3.Roles.Count);
            Assert.AreEqual("admin2", u3.Roles[0].Name);
        }
    }
}
