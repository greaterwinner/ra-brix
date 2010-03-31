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
using ArticlePublisherRecords;
using UserRecords;
using StackedRecords;

namespace Ra.Brix.Tests.Data
{
    [TestFixture]
    public class QuestionTests : BaseTest
    {
        [Test]
        public void CreateQuestionLoadDontRetrieveLazyAndSave()
        {
            SetUp();
            Question q = new Question();
            q.URL = "mumbo-jumbo";
            q.Body = "mumbo jumbo";
            Answer a1 = new Answer();
            q.Answers.Add(a1);
            q.Save();

            // We want to verify that a non-touched LazyList will not be deleted ...!
            Question q2 = Question.SelectByID(q.ID);
            q2.Save();
            q2 = Question.SelectByID(q.ID);

            Assert.IsNotNull(q2.Answers);
            Assert.AreEqual(1, q2.Answers.Count);
        }
    }
}
