using System;
using NUnit.Framework;
using Rhino.Mocks;
using Frenetic;
using Frenetic.Network;
using Lidgren.Network;
using System.Collections.Generic;

namespace UnitTestLibrary
{
    [TestFixture]
    public class ServerChatLogViewTests
    {
        IClientStateTracker stubClientStateTracker;
        IChatLogDiffer stubChatLogDiffer;
        ISnapCounter stubSnapCounter;
        IOutgoingMessageQueue stubOutgoingMessageQueue;
        ServerChatLogView serverChatLogView;

        [SetUp]
        public void SetUp()
        {
            stubClientStateTracker = MockRepository.GenerateStub<IClientStateTracker>();
            List<Client> clientList = new List<Client>();
            stubClientStateTracker.Stub(x => x.CurrentClients).Return(clientList);
            stubChatLogDiffer = MockRepository.GenerateStub<IChatLogDiffer>();
            stubSnapCounter = MockRepository.GenerateStub<ISnapCounter>();
            stubOutgoingMessageQueue = MockRepository.GenerateStub<IOutgoingMessageQueue>();
            serverChatLogView = new ServerChatLogView(stubChatLogDiffer, stubClientStateTracker, stubSnapCounter, stubOutgoingMessageQueue);
        }

        [Test]
        public void GenerateSendsCurrentServerSnap()
        {
            stubSnapCounter.CurrentSnap = 3;
            Client client = new Client();
            stubClientStateTracker.CurrentClients.Add(client);

            serverChatLogView.Generate();

            stubOutgoingMessageQueue.AssertWasCalled(x => x.WriteFor(Arg<Message>.Matches(y => y.Type == MessageType.ServerSnap && (int)y.Data == 3), Arg<Client>.Is.Equal(client)));
        }

        [Test]
        public void GenerateSendsChatMessageLogOneMessageAtATimeToClient()
        {
            MessageLog chatLog = new MessageLog();
            chatLog.AddMessage("Woohoo");
            chatLog.AddMessage("boohoo");
            Client client = new Client();
            stubClientStateTracker.CurrentClients.Add(client);
            stubChatLogDiffer.Stub(x => x.Diff(client)).Return(chatLog);
            stubSnapCounter.CurrentSnap = 3;

            serverChatLogView.Generate();

            stubOutgoingMessageQueue.AssertWasCalled(x => x.WriteFor(Arg<Message>.Matches(y => y.Type == MessageType.ChatLog && (string)y.Data == "Woohoo"), Arg<Client>.Is.Equal(client)));
            stubOutgoingMessageQueue.AssertWasCalled(x => x.WriteFor(Arg<Message>.Matches(y => y.Type == MessageType.ChatLog && (string)y.Data == "boohoo"), Arg<Client>.Is.Equal(client)));
        }

        [Test]
        public void GetsADiffedLogForEachClient()
        {
            Client client1 = new Client() { ID = 1 };
            Client client2 = new Client() { ID = 2 };
            stubClientStateTracker.CurrentClients.Add(client1);
            stubClientStateTracker.CurrentClients.Add(client2);
            stubSnapCounter.CurrentSnap = 3;

            serverChatLogView.Generate();

            stubChatLogDiffer.AssertWasCalled(x => x.Diff(client1));
            stubChatLogDiffer.AssertWasCalled(x => x.Diff(client2));
        }

    }
}
