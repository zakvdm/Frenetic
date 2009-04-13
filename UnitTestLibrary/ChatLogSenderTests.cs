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
    public class ChatLogSenderTests
    {
        IClientStateTracker stubClientStateTracker;
        IChatLogDiffer stubChatLogDiffer;
        ISnapCounter stubSnapCounter;
        IOutgoingMessageQueue stubOutgoingMessageQueue;
        ChatLogSender serverChatLogView;

        [SetUp]
        public void SetUp()
        {
            stubClientStateTracker = MockRepository.GenerateStub<IClientStateTracker>();
            List<Client> clientList = new List<Client>();
            stubClientStateTracker.Stub(x => x.CurrentClients).Return(clientList);
            stubChatLogDiffer = MockRepository.GenerateStub<IChatLogDiffer>();
            stubSnapCounter = MockRepository.GenerateStub<ISnapCounter>();
            stubOutgoingMessageQueue = MockRepository.GenerateStub<IOutgoingMessageQueue>();
            serverChatLogView = new ChatLogSender(stubChatLogDiffer, stubClientStateTracker, stubSnapCounter, stubOutgoingMessageQueue);
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
        public void GenerateSendsLastReceivedClientSnap()
        {
            stubSnapCounter.CurrentSnap = 3;
            Client client = new Client();
            client.LastClientSnap = 78;
            stubClientStateTracker.CurrentClients.Add(client);

            serverChatLogView.Generate();

            stubOutgoingMessageQueue.AssertWasCalled(x => x.WriteFor(Arg<Message>.Matches(y => y.Type == MessageType.ClientSnap && (int)y.Data == 78), Arg<Client>.Is.Equal(client)));
        }

        [Test]
        public void GenerateSendsCurrentSnapAndClientNameWithChatMessage()
        {
            Log<ChatMessage> chatLog = new Log<ChatMessage>();
            ChatMessage chatMsg = new ChatMessage() { ClientName = "terence", Snap = 32 };
            chatLog.AddMessage(chatMsg);
            Client client = new Client();
            stubClientStateTracker.CurrentClients.Add(client);
            stubChatLogDiffer.Stub(x => x.GetOldestToYoungestDiff(client)).Return(chatLog);
            stubSnapCounter.CurrentSnap = 3;

            serverChatLogView.Generate();

            stubOutgoingMessageQueue.AssertWasCalled(x => x.WriteFor(Arg<Message>.Matches(y => y.Type == MessageType.ChatLog && ((ChatMessage)y.Data).ClientName == "terence" && ((ChatMessage)y.Data).Snap == 32), Arg<Client>.Is.Equal(client)));
        }

        [Test]
        public void GenerateSendsChatMessageLogOneMessageAtATimeToClient()
        {
            Log<ChatMessage> chatLog = new Log<ChatMessage>();
            chatLog.AddMessage(new ChatMessage() { Message = "Woohoo" });
            chatLog.AddMessage(new ChatMessage() { Message = "boohoo" });
            Client client = new Client();
            stubClientStateTracker.CurrentClients.Add(client);
            stubChatLogDiffer.Stub(x => x.GetOldestToYoungestDiff(client)).Return(chatLog);
            stubSnapCounter.CurrentSnap = 3;

            serverChatLogView.Generate();

            stubOutgoingMessageQueue.AssertWasCalled(x => x.WriteFor(Arg<Message>.Matches(y => y.Type == MessageType.ChatLog && ((ChatMessage)y.Data).Message == "Woohoo"), Arg<Client>.Is.Equal(client)));
            stubOutgoingMessageQueue.AssertWasCalled(x => x.WriteFor(Arg<Message>.Matches(y => y.Type == MessageType.ChatLog && ((ChatMessage)y.Data).Message == "boohoo"), Arg<Client>.Is.Equal(client)));
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

            stubChatLogDiffer.AssertWasCalled(x => x.GetOldestToYoungestDiff(client1));
            stubChatLogDiffer.AssertWasCalled(x => x.GetOldestToYoungestDiff(client2));
        }

    }
}
