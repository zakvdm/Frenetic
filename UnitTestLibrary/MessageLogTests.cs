using System;
using NUnit.Framework;
using Frenetic;
using System.Collections.Generic;

namespace UnitTestLibrary
{
    [TestFixture]
    public class MessageLogTests
    {
        
        [Test]
        public void CanIterateThroughLog()
        {
            MessageLog chatMsgLog = new MessageLog();
            chatMsgLog.AddMessage("hello there");
            chatMsgLog.AddMessage("you suck");
            chatMsgLog.AddMessage("i'm just kidding, you're pretty cool.");

            int count = 0;
            foreach (string message in chatMsgLog)
            {
                count++;
            }

            Assert.AreEqual(3, count);
        }

        [Test]
        public void LogReturnsMessagesWithNewestFirst()
        {
            MessageLog chatMsgLog = new MessageLog();
            chatMsgLog.AddMessage("1");
            chatMsgLog.AddMessage("2");
            chatMsgLog.AddMessage("3");

            foreach (string message in chatMsgLog)
            {
                Assert.AreEqual("3", message);
                break;
            }
        }

        [Test]
        public void CanIterateThroughLogFromOldestToNewest()
        {
            MessageLog chatLog = new MessageLog();
            chatLog.AddMessage("old");
            chatLog.AddMessage("new");

            int count = 1;
            foreach (string msg in chatLog.OldestToNewest)
            {
                if (count == 1)
                    Assert.AreEqual("old", msg);
                if (count == 2)
                    Assert.AreEqual("new", msg);
                count++;
            }
        }

        [Test]
        public void LogIndexesMessagesWithNewestAtZero()
        {
            MessageLog chatMsgLog = new MessageLog();
            chatMsgLog.AddMessage("1");
            chatMsgLog.AddMessage("2");
            chatMsgLog.AddMessage("3");

            Assert.AreEqual("3", chatMsgLog[0]);
            Assert.AreEqual("1", chatMsgLog[2]);
        }

        [Test]
        public void CanAccessElementsMoreThanOnce()
        {
            MessageLog chatMsgLog = new MessageLog();
            chatMsgLog.AddMessage("hey");

            foreach (string message in chatMsgLog)
                Assert.AreEqual("hey", message);

            foreach (string message in chatMsgLog)
                Assert.AreEqual("hey", message);
        }

        [Test]
        public void CanMakeFromList()
        {
            List<string> testList = new List<string>();
            testList.Add("hello");
            MessageLog msgLog = new MessageLog(testList);

            foreach (string msg in msgLog)
                Assert.AreEqual("hello", msg);
        }

        [Test]
        public void KeepsCount()
        {
            MessageLog chatMsgLog = new MessageLog();
            chatMsgLog.AddMessage("1");
            chatMsgLog.AddMessage("2");

            Assert.AreEqual(2, chatMsgLog.Count);
        }

        [Test]
        public void BuildFromAnotherMessageLogWorks()
        {
            MessageLog sourceLog = new MessageLog();
            MessageLog destinationLog = new MessageLog();
            destinationLog.AddMessage("blah blah");
            sourceLog.AddMessage("profound statement");
            sourceLog.AddMessage("witty rejoinder");

            destinationLog.BuildFromAnotherMessageLog(sourceLog);

            Assert.AreEqual(2, destinationLog.Count);
            Assert.AreEqual("witty rejoinder", destinationLog[0]);
            Assert.AreEqual("profound statement", destinationLog[1]);
        }

        [Test]
        public void CanStripOldestMessage()
        {
            MessageLog log = new MessageLog();
            log.AddMessage("old msg");
            log.AddMessage("newer msg");

            Assert.AreEqual("old msg", log.StripOldestMessage());
            Assert.AreEqual("newer msg", log.StripOldestMessage());
        }

        [Test]
        public void CanCopy()
        {
            MessageLog original = new MessageLog();
            original.AddMessage("1");

            MessageLog copy = original.Copy();
            original.AddMessage("2");

            Assert.AreEqual(1, copy.Count);
            Assert.AreEqual("1", copy[0]);
        }
    }
}
