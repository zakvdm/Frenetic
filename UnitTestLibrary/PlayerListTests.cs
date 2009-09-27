using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Frenetic.Player;
using Rhino.Mocks;

namespace UnitTestLibrary
{
    [TestFixture]
    public class PlayerListTests
    {
        [Test]
        public void AddingPlayersToPlayerListRaisesPlayerJoinedEvent()
        {
            bool eventRaised = false;
            IPlayerList playerList = new PlayerList();
            IPlayer player = MockRepository.GenerateStub<IPlayer>();
            playerList.PlayerAdded += (newPlayer) => { if (newPlayer == player) eventRaised = true; };

            playerList.Add(player);

            Assert.IsTrue(eventRaised);
        }
    }
}
