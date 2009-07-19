using System;
using NUnit.Framework;
using Frenetic.Player;
using Rhino.Mocks;
using Microsoft.Xna.Framework;
using Frenetic.Gameplay;
using System.Collections.Generic;

namespace UnitTestLibrary
{
    [TestFixture]
    public class PlayerUpdaterTests
    {
        PlayerUpdater updater;
        List<IPlayer> playerList;
        [SetUp]
        public void SetUp()
        {
            playerList = new List<IPlayer>();
            updater = new PlayerUpdater(playerList);
        }

        [Test]
        public void CallsShootOnEachPlayer()
        {
            var player1 = MockRepository.GenerateStub<IPlayer>();
            player1.PendingShot = new Vector2(6, 7);
            var player2 = MockRepository.GenerateStub<IPlayer>();
            player2.PendingShot = new Vector2(9, 10);
            playerList.Add(player1);
            playerList.Add(player2);

            updater.Process(1);

            player1.AssertWasCalled(me => me.Shoot(new Vector2(6, 7)));
            player2.AssertWasCalled(me => me.Shoot(new Vector2(9, 10)));
        }
        [Test]
        public void OnlyShootsOncePerPendingShot()
        {
            var player = MockRepository.GenerateStub<IPlayer>();
            player.PendingShot = new Vector2(20, 30);
            playerList.Add(player);

            updater.Process(1);
            updater.Process(100);

            player.AssertWasCalled(me => me.Shoot(new Vector2(20, 30)), o => o.Repeat.Once());
        }
    }
}
