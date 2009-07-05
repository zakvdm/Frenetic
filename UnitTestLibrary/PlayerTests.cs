using System;
using System.Xml.Serialization;
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
        Frenetic.Player.Player player;
        IPhysicsComponent stubPhysicsComponent;
        IBoundaryCollider stubBoundaryCollider;
        ITimer stubTimer;

        [SetUp]
        public void SetUp()
        {
            stubPhysicsComponent = MockRepository.GenerateStub<IPhysicsComponent>();
            stubBoundaryCollider = MockRepository.GenerateStub<IBoundaryCollider>();
            stubTimer = MockRepository.GenerateStub<ITimer>();
            player = new Frenetic.Player.Player(stubPhysicsComponent, stubBoundaryCollider, MockRepository.GenerateStub<IRailGun>(), stubTimer);
        }

        [Test]
        public void RegistersWithIPhysicsComponentForRelevantEvents()
        {
            stubPhysicsComponent.AssertWasCalled(me => me.CollidedWithWorld += Arg<Action>.Is.Anything);
            stubPhysicsComponent.AssertWasCalled(me => me.OnShot += Arg<Action>.Is.Anything);
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

        // MOVEMENT:
        [Test]
        public void JumpAppliesTheJumpVectorAsAnImpulseToThePlayersBodyIfCanJumpIsTrue()
        {
            player.InContactWithLevel = true;

            player.Jump();

            stubPhysicsComponent.AssertWasCalled(pc => pc.ApplyImpulse(Player.JumpImpulse));
        }
        [Test]
        public void JumpAppliesNoImpulseToThePlayersBodyIfNotInContactWithTheLevel()
        {
            player.InContactWithLevel = false;

            player.Jump();

            stubPhysicsComponent.AssertWasNotCalled(pc => pc.ApplyImpulse(Player.JumpImpulse));
        }
        [Test]
        public void MoveLeftAppliesTheCorrectForceToThePlayersBodyWhenStationary()
        {
            player.MoveLeft();

            stubPhysicsComponent.AssertWasCalled(pc => pc.ApplyForce(Player.MoveForce));
        }
        [Test]
        public void MoveRightAppliesTheCorrectForceToThePlayersBodyWhenStationary()
        {
            player.MoveRight();

            stubPhysicsComponent.AssertWasCalled(pc => pc.ApplyForce(Player.MoveForce * -1));
        }
        [Test]
        public void PlayerVelocityIsCappedToTheLeft()
        {
            stubPhysicsComponent.Stub(spc => spc.LinearVelocity).Return(new Vector2(-50, 0));

            player.MoveLeft();

            stubPhysicsComponent.AssertWasNotCalled(pc => pc.ApplyForce(Player.MoveForce));
        }
        [Test]
        public void PlayerVelocityIsCappedToTheRight()
        {
            stubPhysicsComponent.Stub(spc => spc.LinearVelocity).Return(new Vector2(50, 0));

            player.MoveRight();

            stubPhysicsComponent.AssertWasNotCalled(pc => pc.ApplyForce(Player.MoveForce * -1));
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
            Assert.IsTrue(player.IsAlive);

            stubPhysicsComponent.Raise(me => me.OnShot += null);

            Assert.IsFalse(player.IsAlive);
        }
        [Test]
        public void SetsRespawnTimerOnDead()
        {
            // At the moment just Shooting the player kills it...
            stubPhysicsComponent.Raise(me => me.OnShot += null);

            stubTimer.AssertWasCalled(me => me.AddActionTimer(Arg<float>.Is.Anything, Arg<Action>.Is.Anything));
        }
    }
}
