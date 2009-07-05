using System;
using NUnit.Framework;
using Rhino.Mocks;
using Frenetic.Player;
using Microsoft.Xna.Framework;
using Frenetic.Weapons;
using System.Collections.Generic;

namespace UnitTestLibrary
{
    [TestFixture]
    public class PlayerPacketsTests
    {
        [Test]
        public void CanSetPlayerStateValuesFromPlayerObject()
        {
            var player = new Player(null, null, new RailGun(null), null);
            player.IsAlive = false;
            player.CurrentWeapon.Shots.Add(new Shot(new Vector2(1, 2), new Vector2(3, 4)));
            player.Position = Vector2.One;

            PlayerState state = new PlayerState(player);

            Assert.IsFalse(state.IsAlive);
            Assert.AreEqual(Vector2.One, state.Position);
            Assert.AreEqual(1, state.Shots.Count);
            Assert.AreEqual(new Shot(new Vector2(1, 2), new Vector2(3, 4)), state.Shots[0]);
        }
        [Test]
        public void WorksWithNullPlayer()
        {
            PlayerState state = new PlayerState(null);

            Assert.IsNotNull(state);
        }

        [Test]
        public void CanRefreshNetworkPlayerObjectWithPlayerState()
        {
            PlayerState state = new PlayerState();
            state.IsAlive = true;
            state.Position = new Vector2(4, 8);
            state.Shots = new List<Shot>();
            state.Shots.Add(new Shot(Vector2.One, Vector2.UnitX));
            state.Shots.Add(new Shot(Vector2.UnitY, Vector2.Zero));
            var player = new Player(null, null, new RailGun(null), null);

            state.RefreshPlayerValuesFromState(player, PlayerType.Network);

            Assert.IsTrue(state.IsAlive);
            Assert.AreEqual(new Vector2(4, 8), player.Position);
            Assert.AreEqual(2, player.CurrentWeapon.Shots.Count);
            Assert.AreEqual(new Shot(Vector2.One, Vector2.UnitX), player.CurrentWeapon.Shots[0]);
            Assert.AreEqual(new Shot(Vector2.UnitY, Vector2.Zero), player.CurrentWeapon.Shots[1]);
        }
        [Test]
        public void DoesntRefreshPositionOfLocalPlayer()
        {
            PlayerState state = new PlayerState();
            state.IsAlive = false;
            state.Position = new Vector2(4, 8);
            state.Shots = new List<Shot>();
            state.Shots.Add(new Shot(Vector2.One, Vector2.UnitX));
            state.Shots.Add(new Shot(Vector2.UnitY, Vector2.Zero));
            var player = new Player(null, null, new RailGun(null), null) { Position = Vector2.Zero };

            state.RefreshPlayerValuesFromState(player, PlayerType.Local);

            Assert.IsFalse(player.IsAlive);
            Assert.AreEqual(Vector2.Zero, player.Position);
            Assert.AreEqual(2, player.CurrentWeapon.Shots.Count);
            Assert.AreEqual(new Shot(Vector2.One, Vector2.UnitX), player.CurrentWeapon.Shots[0]);
            Assert.AreEqual(new Shot(Vector2.UnitY, Vector2.Zero), player.CurrentWeapon.Shots[1]);
        }
    }
}
