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
        public void ShouldAllowIterationOverPlayerList()
        {
            PlayerList playerList = new PlayerList() { MockRepository.GenerateStub<IPlayer>(), MockRepository.GenerateStub<IPlayer>() };
            int count = 0;

            foreach (var player in playerList)
            {
                count++;
            }

            Assert.AreEqual(2, count);
        }
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
