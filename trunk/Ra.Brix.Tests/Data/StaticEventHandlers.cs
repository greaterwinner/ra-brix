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

namespace Ra.Brix.Tests.Data
{
    [TestFixture]
    public class StaticEventHandlers : BaseTest
    {
        [ActiveRecord]
        internal class User : ActiveRecord<User>
        {
            [ActiveField]
            public string Username { get; set; }

            public static bool WasTriggered;

            public override void Save()
            {
                base.Save();

                // Stupid de-reference to make sure we've initialized all our types and such...!
                // In an actual application, this will never be neseccary since all those things
                // triggering events will be running something within the PluginLoader BEFORE
                // they run anything in the Events class...
                object tmp = PluginLoader.Instance;
                ActiveEvents.Instance.RaiseActiveEvent(
                    this, 
                    "SaveTriggered", 
                    new Node("Value", 5));
            }

            [ActiveEvent(Name = "SaveTriggered")]
            private static void SaveTriggered(object sender, ActiveEventArgs e)
            {
                if (sender is User && (int)e.Params.Value == 5)
                    WasTriggered = true;
            }
        }

        [Test]
        public void VerifyEventTriggeredInStaticModelMethod()
        {
            SetUp();
            User u = new User();
            u.Username = "thomas";
            u.Save();
            Assert.AreEqual(true, User.WasTriggered);
        }
    }
}
