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
    public class MultipleListChildrenOfSameTypeIsOwner : BaseTest
    {
        [ActiveRecord]
        public class Role : ActiveRecord<Role>
        {
            public Role() 
            {
                ChildRoles = new List<Role>();
            }

            [ActiveField]
            public int Value { get; set; }

            [ActiveField]
            public List<Role> ChildRoles { get; set; }
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

            [ActiveField]
            public LazyList<Role> Role1 { get; set; }

            [ActiveField]
            public LazyList<Role> Role2 { get; set; }
        }

        [Test]
        public void SaveWithTwoDifferentValue()
        {
            User u1 = new User();
            u1.Name = "Thomas";
            u1.Role1.Add(new Role());
            u1.Role1[0].Value = 1;
            u1.Role2.Add(new Role());
            u1.Role2[0].Value = 2;
            u1.Role2.Add(new Role());
            u1.Role2[1].Value = 3;
            u1.Save();

            User u2 = new User();
            u2.Name = "Kariem";
            u2.Role1.Add(new Role());
            u2.Role1[0].Value = 2;
            u2.Role2.Add(new Role());
            u2.Role2[0].Value = 1;
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
              
        [Test]
        public void ShouldSaveListActiveField()
        {
            Role childRole1 = new Role();
            childRole1.Value = 2;
            childRole1.Save();

            Role childRole2 = new Role();
            childRole2.Value = 1;
            childRole2.Save();
            
            Role role = new Role();
            role.Value = 3;

            role.ChildRoles.AddRange(new Role[] { childRole1, childRole2 });
            role.Save();

            Role roleAfterSave = Role.SelectByID(role.ID);

            Assert.AreEqual(3, roleAfterSave.Value);
            Assert.AreEqual(2, roleAfterSave.ChildRoles.Count);
            Assert.AreEqual(2, roleAfterSave.ChildRoles[0].Value);
            Assert.AreEqual(1, roleAfterSave.ChildRoles[1].Value);
        }
    }
}
