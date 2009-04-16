using System;
using NUnit.Framework;
using Frenetic;
using Rhino.Mocks;
using Frenetic.Network;

namespace UnitTestLibrary
{
    [TestFixture]
    public class ChatLogDifferByReferenceTests
    {
        Log<ChatMessage> serverLog;
        ChatLogDifferByReference chatLogDiffer;
        [SetUp]
        public void SetUp()
        {
            serverLog = new Log<ChatMessage>();
            chatLogDiffer = new ChatLogDifferByReference(serverLog);
        }

        [Test]
        public void ReturnsCorrectDiff()
        {
            Client client = new Client(null, null);
            client.LastServerSnap = 13;
            serverLog.AddMessage(new ChatMessage() { Snap = 12, Message = "1" });
            serverLog.AddMessage(new ChatMessage() { Snap = 15, Message = "2" });
            serverLog.AddMessage(new ChatMessage() { Snap = 20, Message = "3" });

            Log<ChatMessage> diffedLog = chatLogDiffer.GetOldestToYoungestDiff(client);

            Assert.AreEqual(2, diffedLog.Count);
            Assert.AreEqual("2", diffedLog[0].Message);
            Assert.AreEqual("3", diffedLog[1].Message);
        }

        [Test]
        public void EmptyDiffReturnsNull()
        {
            Client client = new Client(null, null);
            client.LastServerSnap = 4;
            serverLog.AddMessage(new ChatMessage() { Snap = 2, Message = "msg" });

            Assert.IsNull(chatLogDiffer.GetOldestToYoungestDiff(client));
        }

        [Test]
        public void DiffWithAnEmptyServerChatLogIsNull()
        {
            Client client = new Client(null, null);
            client.LastServerSnap = 4;

            Assert.IsNull(chatLogDiffer.GetOldestToYoungestDiff(client));
        }

        [Test]
        public void IsNewClientChatMessageUsesSnapOnMessageToDecide()
        {
            ChatMessage testMsg = new ChatMessage() { ClientName = "zak", Snap = 7, Message = "hi" };

            Assert.IsTrue(chatLogDiffer.IsNewClientChatMessage(testMsg));   // It's new now, and gets added to the list of seen messages
            testMsg.Snap = 80;
            ChatMessage testMsg2 = new ChatMessage() { ClientName = "zak", Snap = 7, Message = "hi" };
            Assert.IsFalse(chatLogDiffer.IsNewClientChatMessage(testMsg2));  // Now it's no longer new...
        }
    }
}
