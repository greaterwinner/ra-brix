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
using Ra.Brix.Loader;

namespace Ra.Brix.Tests.Modules
{
    [TestFixture]
    public class StaticEventHandlersModules
    {
        [ActiveModule]
        internal class TestModule
        {
            [ActiveEvent(Name = "TestEvent")]
            private static void TestEvent(object sender, ActiveEventArgs e)
            {
                WasTriggered = true;
            }

            public static bool WasTriggered { get; set; }
        }

        [Test]
        public void VerifyEventTriggeredInStaticModelMethod()
        {
            ActiveEvents.Instance.RaiseActiveEvent(this, "TestEvent");
            Assert.AreEqual(true, TestModule.WasTriggered);
        }
    }
}
