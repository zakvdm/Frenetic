using System;
using NUnit.Framework;
using Frenetic.Player;
using Rhino.Mocks;
using Microsoft.Xna.Framework;

namespace UnitTestLibrary
{
    [TestFixture]
    public class PlayerUpdaterTests
    {
        PlayerUpdater updater;
        [SetUp]
        public void SetUp()
        {
            updater = new PlayerUpdater();
        }

        [Test]
        public void CanAddPlayers()
        {
            var player = MockRepository.GenerateStub<IPlayer>();

            updater.AddPlayer(player);

            Assert.AreEqual(player, updater.Players[0]);
        }
        [Test]
        public void CanRemovePlayer()
        {
            IPlayer player = MockRepository.GenerateStub<IPlayer>();
            updater.AddPlayer(player);

            updater.RemovePlayer(player);

            Assert.AreEqual(0, updater.Players.Count);
        }

        [Test]
        public void CallsShootOnEachPlayer()
        {
            var player1 = MockRepository.GenerateStub<IPlayer>();
            player1.PendingShot = new Vector2(6, 7);
            var player2 = MockRepository.GenerateStub<IPlayer>();
            player2.PendingShot = new Vector2(9, 10);
            updater.AddPlayer(player1);
            updater.AddPlayer(player2);

            updater.Process(1);

            player1.AssertWasCalled(me => me.Shoot(new Vector2(6, 7)));
            player2.AssertWasCalled(me => me.Shoot(new Vector2(9, 10)));
        }

        [Test]
        public void OnlyShootsOncePerPendingShot()
        {
            var player = MockRepository.GenerateStub<IPlayer>();
            player.PendingShot = new Vector2(20, 30);
            updater.AddPlayer(player);

            updater.Process(1);
            updater.Process(100);

            player.AssertWasCalled(me => me.Shoot(new Vector2(20, 30)), o => o.Repeat.Once());
        }
    }
}
