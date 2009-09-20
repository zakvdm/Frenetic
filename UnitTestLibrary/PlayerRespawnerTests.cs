using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Rhino.Mocks;
using Frenetic.Player;
using Microsoft.Xna.Framework;
using Frenetic.Gameplay.Level;

namespace UnitTestLibrary
{
    [TestFixture]
    public class PlayerRespawnerTests
    {
        [Test]
        public void RespawnsThePlayer()
        {
            var stubPlayer = MockRepository.GenerateStub<IPlayer>();
            stubPlayer.Status = PlayerStatus.Dead;
            stubPlayer.Position = new Vector2(100, 200);
            var respawner = new PlayerRespawner();

            respawner.RespawnPlayer(stubPlayer);

            Assert.AreEqual(PlayerStatus.Alive, stubPlayer.PendingStatus);
            Assert.AreEqual(new Vector2(400, 100), stubPlayer.Position);
        }

        [Test]
        public void ChecksPendingStatusBeforeRespawning()
        {
            var stubPlayer = MockRepository.GenerateStub<IPlayer>();
            stubPlayer.PendingStatus = PlayerStatus.Alive;
            stubPlayer.Position = Vector2.Zero;
            var respawner = new PlayerRespawner();

            respawner.RespawnPlayer(stubPlayer);

            Assert.AreEqual(PlayerStatus.Alive, stubPlayer.PendingStatus);
            Assert.AreEqual(Vector2.Zero, stubPlayer.Position); // NOTE: This assumes that the respawner won't respawn to Vector2.Zero... a bit iffy...
        }
    }
}
