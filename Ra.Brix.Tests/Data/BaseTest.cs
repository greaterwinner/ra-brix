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
using Ra.Brix.Data.Adapters;
using System.Reflection;
using Ra.Brix.Data.Internal;

namespace Ra.Brix.Tests.Data
{
    public class BaseTest
    {
        [TestFixtureSetUp]
        public void SetUpTestFixture()
        {
            // Stupid reference to make sure assembly gets into AppDomain...!
#pragma warning disable 168
// ReSharper disable InconsistentNaming
            Brix.Data.Adapters.MSSQL.MSSQL MSSQL = null;
            Brix.Data.Adapters.MySQL.MySQL MySQL = null;
            Brix.Data.Adapters.XML.XML XML = null;
// ReSharper restore InconsistentNaming
#pragma warning restore 168
        }

        [TestFixtureTearDown]
        public void TearDownTestFixture()
        {
        }

        // This method will *delete all* documents in database...!
        protected void SetUp()
        {
            List<int> idsOfAllDocumentsToDelete = new List<int>();
            foreach (object idx in Adapter.Instance.Select())
            {
                Type type = idx.GetType();
                PropertyInfo method = type.GetProperty(
                    "ID", 
                    BindingFlags.Instance | 
                    BindingFlags.Public | 
                    BindingFlags.NonPublic);
                int id = (int)method.GetGetMethod(true).Invoke(idx, null);
                idsOfAllDocumentsToDelete.Add(id);
            }
            foreach (int idx in idsOfAllDocumentsToDelete)
            {
                Adapter.Instance.Delete(idx);
            }
        }
    }
}
