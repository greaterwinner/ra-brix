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
    public class CriteriaWithChildValue : BaseTest
    {
        [ActiveRecord]
        public class Role : ActiveRecord<Role>
        {
            [ActiveField]
            public string Name { get; set; }
        }

        [ActiveRecord]
        public class User : ActiveRecord<User>
        {
            [ActiveField]
            public string Name { get; set; }

            [ActiveField]
            public Role Role { get; set; }
        }

        [Test]
        public void GetCriteriaWithChildParameterValue()
        {
            SetUp();

            Role r = new Role();
            r.Name = "admin";

            User u = new User();
            u.Name = "thomas";
            u.Role = r;
            u.Save();

            User u2 = User.SelectFirst(Criteria.Eq("Role.Name", "admin"));
            Assert.AreEqual("thomas", u2.Name);
        }

        [Test]
        public void GetCriteriaWithChildParameterValueMultipleValuesOneHit()
        {
            SetUp();

            Role r = new Role();
            r.Name = "admin";

            User u = new User();
            u.Name = "thomas";
            u.Role = r;
            u.Save();

            r = new Role();
            r.Name = "admin2";

            u = new User();
            u.Name = "thomas2";
            u.Role = r;
            u.Save();

            List<User> u2 = new List<User>(User.Select(Criteria.Eq("Role.Name", "admin")));
            Assert.AreEqual(1, u2.Count);
            Assert.AreEqual("thomas", u2[0].Name);
        }

        [Test]
        public void GetCriteriaWithChildParameterValueMultipleValuesMultipleHits()
        {
            SetUp();

            Role r = new Role();
            r.Name = "admin";

            User u = new User();
            u.Name = "thomas";
            u.Role = r;
            u.Save();

            r = new Role();
            r.Name = "admin2";

            u = new User();
            u.Name = "thomas2";
            u.Role = r;
            u.Save();

            List<User> u2 = new List<User>(User.Select(Criteria.Like("Role.Name", "adm%")));
            Assert.AreEqual(2, u2.Count);
            Assert.AreEqual("thomas", u2[0].Name);
            Assert.AreEqual("thomas2", u2[1].Name);
        }
    }
}
