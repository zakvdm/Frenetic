using System;
using NUnit.Framework;
using Frenetic;
using Rhino.Mocks;
using Frenetic.Network;

namespace UnitTestLibrary
{
    [TestFixture]
    public class ChatLogDifferTests
    {
        MessageLog serverLog;
        IChatLogArchive stubChatLogArchive;
        ChatLogDiffer chatLogDiffer;
        [SetUp]
        public void SetUp()
        {
            stubChatLogArchive = MockRepository.GenerateStub<IChatLogArchive>();
            serverLog = new MessageLog();
            chatLogDiffer = new ChatLogDiffer(serverLog, stubChatLogArchive);
        }

        [Test]
        public void GetsCorrectArchiveLog()
        {
            Client client = new Client();
            serverLog.AddMessage("token message");
            bool worked = false;
            stubChatLogArchive.Stub(x => x[client]).Return(null).WhenCalled(x => worked = true);

            chatLogDiffer.Diff(client);

            Assert.IsTrue(worked);
        }

        [Test]
        public void ReturnsCorrectDiff()
        {
            Client client = new Client();
            serverLog.AddMessage("1");
            serverLog.AddMessage("2");
            serverLog.AddMessage("3");
            MessageLog archiveLog = new MessageLog();
            archiveLog.AddMessage("1");
            stubChatLogArchive.Stub(x => x[client]).Return(archiveLog);

            MessageLog diffedLog = chatLogDiffer.Diff(client);

            Assert.AreEqual(2, diffedLog.Count);
            Assert.AreEqual("3", diffedLog[0]);
            Assert.AreEqual("2", diffedLog[1]);
        }

        [Test]
        public void DiffWithArchivedAndServerChatLogIdenticalIsNull()
        {
            Client client = new Client();
            serverLog.AddMessage("msg");
            MessageLog archiveLog = new MessageLog();
            archiveLog.AddMessage("msg");
            stubChatLogArchive.Stub(x => x[client]).Return(archiveLog);

            Assert.IsNull(chatLogDiffer.Diff(client));
        }

        [Test]
        public void CanHandleANullArchiveLog()
        {
            Client client = new Client();
            stubChatLogArchive.Stub(x => x[client]).Return(null);

            Assert.IsNull(chatLogDiffer.Diff(client));
        }

        [Test]
        public void DiffWithAnEmptyServerChatLogIsNull()
        {
            Client client = new Client();
            stubChatLogArchive.Stub(x => x[client]).Return(new MessageLog());

            Assert.IsNull(chatLogDiffer.Diff(client));
        }

        [Test]
        public void EmptyArchivedChatLogReturnsFullServerLog()
        {
            Client client = new Client();
            serverLog.AddMessage("1");
            serverLog.AddMessage("2");
            stubChatLogArchive.Stub(x => x[client]).Return(new MessageLog());

            Assert.AreEqual(2, chatLogDiffer.Diff(client).Count);
        }
    }
}
