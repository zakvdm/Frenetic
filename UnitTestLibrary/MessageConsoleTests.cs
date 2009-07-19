using System;
using NUnit.Framework;
using Frenetic;
using Rhino.Mocks;

namespace UnitTestLibrary
{
    [TestFixture]
    public class MessageConsoleTests
    {
        [Test]
        public void NewMessagesAreAddedToUnsortedMessages()
        {
            MessageConsole console = new MessageConsole(new Log<ChatMessage>(), new Log<ChatMessage>());
            
            console.ProcessInput("message1");
            console.ProcessInput("message2");

            Assert.AreEqual(0, console.Log.Count);
            int count = 0;
            foreach (ChatMessage msg in console.GetPendingMessagesFromAfter(1))
                count++;
            Assert.AreEqual(0, count);
            count = 0;
            foreach (ChatMessage msg in console.UnsortedMessages)
                count++;
            Assert.AreEqual(2, count);
        }

        [Test]
        public void CanSetSnapOnPendingMessages()
        {
            MessageConsole console = new MessageConsole(null, new Log<ChatMessage>());
            console.ProcessInput("msg");

            foreach (ChatMessage chatMsg in console.UnsortedMessages)
            {
                chatMsg.Snap = 12;
            }

            // Assert:
            foreach (ChatMessage chatMsg in console.GetPendingMessagesFromAfter(10))
            {
                Assert.AreEqual("msg", chatMsg.Message);
                Assert.AreEqual(12, chatMsg.Snap);
            }
        }

        [Test]
        public void CanIterateOverPendingMessages()
        {
            // Arrange:
            SnapCounter snapCounter = new SnapCounter();
            MessageConsole console = new MessageConsole(new Log<ChatMessage>(), new Log<ChatMessage>());
            console.ProcessInput("msg 1");
            console.ProcessInput("msg 2");
            console.ProcessInput("msg 3");
            foreach (ChatMessage msg in console.UnsortedMessages)
            {
                if (msg.Message == "msg 3")
                    msg.Snap = 7;
                if (msg.Message == "msg 2")
                    msg.Snap = 5;
                if (msg.Message == "msg 1")
                    msg.Snap = 3;
            }

            // Act/Assert:
            foreach (ChatMessage msg in console.GetPendingMessagesFromAfter(6))
            {
                Assert.AreEqual("msg 3", msg.Message);
            }
            int count = 0;
            foreach (ChatMessage msg in console.GetPendingMessagesFromAfter(3))
            {
                count++;
            }
            Assert.AreEqual(2, count);
        }
    }
}
