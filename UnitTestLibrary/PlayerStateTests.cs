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
    public class PlayerStateTests
    {
        IPlayer player;
        [SetUp]
        public void SetUp()
        {
            player = MockRepository.GenerateStub<IPlayer>();
            player.Stub(me => me.CurrentWeapon).Return(MockRepository.GenerateStub<IRailGun>());
            player.CurrentWeapon.Stub(me => me.Shots).Return(new Shots());
            player.Stub(me => me.PlayerScore).Return(new PlayerScore());
        }

        [Test]
        public void CanSetPlayerStateValuesFromPlayerObject()
        {
            player.Status = PlayerStatus.Dead;
            player.CurrentWeapon.Shots.Add(new Shot(new Vector2(1, 2), new Vector2(3, 4)));
            player.Position = Vector2.One;
            player.PlayerScore.Kills = 4;
            player.PlayerScore.Deaths = 100;

            PlayerState state = new PlayerState(player);

            Assert.AreEqual(PlayerStatus.Dead, state.Status);
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
            state.Status = PlayerStatus.Alive;
            state.Position = new Vector2(4, 8);
            state.NewShots = new List<Shot>();
            // Add 2 new Shot objects:
            state.NewShots.Add(new Shot(Vector2.One, Vector2.UnitX));
            state.NewShots.Add(new Shot(Vector2.UnitY, Vector2.Zero));
            state.Score = new PlayerScore() { Deaths = 3, Kills = 20 };

            state.RefreshPlayerValuesFromState(player);

            Assert.AreEqual(PlayerStatus.Alive, state.Status);
            player.AssertWasCalled(me => me.UpdatePositionFromNetwork(Arg<Vector2>.Is.Equal(state.Position), Arg<float>.Is.Anything));
            Assert.AreEqual(3, player.CurrentWeapon.Shots.Count);
            Assert.AreEqual(new Shot(Vector2.One, Vector2.UnitX), player.CurrentWeapon.Shots[1]);
            Assert.AreEqual(new Shot(Vector2.UnitY, Vector2.Zero), player.CurrentWeapon.Shots[2]);
            Assert.AreEqual(3, player.PlayerScore.Deaths);
            Assert.AreEqual(20, player.PlayerScore.Kills);
        }
        [Test]
        public void ResetsPendingStateWhenItIsAchieved()
        {
            player.Status = PlayerStatus.Alive;
            player.PendingStatus = PlayerStatus.Dead;
            PlayerState state = new PlayerState();
            state.Status = PlayerStatus.Alive;

            state.RefreshPlayerValuesFromState(player);
            Assert.AreEqual(PlayerStatus.Dead, player.PendingStatus);

            state.Status = PlayerStatus.Dead;
            state.RefreshPlayerValuesFromState(player);
            Assert.IsNull(player.PendingStatus);
        }
    }
}
