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
    public class EntityTypesCompositionTest : BaseTest
    {
        [ActiveRecord]
        internal class Contact : ActiveRecord<Contact>
        {
            [ActiveField]
            public string ContactName { get; set; }
        }

        [ActiveRecord]
        internal class Customer : ActiveRecord<Customer>
        {
            private LazyList<Contact> _contacts = new LazyList<Contact>();

            [ActiveField]
            public string CustomerName { get; set; }

            [ActiveField]
            public LazyList<Contact> Contacts
            {
                get { return _contacts; }
                set { _contacts = value; }
            }
        }

        [Test]
        public void TestEntityTypeCompositionSave()
        {
            SetUp();
            //Database.Instance.RegisterEntityType<Customer>();
            //Database.Instance.RegisterEntityType<Contact>();

            Customer cust = new Customer();
            cust.CustomerName = "Ra-Software Inc.";

            Contact c1 = new Contact();
            c1.ContactName = "Thomas Hansen";
            cust.Contacts.Add(c1);

            cust.Save();

            Assert.AreNotEqual(0, cust.ID);
            Assert.AreNotEqual(0, c1.ID);
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
        public void TestEntityTypeCompositionSaveAndRetrieveTWICE()
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

            Contact c2 = new Contact();
            c2.ContactName = "Thomas Hansen";
            cust.Contacts.Add(c2);
            cust.Save();

            Customer after2 = Customer.SelectByID(cust.ID);
            Assert.AreEqual(2, after2.Contacts.Count);

            Customer after3 = Customer.SelectByID(cust.ID);
            Assert.AreEqual(2, after3.Contacts.Count);
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

        [ActiveRecord]
        internal class Customer2 : ActiveRecord<Customer2>
        {
            [ActiveField]
            public string CustomerName { get; set; }

            [ActiveField(IsOwner = false)]
            public Contact2 Contact { get; set; }
        }

        [ActiveRecord]
        internal class Contact2 : ActiveRecord<Contact2>
        {
            [ActiveField]
            public string ContactName { get; set; }
        }

        [Test]
        public void TestOneCompositionNotOwnedEntityTypeNotNullAfterRetrieve()
        {
            SetUp();
            //Database.Instance.RegisterEntityType<Customer2>();
            //Database.Instance.RegisterEntityType<Contact2>();

            Contact2 cont1 = new Contact2();
            cont1.ContactName = "thomas hansen";
            cont1.Save();

            Contact2 cont2 = new Contact2();
            cont2.ContactName = "ola dunk";
            cont2.Save();

            Customer2 cust1 = new Customer2();
            cust1.CustomerName = "Ra-Software";
            cust1.Contact = cont2;
            cust1.Save();

            Customer2 cust2 = Customer2.SelectByID(cust1.ID);
            Assert.IsNotNull(cust2.Contact);
            Assert.AreEqual(cust2.Contact.ContactName, cont2.ContactName);
        }

        [Test]
        public void TestOneCompositionNotOwnedEntityTypeValuesRight()
        {
            SetUp();
            //Database.Instance.RegisterEntityType<Customer2>();
            //Database.Instance.RegisterEntityType<Contact2>();

            Contact2 cont1 = new Contact2();
            cont1.ContactName = "thomas hansen";
            cont1.Save();

            Contact2 cont2 = new Contact2();
            cont2.ContactName = "ola dunk";
            cont2.Save();

            Customer2 cust1 = new Customer2();
            cust1.CustomerName = "Ra-Software";
            cust1.Contact = cont2;
            cust1.Save();

            Customer2 cust2 = Customer2.SelectByID(cust1.ID);
            Assert.IsNotNull(cust2.Contact);
            Assert.AreEqual(cust2.Contact.ContactName, cont2.ContactName);

            Contact2 cont3 = Contact2.SelectByID(cont2.ID);
            cont3.Delete();

            Customer2 cust3 = Customer2.SelectByID(cust1.ID);
            Assert.IsNull(cust3.Contact);
        }
    }
}
