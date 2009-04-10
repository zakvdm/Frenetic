using System;
using NUnit.Framework;
using Frenetic;
using Rhino.Mocks;
using Frenetic.Network;

namespace UnitTestLibrary
{
    [TestFixture]
    public class ChatLogArchiveTests
    {
        MessageLog serverChatLog;
        ISnapCounter stubSnapCounter;
        ChatLogArchive chatLogArchive;
        [SetUp]
        public void SetUp()
        {
            serverChatLog = new MessageLog();
            stubSnapCounter = MockRepository.GenerateStub<ISnapCounter>();
            chatLogArchive = new ChatLogArchive(serverChatLog, stubSnapCounter);
        }


        [Test]
        public void ArchivesChatLog()
        {
            Client client = new Client();
            client.LastServerSnap = 2;
            stubSnapCounter.CurrentSnap = 2;
            serverChatLog.AddMessage("msg 1");

            chatLogArchive.Process(1); // It should archive at snap 2 now

            Assert.AreEqual("msg 1", chatLogArchive[client][0]);
        }

        [Test]
        public void ArchivesChatLogForEverySnap()
        {
            stubSnapCounter.CurrentSnap = 2;
            serverChatLog.AddMessage("msg 1");
            chatLogArchive.Process(1); // Store at snap 1 & 2
            stubSnapCounter.CurrentSnap = 4;
            serverChatLog.AddMessage("msg 2");
            chatLogArchive.Process(1); // Store at snap 3 & 4

            Client client = new Client() { LastServerSnap = 1 };
            Assert.AreEqual("msg 1", chatLogArchive[client][0]);

            client.LastServerSnap = 4;
            Assert.AreEqual(2, chatLogArchive[client].Count);
            Assert.AreEqual("msg 2", chatLogArchive[client][0]);
            Assert.AreEqual("msg 1", chatLogArchive[client][1]);
        }

        [Test]
        public void ReturnsNullWhenNoArchivedLogExists()
        {
            Client client = new Client() { LastServerSnap = 10 };

            Assert.IsNull(chatLogArchive[client]);
        }
    }
}
