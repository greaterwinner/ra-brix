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

namespace Ra.Brix.Tests.Data
{
    [TestFixture]
    public class EntityTypesInheritanceTest : BaseTest
    {
        [ActiveType]
        internal class Mammal<T> : ActiveType<T>
        {
            [ActiveField]
            public string Name { get; set; }

            [ActiveField]
            public bool IsPrimate { get; set; }
        }

        internal class Dog : Mammal<Dog>
        {
            [ActiveField]
            public bool Barks { get; set; }
        }

        [Test]
        public void TestInheritedTemplateClass()
        {
            SetUp();
            //Database.Instance.RegisterEntityType<Dog>();
            Dog d = new Dog();
            d.Name = "Goldie";
            d.IsPrimate = false;
            d.Barks = true;
            d.Save();

            Dog d2 = Dog.SelectByID(d.ID);
            Assert.IsNotNull(d2);
            Assert.AreEqual(d2.Name, "Goldie");
            Assert.AreEqual(d2.IsPrimate, false);
            Assert.AreEqual(d2.Barks, true);
        }
    }
}
