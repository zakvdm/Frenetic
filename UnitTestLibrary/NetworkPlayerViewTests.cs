using System;
using System.IO;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;

using Frenetic;
using Lidgren.Network;

using NUnit.Framework;
using Rhino.Mocks;
using Frenetic.Network;

namespace UnitTestLibrary
{
    [TestFixture]
    public class NetworkPlayerViewTests
    {
        [Test]
        public void CanCreate()
        {
            var stubOutgoingMessageQueue = MockRepository.GenerateStub<IOutgoingMessageQueue>();
            NetworkPlayerView npView = new NetworkPlayerView(new Player(1, null, null), stubOutgoingMessageQueue);

            Assert.IsNotNull(npView);
        }

        [Test]
        public void GenerateCallsNetworkSessionSendNetChannelUnreliable()
        {
            var stubOutgoingMessageQueue = MockRepository.GenerateStub<IOutgoingMessageQueue>();
            NetworkPlayerView npView = new NetworkPlayerView(new Player(1, null, null), stubOutgoingMessageQueue);
            
            npView.Generate();

            stubOutgoingMessageQueue.AssertWasCalled(x => x.Write(Arg<Message>.Is.Anything, Arg<Lidgren.Network.NetChannel>.Is.Equal(Lidgren.Network.NetChannel.Unreliable)));
        }

        [Test]
        public void GenerateSendsPlayer()
        {
            var stubOutgoingMessageQueue = MockRepository.GenerateStub<IOutgoingMessageQueue>();
            Player player = new Player(1, null, null);
            player.Position = new Vector2(100, 200);
            NetworkPlayerView npView = new NetworkPlayerView(player, stubOutgoingMessageQueue);

            npView.Generate();

            stubOutgoingMessageQueue.AssertWasCalled(x => x.Write(Arg<Message>.Matches(y => y.Type == MessageType.PlayerData && y.Data == player), Arg<NetChannel>.Is.Anything));
        }

        [Test]
        public void GenerateDoesNothingIfPlayerNotYetConnectedToNetworkSession()
        {
            var stubOutgoingMessageQueue = MockRepository.GenerateStub<IOutgoingMessageQueue>();
            Player player = new Player(1, null, null);
            player.ID = 0;
            NetworkPlayerView npView = new NetworkPlayerView(player, stubOutgoingMessageQueue);

            npView.Generate();

            stubOutgoingMessageQueue.AssertWasNotCalled(x => x.Write(Arg<Message>.Matches(y => y.Type == MessageType.PlayerData && y.Data == player), Arg<NetChannel>.Is.Anything));
        }
    }
}
