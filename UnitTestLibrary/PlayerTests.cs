using System;
using System.IO;
using Microsoft.Xna.Framework;

using Frenetic;
using Frenetic.Physics;

using NUnit.Framework;
using Rhino.Mocks;
using FarseerGames.FarseerPhysics.Dynamics;
using Frenetic.Player;
using Frenetic.Weapons;
using Frenetic.Engine;

namespace UnitTestLibrary
{
    [TestFixture]
    public class PlayerTests
    {
        Frenetic.Player.BasePlayer player;
        IPlayerSettings stubPlayerSettings;
        IPhysicsComponent stubPhysicsComponent;
        IBoundaryCollider stubBoundaryCollider;
        ITimer stubTimer;

        [SetUp]
        public void SetUp()
        {
            stubPlayerSettings = MockRepository.GenerateStub<IPlayerSettings>();
            stubPhysicsComponent = MockRepository.GenerateStub<IPhysicsComponent>();
            stubBoundaryCollider = MockRepository.GenerateStub<IBoundaryCollider>();
            stubTimer = MockRepository.GenerateStub<ITimer>();
            player = new BasePlayer(stubPlayerSettings, stubPhysicsComponent, stubBoundaryCollider, MockRepository.GenerateStub<IRailGun>(), stubTimer);
        }

        [Test]
        public void RegistersWithIPhysicsComponentForRelevantEvents()
        {
            stubPhysicsComponent.AssertWasCalled(me => me.CollidedWithWorld += Arg<Action>.Is.Anything);
            stubPhysicsComponent.AssertWasCalled(me => me.Shot += Arg<Action>.Is.Anything);
        }

        [Test]
        public void PositionImplementedInTermsOfPhysicsComponent()
        {
            stubPhysicsComponent.Position = new Vector2(100, 200);

            Assert.AreEqual(new Vector2(100, 200), player.Position);
        }

        [Test]
        public void UpdateCallsMoveWithinBoundary()
        {
            stubBoundaryCollider.Stub(x => x.MoveWithinBoundary(Arg<Vector2>.Is.Anything)).Return(player.Position);

            player.Update();

            stubBoundaryCollider.AssertWasCalled(x => x.MoveWithinBoundary(Arg<Vector2>.Is.Equal(player.Position)));
        }

        [Test]
        public void CanUpdatePlayerScore()
        {
            player.PlayerScore.Deaths += 3;
            player.PlayerScore.Kills += 1000;

            Assert.AreEqual(3, player.PlayerScore.Deaths);
            Assert.AreEqual(1000, player.PlayerScore.Kills);
        }

        // MOVEMENT:
        [Test]
        public void JumpAppliesTheJumpVectorAsAnImpulseToThePlayersBodyIfCanJumpIsTrue()
        {
            player.InContactWithLevel = true;

            player.Jump();

            stubPhysicsComponent.AssertWasCalled(pc => pc.ApplyImpulse(BasePlayer.JumpImpulse));
        }
        [Test]
        public void JumpAppliesNoImpulseToThePlayersBodyIfNotInContactWithTheLevel()
        {
            player.InContactWithLevel = false;

            player.Jump();

            stubPhysicsComponent.AssertWasNotCalled(pc => pc.ApplyImpulse(BasePlayer.JumpImpulse));
        }
        [Test]
        public void MoveLeftAppliesTheCorrectForceToThePlayersBodyWhenStationary()
        {
            player.MoveLeft();

            stubPhysicsComponent.AssertWasCalled(pc => pc.ApplyForce(BasePlayer.MoveForce));
        }
        [Test]
        public void MoveRightAppliesTheCorrectForceToThePlayersBodyWhenStationary()
        {
            player.MoveRight();

            stubPhysicsComponent.AssertWasCalled(pc => pc.ApplyForce(BasePlayer.MoveForce * -1));
        }
        [Test]
        public void PlayerVelocityIsCappedToTheLeft()
        {
            stubPhysicsComponent.LinearVelocity = new Vector2(-BasePlayer.MaxSpeed * 2, 0);

            player.MoveLeft();

            stubPhysicsComponent.AssertWasNotCalled(pc => pc.ApplyForce(BasePlayer.MoveForce));
        }
        [Test]
        public void PlayerVelocityIsCappedToTheRight()
        {
            stubPhysicsComponent.LinearVelocity = new Vector2(BasePlayer.MaxSpeed * 2, 0);

            player.MoveRight();

            stubPhysicsComponent.AssertWasNotCalled(pc => pc.ApplyForce(BasePlayer.MoveForce * -1));
        }

        // SHOOTING AND BEING SHOT:
        [Test]
        public void ShootCallsShootOnCurrentWeapon()
        {
            player.Position = new Vector2(10, 20);

            player.Shoot(new Vector2(30, 40));

            player.CurrentWeapon.AssertWasCalled(me => me.Shoot(new Vector2(10, 20), new Vector2(30, 40)));
        }
        [Test]
        public void GettingShotKillsThePlayer()
        {
            bool raisedOnDeath = false;
            Assert.IsTrue(player.IsAlive);
            player.Died += () => raisedOnDeath = !raisedOnDeath;

            stubPhysicsComponent.Raise(me => me.Shot += null);

            Assert.IsFalse(player.IsAlive);
            Assert.IsTrue(raisedOnDeath);
            Assert.AreEqual(1, player.PlayerScore.Deaths);
        }
        [Test]
        public void SetsRespawnTimerOnDead()
        {
            // At the moment just Shooting the player kills it...
            stubPhysicsComponent.Raise(me => me.Shot += null);

            stubTimer.AssertWasCalled(me => me.AddActionTimer(Arg<float>.Is.Anything, Arg<Action>.Is.Anything));
        }
    }
}
