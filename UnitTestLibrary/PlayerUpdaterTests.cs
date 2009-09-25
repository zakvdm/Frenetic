using System;
using NUnit.Framework;
using Frenetic.Player;
using Rhino.Mocks;
using Microsoft.Xna.Framework;
using Frenetic.Gameplay.Level;
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
        public void UpdatesPlayerStatusBasedOnIncomingPendingStatus()
        {
            var player1 = MockRepository.GenerateStub<IPlayer>();
            player1.Status = PlayerStatus.Dead;
            player1.PendingStatus = PlayerStatus.Alive;
            playerList.Add(player1);
            var player2 = MockRepository.GenerateStub<IPlayer>();
            player2.Status = PlayerStatus.Dead;
            playerList.Add(player2);

            updater.Process(123);

            Assert.AreEqual(PlayerStatus.Alive, player1.Status);
            Assert.AreEqual(PlayerStatus.Dead, player2.Status);
        }
        [Test]
        public void ResetsPlayerHealthWhenRespawningPlayer()
        {
            var player = MockRepository.GenerateStub<IPlayer>();
            player.Status = PlayerStatus.Dead;
            player.PendingStatus = PlayerStatus.Alive;
            player.Health = 0;
            playerList.Add(player);

            updater.Process(1000);

            Assert.AreEqual(100, player.Health);
        }

        [Test]
        public void CallsShootOnEachAlivePlayer()
        {
            var player1 = MockRepository.GenerateStub<IPlayer>();
            player1.Status = PlayerStatus.Alive;
            player1.PendingShot = new Vector2(6, 7);
            var player2 = MockRepository.GenerateStub<IPlayer>();
            player2.Status = PlayerStatus.Alive;
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
            player.Status = PlayerStatus.Alive;
            player.PendingShot = new Vector2(20, 30);
            playerList.Add(player);

            updater.Process(1);
            updater.Process(100);

            player.AssertWasCalled(me => me.Shoot(new Vector2(20, 30)), o => o.Repeat.Once());
        }
    }
}
