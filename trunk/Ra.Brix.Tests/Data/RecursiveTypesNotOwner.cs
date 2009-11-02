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
    public class RecursiveTypesNotOwner : BaseTest
    {
        [ActiveType]
        public class Role : ActiveType<Role>
        {
            [ActiveField]
            public int Value { get; set; }
        }

        [ActiveType]
        public class User : ActiveType<User>
        {
            public User()
            {
                Roles = new LazyList<Role>();
            }

            [ActiveField]
            public int Value { get; set; }

            [ActiveField(IsOwner=false)]
            public LazyList<Role> Roles { get; set; }
        }

        [ActiveType]
        public class AdministratorUser : ActiveType<AdministratorUser>
        {
            [ActiveField(IsOwner=false)]
            public User User { get; set; }

            [ActiveField]
            public int Value { get; set; }
        }

        [Test]
        public void SaveObjectContainingAnotherNotOwnedAndVerifyContainedNotDeleted()
        {
            SetUp();

            Role r = new Role();
            r.Value = 1;
            r.Save();

            User u = new User();
            u.Value = 2;
            u.Roles.Add(r);
            u.Save();

            AdministratorUser a = new AdministratorUser();
            a.Value = 3;
            a.User = u;
            a.Save();

            // If our "IsOwner=false" relationships goes wrong then the save logic
            // beneath will remove all roles from the User contained within the
            // administrator object. This is because the Roles have never been
            // touched. But since User is NOT owned by the Administrator
            // this should logically NOT happen...
            AdministratorUser a2 = AdministratorUser.SelectByID(a.ID);
            a2.Value = 6;
            a2.Save();

            AdministratorUser a3 = AdministratorUser.SelectByID(a2.ID);
            Assert.AreEqual(2, a3.User.Value);
            Assert.AreEqual(1, a3.User.Roles[0].Value);
        }

        [Test]
        public void SaveRichHierarchyThenSwitchChild()
        {
            SetUp();

            Role r = new Role();
            r.Value = 1;
            r.Save();

            User u = new User();
            u.Value = 2;
            u.Roles.Add(r);
            u.Save();

            User u2 = new User();
            u2.Value = 12;
            u2.Save();

            AdministratorUser a = new AdministratorUser();
            a.Value = 3;
            a.User = u;
            a.Save();

            // If our "IsOwner=false" relationships goes wrong then the save logic
            // beneath will remove all roles from the User contained within the
            // administrator object. This is because the Roles have never been
            // touched. But since User is NOT owned by the Administrator
            // this should logically NOT happen...
            AdministratorUser a2 = AdministratorUser.SelectByID(a.ID);
            a2.Value = 6;
            a2.User = u2;
            a2.Save();

            AdministratorUser a3 = AdministratorUser.SelectByID(a2.ID);
            Assert.AreEqual(12, a3.User.Value);
            Assert.AreEqual(0, a3.User.Roles.Count);
        }

        [Test]
        public void SaveThenRetrieveWithoutCheckingRolesThenSaveAndRetrieve()
        {
            SetUp();
            Role r1 = new Role();
            r1.Value = 1;
            r1.Save();

            User u1 = new User();
            u1.Roles.Add(r1);
            u1.Save();

            // If our LazyList logic doesn't work then this save will REMOVE all Roles from User...!
            User u2 = User.SelectByID(u1.ID);
            u2.Save();

            // Unless our Lazy Loading logic works correctly then this query
            // will have *ZERO* Role items...!
            User u3 = User.SelectByID(u1.ID);
            Assert.AreEqual(1, u3.Roles.Count);
        }

        [Test]
        public void ChangeRoleValueCheckUserToSeeIfRoleWasChanged()
        {
            SetUp();
            Role r1 = new Role();
            r1.Value = 1;
            r1.Save();

            User u1 = new User();
            u1.Roles.Add(r1);
            u1.Save();

            Role r2 = Role.SelectByID(r1.ID);
            r2.Value = 2;
            r2.Save();

            User u2 = User.SelectByID(u1.ID);
            Assert.AreEqual(2, u2.Roles[0].Value);
        }
    }
}
