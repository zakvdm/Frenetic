using System;
using System.IO;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;

using Frenetic;
using Lidgren.Network;

using NUnit.Framework;
using Rhino.Mocks;

namespace UnitTestLibrary
{
    [TestFixture]
    public class NetworkPlayerViewTests
    {
        [Test]
        public void CanCreate()
        {
            var stubNS = MockRepository.GenerateStub<INetworkSession>();
            NetworkPlayerView npView = new NetworkPlayerView(new Player(1, null, null), stubNS);

            Assert.IsNotNull(npView);
        }

        [Test]
        public void GenerateCallsNetworkSessionSendNetChannelUnreliable()
        {
            var stubNS = MockRepository.GenerateStub<INetworkSession>();
            NetworkPlayerView npView = new NetworkPlayerView(new Player(1, null, null), stubNS);
            stubNS.Stub(x => x.IsServer).Return(true);

            npView.Generate();

            stubNS.AssertWasCalled(x => x.SendToAll(Arg<Message>.Is.Anything, Arg<Lidgren.Network.NetChannel>.Is.Equal(Lidgren.Network.NetChannel.Unreliable)));
        }

        [Test]
        public void GenerateSendsPlayerToAllIfServer()
        {
            var stubNS = MockRepository.GenerateStub<INetworkSession>();
            Player player = new Player(1, null, null);
            player.Position = new Vector2(100, 200);
            NetworkPlayerView npView = new NetworkPlayerView(player, stubNS);

            stubNS.Stub(x => x.IsServer).Return(true);

            npView.Generate();

            stubNS.AssertWasCalled(x => x.SendToAll(Arg<Message>.Matches(y => y.Type == MessageType.PlayerData && y.Data == player), Arg<NetChannel>.Is.Anything));
        }

        [Test]
        public void GenerateSendsToServerIfClient()
        {
            var stubNS = MockRepository.GenerateStub<INetworkSession>();
            Player player = new Player(1, null, null);
            player.Position = new Vector2(100, 200);
            NetworkPlayerView npView = new NetworkPlayerView(player, stubNS);

            stubNS.Stub(x => x.IsServer).Return(false);

            npView.Generate();

            stubNS.AssertWasCalled(x => x.Send(Arg<Message>.Matches(y => y.Type == MessageType.PlayerData && y.Data == player), Arg<NetChannel>.Is.Anything));
        }
    }
}
