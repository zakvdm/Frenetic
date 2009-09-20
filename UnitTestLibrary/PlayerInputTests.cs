using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Rhino.Mocks;
using Frenetic.Player;
using Microsoft.Xna.Framework;

namespace UnitTestLibrary
{
    [TestFixture]
    public class PlayerInputTests
    {
        [Test]
        public void CanCreatePlayerInputFromPlayerValues()
        {
            var player = MockRepository.GenerateStub<IPlayer>();
            player.Position = new Vector2(3, 4);
            player.PendingShot = new Vector2(8, 9);
            player.PendingStatus = PlayerStatus.Dead;

            var input = new PlayerInput(player);

            Assert.AreEqual(new Vector2(3, 4), input.Position);
            Assert.AreEqual(new Vector2(8, 9), input.PendingShot);
            Assert.AreEqual(PlayerStatus.Dead, input.PendingStatus);
        }
        
        [Test]
        public void UpdatesPlayerFromInputCorrectly()
        {
            var player = MockRepository.GenerateStub<IPlayer>();
            player.Status = PlayerStatus.Alive;
            var playerInput = new PlayerInput();
            playerInput.PendingStatus = PlayerStatus.Dead;
            playerInput.PendingShot = new Vector2(100, 200);
            playerInput.Position = new Vector2(1000, 2000);

            playerInput.RefreshPlayerValuesFromInput(player);

            Assert.AreEqual(PlayerStatus.Dead, player.PendingStatus);
            Assert.AreEqual(new Vector2(100, 200), player.PendingShot);
            Assert.AreEqual(new Vector2(1000, 2000), player.Position);
        }
    }
}
