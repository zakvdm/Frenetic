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
        IPlayerRespawner stubRespawner;
        PlayerUpdater updater;
        List<IPlayer> playerList;
        [SetUp]
        public void SetUp()
        {
            stubRespawner = MockRepository.GenerateStub<IPlayerRespawner>();
            playerList = new List<IPlayer>();
            updater = new PlayerUpdater(playerList, stubRespawner);
        }

        [Test]
        public void RespawnsDeadPlayersWhoShot()
        {
            var player1 = MockRepository.GenerateStub<IPlayer>();
            player1.IsAlive = false;
            player1.PendingShot = new Vector2();
            playerList.Add(player1);
            var player2 = MockRepository.GenerateStub<IPlayer>();
            player2.IsAlive = true;
            player2.PendingShot = new Vector2();
            playerList.Add(player2);

            updater.Process(123);

            stubRespawner.AssertWasCalled(me => me.RespawnPlayer(player1), e => e.Repeat.Once());
        }

        [Test]
        public void CallsShootOnEachAlivePlayer()
        {
            var player1 = MockRepository.GenerateStub<IPlayer>();
            player1.IsAlive = true;
            player1.PendingShot = new Vector2(6, 7);
            var player2 = MockRepository.GenerateStub<IPlayer>();
            player2.IsAlive = true;
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
            player.IsAlive = true;
            player.PendingShot = new Vector2(20, 30);
            playerList.Add(player);

            updater.Process(1);
            updater.Process(100);

            player.AssertWasCalled(me => me.Shoot(new Vector2(20, 30)), o => o.Repeat.Once());
        }
    }
}
