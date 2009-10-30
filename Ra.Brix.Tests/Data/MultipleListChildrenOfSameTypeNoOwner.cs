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
    public class MultipleListChildrenOfSameTypeNoOwner : BaseTest
    {
        [ActiveRecord]
        public class Role : ActiveRecord<Role>
        {
            [ActiveField]
            public int Value { get; set; }
        }

        [ActiveRecord]
        public class User : ActiveRecord<User>
        {
            public User()
            {
                Role1 = new LazyList<Role>();
                Role2 = new LazyList<Role>();
            }
            [ActiveField]
            public string Name{ get; set; }

            [ActiveField(IsOwner = false)]
            public LazyList<Role> Role1 { get; set; }

            [ActiveField(IsOwner = false)]
            public LazyList<Role> Role2 { get; set; }
        }

        [Test]
        public void SaveWithTwoDifferentValue()
        {
            Role r1 = new Role();
            r1.Value = 1;
            r1.Save();

            Role r2 = new Role();
            r2.Value = 2;
            r2.Save();

            Role r3 = new Role();
            r3.Value = 3;
            r3.Save();

            User u1 = new User();
            u1.Name = "Thomas";
            u1.Role1.Add(r1);
            u1.Role2.Add(r2);
            u1.Role2.Add(r3);
            u1.Save();

            User u2 = new User();
            u2.Name = "Kariem";
            u2.Role1.Add(r2);
            u2.Role2.Add(r1);
            u2.Save();

            User u1_after = User.SelectByID(u1.ID);
            Assert.AreEqual(1, u1_after.Role1.Count);
            Assert.AreEqual(2, u1_after.Role2.Count);
            Assert.AreEqual(1, u1_after.Role1[0].Value);
            Assert.AreEqual(2, u1_after.Role2[0].Value);
            Assert.AreEqual(3, u1_after.Role2[1].Value);

            User u2_after = User.SelectByID(u2.ID);
            Assert.AreEqual(1, u2_after.Role1.Count);
            Assert.AreEqual(1, u2_after.Role2.Count);
            Assert.AreEqual(2, u2_after.Role1[0].Value);
            Assert.AreEqual(1, u2_after.Role2[0].Value);
        }
    }
}
