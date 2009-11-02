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
    public class RecursiveTypesNotLazy : BaseTest
    {
        [ActiveType]
        public class Tree : ActiveType<Tree>
        {
            public Tree()
            {
                Children = new List<Tree>();
            }

            public Tree(int value)
            {
                Value = value;
                Children = new List<Tree>();
            }

            [ActiveField]
            public int Value { get; set; }

            [ActiveField]
            public List<Tree> Children { get; set; }
        }

        [Test]
        public void SaveAndRetrieveRecursiveTreeLazy()
        {
            SetUp();

            Tree root1 = new Tree(1);
            Tree root1c1 = new Tree(4);
            root1.Children.Add(root1c1);
            Tree root1c2 = new Tree(5);
            root1.Children.Add(root1c2);

            Tree root2 = new Tree(2);
            Tree root2c1 = new Tree(6);
            root2.Children.Add(root2c1);
            Tree root2c2 = new Tree(7);
            root2.Children.Add(root2c2);

            Tree root3 = new Tree(3);
            Tree root3c1 = new Tree(8);
            root3.Children.Add(root3c1);
            Tree root3c2 = new Tree(9);
            root3.Children.Add(root3c2);

            Tree root3c2c1 = new Tree(10);
            root3c2.Children.Add(root3c2c1);

            root1.Save();
            root2.Save();
            root3.Save();

            Tree r1 = Tree.SelectByID(root1.ID);
            Tree r2 = Tree.SelectByID(root2.ID);
            Tree r3 = Tree.SelectByID(root3.ID);

            Assert.AreEqual(2, r1.Children.Count);
            Assert.AreEqual(4, r1.Children[0].Value);
            Assert.AreEqual(5, r1.Children[1].Value);

            Assert.AreEqual(2, r2.Children.Count);
            Assert.AreEqual(6, r2.Children[0].Value);
            Assert.AreEqual(7, r2.Children[1].Value);

            Assert.AreEqual(2, r3.Children.Count);
            Assert.AreEqual(8, r3.Children[0].Value);
            Assert.AreEqual(9, r3.Children[1].Value);

            Assert.AreEqual(1, r3.Children[1].Children.Count);
            Assert.AreEqual(10, r3.Children[1].Children[0].Value);
        }
    }
}
