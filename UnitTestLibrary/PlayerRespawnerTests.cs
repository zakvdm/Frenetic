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
            stubPlayer.IsAlive = false;
            stubPlayer.Position = new Vector2(100, 200);
            var respawner = new PlayerRespawner();

            respawner.RespawnPlayer(stubPlayer);

            Assert.IsTrue(stubPlayer.IsAlive);
            Assert.AreEqual(new Vector2(400, 100), stubPlayer.Position);
        }
    }
}
