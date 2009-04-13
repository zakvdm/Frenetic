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
            
            chatMsgLog.AddMessage(new ChatMessage() { ClientName = "Zak", Snap = 100, Message = "yo" });

            Assert.AreEqual(1, chatMsgLog.Count);
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

        /*
        [Test]
        public void BuildFromAnotherMessageLogWorks()
        {
            Log<string> sourceLog = new Log<string>();
            Log<string> destinationLog = new Log<string>();
            destinationLog.AddMessage("blah blah");
            sourceLog.AddMessage("profound statement");
            sourceLog.AddMessage("witty rejoinder");

            destinationLog.BuildFromAnotherMessageLog(sourceLog);

            Assert.AreEqual(2, destinationLog.Count);
            Assert.AreEqual("witty rejoinder", destinationLog[0]);
            Assert.AreEqual("profound statement", destinationLog[1]);
        }

        [Test]
        public void BuildFromAnotherMessageLogWorksForChatMessages()
        {
            Log<ChatMessage> sourceLog = new Log<ChatMessage>();
            Log<ChatMessage> destinationLog = new Log<ChatMessage>();
            ChatMessage tmpMsg = new ChatMessage() { Message = "pft" };
            destinationLog.AddMessage(new ChatMessage() { Message = "blah blah" });
            sourceLog.AddMessage(tmpMsg);

            destinationLog.BuildFromAnotherMessageLog(sourceLog);
            tmpMsg.Message = "tmp";

            Assert.AreEqual(1, destinationLog.Count);
            Assert.AreEqual("pft", destinationLog[0].Message);
        }
        */

        [Test]
        public void CanStripOldestMessage()
        {
            Log<string> log = new Log<string>();
            log.AddMessage("old msg");
            log.AddMessage("newer msg");

            Assert.AreEqual("old msg", log.StripOldestMessage());
            Assert.AreEqual("newer msg", log.StripOldestMessage());
        }

        /*
        [Test]
        public void CanCopy()
        {
            Log<string> original = new Log<string>();
            original.AddMessage("1");

            Log<string> copy = original.Copy();
            original.AddMessage("2");

            Assert.AreEqual(1, copy.Count);
            Assert.AreEqual("1", copy[0]);
        }

        [Test]
        public void CanCopyWithChatMessages()
        {
            Log<ChatMessage> original = new Log<ChatMessage>();
            ChatMessage chatMsg = new ChatMessage() { Message = "1" };
            original.AddMessage(chatMsg);

            Log<ChatMessage> copy = original.Copy();
            original.AddMessage(new ChatMessage() { Message = "2" });
            chatMsg.Message = "7";

            Assert.AreEqual(1, copy.Count);
            Assert.AreEqual("1", copy[0].Message);
        }
        */
    }
}
