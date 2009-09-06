using System;
using NUnit.Framework;
using Frenetic;
using System.Collections.Generic;

namespace UnitTestLibrary
{
    [TestFixture]
    public class LogTests
    {
        
        [Test]
        public void CanIterateThroughLog()
        {
            Log<string> chatMsgLog = new Log<string>();
            chatMsgLog.Add("hello there");
            chatMsgLog.Add("you suck");
            chatMsgLog.Add("i'm just kidding, you're pretty cool.");

            int count = 0;
            foreach (string message in chatMsgLog)
            {
                count++;
            }

            Assert.AreEqual(3, count);
        }

        [Test]
        public void CanBuildALogOfChatMessages()
        {
            Log<ChatMessage> chatMsgLog = new Log<ChatMessage>();
            
            chatMsgLog.Add(new ChatMessage() { ClientName = "Zak", Message = "yo" });

            Assert.AreEqual(1, chatMsgLog.Count);
        }

        [Test]
        public void AddingMessagesSetsIsDirty()
        {
            Log<ChatMessage> chatMsgLog = new Log<ChatMessage>();
            Assert.IsFalse(chatMsgLog.IsDirty);

            chatMsgLog.Add(new ChatMessage() { ClientName = "zak", Message = "homo" });

            Assert.IsTrue(chatMsgLog.IsDirty);
        }

        [Test]
        public void BuildsDiffCorrectly()
        {
            Log<ChatMessage> chatMsgLog = new Log<ChatMessage>();

            chatMsgLog.Add(new ChatMessage() { ClientName = "1", Message = "beef" });
            chatMsgLog.Add(new ChatMessage() { ClientName = "2", Message = "stick" });

            Assert.AreEqual(2, chatMsgLog.GetDiff().Count);
            Assert.AreEqual("stick", chatMsgLog.GetDiff()[0].Message);
            Assert.AreEqual("beef", chatMsgLog.GetDiff()[1].Message);
        }

        [Test]
        public void CleanResultsInEmptyDiff()
        {
            Log<ChatMessage> chatMsgLog = new Log<ChatMessage>();
            chatMsgLog.Add(new ChatMessage() { ClientName = "1", Message = "beef" });
            chatMsgLog.Add(new ChatMessage() { ClientName = "2", Message = "stick" });

            chatMsgLog.Clean();

            Assert.AreEqual(0, chatMsgLog.GetDiff().Count);
        }

        [Test]
        public void LogReturnsMessagesWithNewestFirst()
        {
            Log<string> chatMsgLog = new Log<string>();
            chatMsgLog.Add("1");
            chatMsgLog.Add("2");
            chatMsgLog.Add("3");

            foreach (string message in chatMsgLog)
            {
                Assert.AreEqual("3", message);
                break;
            }
        }

        [Test]
        public void CanIterateThroughLogFromOldestToNewest()
        {
            Log<string> chatLog = new Log<string>();
            chatLog.Add("old");
            chatLog.Add("new");

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
            Log<string> chatMsgLog = new Log<string>();
            chatMsgLog.Add("1");
            chatMsgLog.Add("2");
            chatMsgLog.Add("3");

            Assert.AreEqual("3", chatMsgLog[0]);
            Assert.AreEqual("1", chatMsgLog[2]);
        }

        [Test]
        public void CanAccessElementsMoreThanOnce()
        {
            Log<string> chatMsgLog = new Log<string>();
            chatMsgLog.Add("hey");

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
            Log<string> msgLog = new Log<string>(testList);

            foreach (string msg in msgLog)
                Assert.AreEqual("hello", msg);
        }

        [Test]
        public void KeepsCount()
        {
            Log<string> chatMsgLog = new Log<string>();
            chatMsgLog.Add("1");
            chatMsgLog.Add("2");

            Assert.AreEqual(2, chatMsgLog.Count);
        }

        [Test]
        public void CanStripOldestMessage()
        {
            Log<string> log = new Log<string>();
            log.Add("old msg");
            log.Add("newer msg");

            Assert.AreEqual("old msg", log.StripOldestMessage());
            Assert.AreEqual("newer msg", log.StripOldestMessage());
        }
    }
}
