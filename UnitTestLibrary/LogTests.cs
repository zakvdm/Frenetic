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
        public void CanBuildALogOfChatMessages()
        {
            Log<ChatMessage> chatMsgLog = new Log<ChatMessage>();
            
            chatMsgLog.AddMessage(new ChatMessage() { ClientName = "Zak", Message = "yo" });

            Assert.AreEqual(1, chatMsgLog.Count);
        }

        [Test]
        public void AddingMessagesSetsIsDirty()
        {
            Log<ChatMessage> chatMsgLog = new Log<ChatMessage>();
            Assert.IsFalse(chatMsgLog.IsDirty);

            chatMsgLog.AddMessage(new ChatMessage() { ClientName = "zak", Message = "homo" });

            Assert.IsTrue(chatMsgLog.IsDirty);
        }

        [Test]
        public void BuildsDiffCorrectly()
        {
            Log<ChatMessage> chatMsgLog = new Log<ChatMessage>();

            chatMsgLog.AddMessage(new ChatMessage() { ClientName = "1", Message = "beef" });
            chatMsgLog.AddMessage(new ChatMessage() { ClientName = "2", Message = "stick" });

            Assert.AreEqual(2, chatMsgLog.GetDiff().Count);
            Assert.AreEqual("stick", chatMsgLog.GetDiff()[0].Message);
            Assert.AreEqual("beef", chatMsgLog.GetDiff()[1].Message);
        }

        [Test]
        public void CleanResultsInEmptyDiff()
        {
            Log<ChatMessage> chatMsgLog = new Log<ChatMessage>();
            chatMsgLog.AddMessage(new ChatMessage() { ClientName = "1", Message = "beef" });
            chatMsgLog.AddMessage(new ChatMessage() { ClientName = "2", Message = "stick" });

            chatMsgLog.Clean();

            Assert.AreEqual(0, chatMsgLog.GetDiff().Count);
        }

        [Test]
        public void LogReturnsMessagesWithNewestFirst()
        {
            Log<string> chatMsgLog = new Log<string>();
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
            Log<string> chatLog = new Log<string>();
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
            Log<string> chatMsgLog = new Log<string>();
            chatMsgLog.AddMessage("1");
            chatMsgLog.AddMessage("2");
            chatMsgLog.AddMessage("3");

            Assert.AreEqual("3", chatMsgLog[0]);
            Assert.AreEqual("1", chatMsgLog[2]);
        }

        [Test]
        public void CanAccessElementsMoreThanOnce()
        {
            Log<string> chatMsgLog = new Log<string>();
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
            Log<string> msgLog = new Log<string>(testList);

            foreach (string msg in msgLog)
                Assert.AreEqual("hello", msg);
        }

        [Test]
        public void KeepsCount()
        {
            Log<string> chatMsgLog = new Log<string>();
            chatMsgLog.AddMessage("1");
            chatMsgLog.AddMessage("2");

            Assert.AreEqual(2, chatMsgLog.Count);
        }

        [Test]
        public void CanStripOldestMessage()
        {
            Log<string> log = new Log<string>();
            log.AddMessage("old msg");
            log.AddMessage("newer msg");

            Assert.AreEqual("old msg", log.StripOldestMessage());
            Assert.AreEqual("newer msg", log.StripOldestMessage());
        }
    }
}
