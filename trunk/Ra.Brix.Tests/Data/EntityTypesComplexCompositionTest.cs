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
    public class EntityTypesComplexCompositionTest : BaseTest
    {
        [ActiveRecord]
        internal class Contact : ActiveRecord<Contact>
        {
            [ActiveField]
            public string ContactName { get; set; }
        }

        [ActiveRecord]
        internal class CustomerExtraData : ActiveRecord<CustomerExtraData>
        {
            [ActiveField]
            public string Extra { get; set; }
        }

        [ActiveRecord]
        internal class Address : ActiveRecord<Address>
        {
            [ActiveField]
            public string PostalPlace { get; set; }

            [ActiveField]
            public int ZipCode { get; set; }
        }

        [ActiveRecord]
        internal class Customer : ActiveRecord<Customer>
        {
            private LazyList<Contact> _contacts = new LazyList<Contact>();

            private LazyList<Address> _address = new LazyList<Address>();

            [ActiveField]
            public string CustomerName { get; set; }

            [ActiveField]
            public CustomerExtraData Extra { get; set; }

            [ActiveField]
            public LazyList<Contact> Contacts
            {
                get { return _contacts; }
                set { _contacts = value; }
            }

            [ActiveField]
            public LazyList<Address> Address
            {
                get { return _address; }
                set { _address = value; }
            }
        }

        [Test]
        public void TestEntityTypeCompositionSave()
        {
            SetUp();
            //Database.Instance.RegisterEntityType<Customer>();
            //Database.Instance.RegisterEntityType<Contact>();
            //Database.Instance.RegisterEntityType<Address>();

            Customer cust = new Customer();
            cust.CustomerName = "Ra-Software Inc.";

            cust.Extra = new CustomerExtraData();
            cust.Extra.Extra = "extra string";

            Contact c1 = new Contact();
            c1.ContactName = "Thomas Hansen";
            cust.Contacts.Add(c1);

            Contact c2 = new Contact();
            c2.ContactName = "Kariem Ali";
            cust.Contacts.Add(c2);

            Address ad1 = new Address();
            ad1.PostalPlace = "Norge";
            ad1.ZipCode = 3942;
            cust.Address.Add(ad1);

            Address ad2 = new Address();
            ad2.PostalPlace = "Egypt";
            ad2.ZipCode = 9876;
            cust.Address.Add(ad2);

            cust.Save();

            Customer after = Customer.SelectByID(cust.ID);

            Assert.AreEqual(2, after.Address.Count);
            Assert.AreEqual(2, after.Contacts.Count);
            Assert.AreEqual("Thomas Hansen", after.Contacts[0].ContactName);
            Assert.AreEqual("Kariem Ali", after.Contacts[1].ContactName);
            Assert.AreEqual("Norge", after.Address[0].PostalPlace);
            Assert.AreEqual("Egypt", after.Address[1].PostalPlace);
            Assert.AreEqual(3942, after.Address[0].ZipCode);
            Assert.AreEqual(9876, after.Address[1].ZipCode);
            Assert.AreEqual("extra string", after.Extra.Extra);
        }

        [Test]
        public void TestEntityTypeCompositionSaveDeleteParentThenNoChildren()
        {
            SetUp();
            //Database.Instance.RegisterEntityType<Customer>();
            //Database.Instance.RegisterEntityType<Contact>();
            //Database.Instance.RegisterEntityType<Address>();

            Customer cust = new Customer();
            cust.CustomerName = "Ra-Software Inc.";

            Contact c1 = new Contact();
            c1.ContactName = "Thomas Hansen";
            cust.Contacts.Add(c1);

            Address ad1 = new Address();
            ad1.PostalPlace = "Norge";
            ad1.ZipCode = 3942;
            cust.Address.Add(ad1);

            CustomerExtraData x = new CustomerExtraData();
            x.Extra = "howdy";
            cust.Extra = x;

            cust.Save();

            Customer after = Customer.SelectByID(cust.ID);
            after.Delete();

            Customer after2 = Customer.SelectByID(cust.ID);
            Assert.IsNull(after2);

            Contact afterC1 = Contact.SelectByID(c1.ID);
            Assert.IsNull(afterC1);

            Address afterA1 = Address.SelectByID(ad1.ID);
            Assert.IsNull(afterA1);

            CustomerExtraData afterX1 = CustomerExtraData.SelectByID(x.ID);
            Assert.IsNull(afterX1);
        }

        [Test]
        public void TestEntityTypeCompositionEmptySave()
        {
            SetUp();
            //Database.Instance.RegisterEntityType<Customer>();
            //Database.Instance.RegisterEntityType<Contact>();

            Customer cust = new Customer();
            cust.CustomerName = "Ra-Software Inc.";

            cust.Save();

            Customer after = Customer.SelectByID(cust.ID);
            Assert.AreEqual(0, after.Contacts.Count);
            Assert.AreEqual(0, after.Address.Count);
            Assert.IsNull(after.Extra);
        }

        [Test]
        public void TestEntityTypeCompositionNoListMemberSave()
        {
            SetUp();
            //Database.Instance.RegisterEntityType<Customer>();
            //Database.Instance.RegisterEntityType<Contact>();
            //Database.Instance.RegisterEntityType<CustomerExtraData>();

            Customer cust = new Customer();
            cust.CustomerName = "Ra-Software Inc.";

            CustomerExtraData x = new CustomerExtraData();
            x.Extra = "Howdy";
            cust.Extra = x;

            cust.Save();

            Customer after = Customer.SelectByID(cust.ID);
            Assert.AreEqual(0, after.Contacts.Count);
            Assert.AreEqual(0, after.Address.Count);
            Assert.IsNotNull(after.Extra);
            Assert.AreEqual("Howdy", after.Extra.Extra);
        }

        [Test]
        public void TestEntityTypeCompositionMultipleSave()
        {
            SetUp();
            //Database.Instance.RegisterEntityType<Customer>();
            //Database.Instance.RegisterEntityType<Contact>();

            Customer cust = new Customer();
            cust.CustomerName = "Ra-Software Inc.";

            Contact c1 = new Contact();
            c1.ContactName = "Thomas Hansen";
            cust.Contacts.Add(c1);

            Contact c2 = new Contact();
            c2.ContactName = "Kariem Ali";
            cust.Contacts.Add(c2);

            cust.Save();

            Assert.AreNotEqual(0, cust.ID);
            Assert.AreNotEqual(0, c1.ID);
            Assert.AreNotEqual(0, c2.ID);
        }

        [Test]
        public void TestEntityTypeCompositionSaveAndRetrieve()
        {
            SetUp();
            //Database.Instance.RegisterEntityType<Customer>();
            //Database.Instance.RegisterEntityType<Contact>();

            Customer cust = new Customer();
            cust.CustomerName = "Ra-Software Inc.";

            Contact c1 = new Contact();
            c1.ContactName = "Thomas Hansen";
            cust.Contacts.Add(c1);
            Assert.AreEqual(0, c1.ID);

            cust.Save();

            Assert.AreNotEqual(0, c1.ID);

            Customer after = Customer.SelectByID(cust.ID);

            Assert.IsNotNull(after);
            Assert.IsNotNull(after.Contacts);
            Assert.AreEqual(1, after.Contacts.Count);
            Assert.AreEqual(c1.ID, after.Contacts[0].ID);
        }

        [Test]
        public void TestEntityTypeCompositionMultipleSaveAndRetrieve()
        {
            SetUp();
            //Database.Instance.RegisterEntityType<Customer>();
            //Database.Instance.RegisterEntityType<Contact>();

            Customer cust = new Customer();
            cust.CustomerName = "Ra-Software Inc.";

            Contact c1 = new Contact();
            c1.ContactName = "Thomas Hansen";
            cust.Contacts.Add(c1);

            Contact c2 = new Contact();
            c2.ContactName = "Thomas Hansen II";
            cust.Contacts.Add(c2);

            cust.Save();

            Customer after = Customer.SelectByID(cust.ID);

            Assert.AreEqual(2, after.Contacts.Count);
            Assert.AreEqual(c1.ID, after.Contacts[0].ID);
            Assert.AreEqual(c2.ID, after.Contacts[1].ID);
        }


        // Has many tests
        [ActiveRecord]
        public class User : ActiveRecord<User>
        {
            private LazyList<Role> _roles = new LazyList<Role>();

            [ActiveField]
            public string Name { get; set; }

            [ActiveField]
            public string Pwd { get; set; }

            [ActiveField(IsOwner=false)]
            public LazyList<Role> Roles
            {
                get { return _roles; }
                set { _roles = value; }
            }
        }

        [ActiveRecord]
        public class Role : ActiveRecord<Role>
        {
            [ActiveField]
            public string Name { get; set; }
        }

        [Test]
        public void TestMany2Many()
        {
            SetUp();
            //Database.Instance.RegisterEntityType<User>();
            //Database.Instance.RegisterEntityType<Role>();

            Role r1 = new Role();
            r1.Name = "Administrator";
            r1.Save();

            Role r2 = new Role();
            r2.Name = "Debugger";
            r2.Save();

            User u1 = new User();
            u1.Name = "ole";
            u1.Pwd = "ole1";
            u1.Roles.Add(r1);
            u1.Roles.Add(r2);
            u1.Save();

            User u2 = new User();
            u2.Name = "per";
            u2.Pwd = "per1";
            u2.Roles.Add(r1);
            u2.Roles.Add(r2);
            u2.Save();

            // Must be TWO roles exactly...
            Assert.AreEqual(2, Role.Count);

            User afterUser1 = User.SelectByID(u1.ID);
            User afterUser2 = User.SelectByID(u2.ID);

            // Roles for the two different users MUST be the same...
            Assert.AreEqual(u1.Roles[0].ID, afterUser1.Roles[0].ID);
            Assert.AreEqual(u1.Roles[1].ID, afterUser1.Roles[1].ID);
            Assert.AreEqual(u2.Roles[0].ID, afterUser2.Roles[0].ID);
            Assert.AreEqual(u2.Roles[1].ID, afterUser2.Roles[1].ID);

            // Roles must NOT be deleted when User is deleted...
            afterUser1.Delete();
            Assert.AreEqual(2, Role.Count);

            afterUser2 = User.SelectByID(u2.ID);

            // Roles for the two different users MUST be the same...
            Assert.AreEqual(u2.Roles[0].ID, afterUser2.Roles[0].ID);
            Assert.AreEqual(u2.Roles[1].ID, afterUser2.Roles[1].ID);
        }

        [Test]
        public void TestMany2ManyEmpty()
        {
            SetUp();
            //Database.Instance.RegisterEntityType<User>();
            //Database.Instance.RegisterEntityType<Role>();

            Role r1 = new Role();
            r1.Name = "Administrator";
            r1.Save();

            Role r2 = new Role();
            r2.Name = "Debugger";
            r2.Save();

            User u1 = new User();
            u1.Name = "ole";
            u1.Pwd = "ole1";
            u1.Save(); // Notice NO Roles on this bugger...

            // Must be TWO roles exactly...
            Assert.AreEqual(2, Role.Count);

            User afterUser1 = User.SelectByID(u1.ID);

            // Roles for the two different users MUST be the same...
            Assert.AreEqual(0, afterUser1.Roles.Count);

            afterUser1.Roles.Add(r1);
            afterUser1.Save();

            afterUser1 = User.SelectByID(u1.ID);

            // Roles for the two different users MUST be the same...
            Assert.AreEqual(1, afterUser1.Roles.Count);

            // Roles must NOT be deleted when User is deleted...
            afterUser1.Delete();
            Assert.AreEqual(2, Role.Count);
        }

        [Test]
        public void TestMany2ManySaveTwice()
        {
            SetUp();
            //Database.Instance.RegisterEntityType<User>();
            //Database.Instance.RegisterEntityType<Role>();

            Role r1 = new Role();
            r1.Name = "Administrator";
            r1.Save();

            User u1 = new User();
            u1.Name = "ole";
            u1.Pwd = "ole1";
            u1.Roles.Add(r1);
            u1.Save();
            u1.Save();

            // Must be TWO roles exactly...
            Assert.AreEqual(1, Role.Count);

            User afterUser1 = User.SelectByID(u1.ID);

            // Roles for the two different users MUST be the same...
            Assert.AreEqual(1, afterUser1.Roles.Count);
        }

        [Test]
        public void TestMany2ManyAddAndRemove()
        {
            SetUp();
            //Database.Instance.RegisterEntityType<User>();
            //Database.Instance.RegisterEntityType<Role>();

            Role r1 = new Role();
            r1.Name = "Administrator";
            r1.Save();

            User u1 = new User();
            u1.Name = "ole";
            u1.Pwd = "ole1";
            u1.Roles.Add(r1);
            u1.Save();

            // Must be TWO roles exactly...
            Assert.AreEqual(1, Role.Count);

            User afterUser1 = User.SelectByID(u1.ID);

            // Roles for the two different users MUST be the same...
            Assert.AreEqual(1, afterUser1.Roles.Count);

            afterUser1.Roles.RemoveAt(0);
            afterUser1.Save();

            afterUser1 = User.SelectByID(u1.ID);

            // Roles for the two different users MUST be the same...
            Assert.AreEqual(0, afterUser1.Roles.Count);
        }

        [Test]
        public void TestMany2ManyAddMultipleDeleteOne()
        {
            SetUp();
            //Database.Instance.RegisterEntityType<User>();
            //Database.Instance.RegisterEntityType<Role>();

            Role r1 = new Role();
            r1.Name = "Administrator";
            r1.Save();

            User u1 = new User();
            u1.Name = "ole";
            u1.Pwd = "ole1";
            u1.Roles.Add(r1);
            u1.Save();

            // Must be TWO roles exactly...
            Assert.AreEqual(1, Role.Count);

            User afterUser1 = User.SelectByID(u1.ID);

            // Roles for the two different users MUST be the same...
            Assert.AreEqual(1, afterUser1.Roles.Count);

            afterUser1.Roles.Clear();
            afterUser1.Save();
            r1.Delete();

            afterUser1 = User.SelectByID(u1.ID);

            // Roles for the two different users MUST be the same...
            Assert.AreEqual(0, afterUser1.Roles.Count);
        }
    }
}




















