using System;
using NUnit.Framework;
using Frenetic;
using Frenetic.Network;
using Rhino.Mocks;
using Lidgren.Network;

namespace UnitTestLibrary
{
    [TestFixture]
    public class ClientInputSenderTests
    {
        [Test]
        public void DoesNothingWhenClientIDIsZero()
        {
            var mockOutgoingMessageQueue = MockRepository.GenerateMock<IOutgoingMessageQueue>();
            Client client = new Client() { ID = 0 };
            ClientInputSender clientInputSender = new ClientInputSender(client, null, mockOutgoingMessageQueue);
            mockOutgoingMessageQueue.Expect(x => x.Write(Arg<Message>.Is.Anything)).Repeat.Never();

            clientInputSender.Generate();

            mockOutgoingMessageQueue.VerifyAllExpectations();
        }

        [Test]
        public void SendsLastReceivedSnap()
        {
            var stubOutgoingMessageQueue = MockRepository.GenerateStub<IOutgoingMessageQueue>();
            Client client = new Client() { ID = 9, LastServerSnap = 33 };
            ClientInputSender clientInputSender = new ClientInputSender(client, MockRepository.GenerateStub<IMessageConsole>(), stubOutgoingMessageQueue);

            clientInputSender.Generate();

            stubOutgoingMessageQueue.AssertWasCalled(x => x.Write(Arg<Message>.Matches(y => y.ClientID == 9 && y.Type == MessageType.ServerSnap && (int)y.Data == 33)));
        }

        [Test]
        public void SendsChatLogMessages()
        {
            MessageConsole console = new MessageConsole(null, new MessageLog());
            var stubOutgoingMessageQueue = MockRepository.GenerateStub<IOutgoingMessageQueue>();
            Client client = new Client() { LastServerSnap = 20, ID = 5 };
            ClientInputSender clientInputSender = new ClientInputSender(client, console, stubOutgoingMessageQueue);
            console.ProcessInput("new message 1");
            console.ProcessInput("new message 2");

            clientInputSender.Generate();

            stubOutgoingMessageQueue.AssertWasCalled(x => x.Write(Arg<Message>.Matches(y => y.ClientID == 5 && y.Type == MessageType.ChatLog && (string)y.Data == "new message 1")));
            stubOutgoingMessageQueue.AssertWasCalled(x => x.Write(Arg<Message>.Matches(y => y.ClientID == 5 && y.Type == MessageType.ChatLog && (string)y.Data == "new message 2")));
        }
    }
}
