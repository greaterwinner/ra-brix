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
    public class MultipleSingleChildrenOfSameTypeIsOwner : BaseTest
    {
        [ActiveRecord]
        public class Role : ActiveRecord<Role>
        {
            [ActiveField]
            public int Value { get; set; }
            
            [ActiveField]
            public Role ChildRole { get; set; }
        }

        [ActiveRecord]
        public class User : ActiveRecord<User>
        {
            [ActiveField]
            public string Name{ get; set; }

            [ActiveField]
            public Role Role1 { get; set; }

            [ActiveField]
            public Role Role2 { get; set; }
        }

        [Test]
        public void SaveWithTwoDifferentValue()
        {
            User u1 = new User();
            u1.Name = "Thomas";
            u1.Role1 = new Role();
            u1.Role1.Value = 1;
            u1.Role2 = new Role();
            u1.Role2.Value = 2;
            u1.Save();

            User u2 = new User();
            u2.Name = "Kariem";
            u2.Role1 = new Role();
            u2.Role1.Value = 2;
            u2.Role2 = new Role();
            u2.Role2.Value = 1;
            u2.Save();

            User u1_after = User.SelectByID(u1.ID);
            Assert.AreEqual(1, u1_after.Role1.Value);
            Assert.AreEqual(2, u1_after.Role2.Value);

            User u2_after = User.SelectByID(u2.ID);
            Assert.AreEqual(2, u2_after.Role1.Value);
            Assert.AreEqual(1, u2_after.Role2.Value);
        }

        [Test]
        public void ShouldSaveObjectActiveField()
        {
            Role childRole = new Role();
            childRole.Value = 4;
            childRole.Save();

            Role role = new Role();
            role.Value = 5;
            role.ChildRole = childRole;
            role.Save();

            Role roleAfterSave = Role.SelectByID(role.ID);
            Assert.AreEqual(5, roleAfterSave.Value);
            Assert.AreEqual(4, roleAfterSave.ChildRole.Value);
        }
    }
}
