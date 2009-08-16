using System;
using NUnit.Framework;
using Rhino.Mocks;
using Frenetic.Player;
using Microsoft.Xna.Framework;
using Frenetic.Weapons;
using System.Collections.Generic;
using Frenetic.Gameplay;

namespace UnitTestLibrary
{
    [TestFixture]
    public class PlayerPacketsTests
    {
        Player player;
        [SetUp]
        public void SetUp()
        {
            player = new Player(null, null, null, new RailGun(null), null);
        }

        [Test]
        public void CanSetPlayerStateValuesFromPlayerObject()
        {
            player.IsAlive = false;
            player.CurrentWeapon.Shots.Add(new Shot(new Vector2(1, 2), new Vector2(3, 4)));
            player.Position = Vector2.One;
            player.PlayerScore.Kills = 4;
            player.PlayerScore.Deaths = 100;

            PlayerState state = new PlayerState(player);

            Assert.IsFalse(state.IsAlive);
            Assert.AreEqual(Vector2.One, state.Position);
            Assert.AreEqual(1, state.NewShots.Count);
            Assert.AreEqual(new Shot(new Vector2(1, 2), new Vector2(3, 4)), state.NewShots[0]);
            Assert.AreEqual(4, state.Score.Kills);
            Assert.AreEqual(100, state.Score.Deaths);
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
            player.CurrentWeapon.Shots.Add(new Shot());
            PlayerState state = new PlayerState();
            state.IsAlive = true;
            state.Position = new Vector2(4, 8);
            state.NewShots = new List<Shot>();
            // Add 2 new Shot objects:
            state.NewShots.Add(new Shot(Vector2.One, Vector2.UnitX));
            state.NewShots.Add(new Shot(Vector2.UnitY, Vector2.Zero));
            state.Score = new PlayerScore() { Deaths = 3, Kills = 20 };

            state.RefreshPlayerValuesFromState(player, PlayerType.Network);

            Assert.IsTrue(state.IsAlive);
            Assert.AreEqual(new Vector2(4, 8), player.Position);
            Assert.AreEqual(3, player.CurrentWeapon.Shots.Count);
            Assert.AreEqual(new Shot(Vector2.One, Vector2.UnitX), player.CurrentWeapon.Shots[1]);
            Assert.AreEqual(new Shot(Vector2.UnitY, Vector2.Zero), player.CurrentWeapon.Shots[2]);
            Assert.AreEqual(3, player.PlayerScore.Deaths);
            Assert.AreEqual(20, player.PlayerScore.Kills);
        }
        [Test]
        public void DoesntRefreshPositionOfLocalPlayer()
        {
            PlayerState state = new PlayerState();
            state.IsAlive = false;
            state.Position = new Vector2(4, 8);
            state.NewShots = new List<Shot>();
            state.NewShots.Add(new Shot(Vector2.One, Vector2.UnitX));
            state.NewShots.Add(new Shot(Vector2.UnitY, Vector2.Zero));
            player.Position = Vector2.Zero;

            state.RefreshPlayerValuesFromState(player, PlayerType.Local);

            Assert.IsFalse(player.IsAlive);
            Assert.AreEqual(Vector2.Zero, player.Position);
            Assert.AreEqual(2, player.CurrentWeapon.Shots.Count);
            Assert.AreEqual(new Shot(Vector2.One, Vector2.UnitX), player.CurrentWeapon.Shots[0]);
            Assert.AreEqual(new Shot(Vector2.UnitY, Vector2.Zero), player.CurrentWeapon.Shots[1]);
        }
    }
}
