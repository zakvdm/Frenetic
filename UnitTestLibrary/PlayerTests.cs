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
using System.Collections.Generic;
using Frenetic.Gameplay;

namespace UnitTestLibrary
{
    [TestFixture]
    public class PlayerTests
    {
        IPlayer player;
        IPlayerSettings stubPlayerSettings;
        IPhysicsComponent stubPhysicsComponent;
        IBoundaryCollider stubBoundaryCollider;
        IRailGun stubRailGun;
        ITimer stubTimer;

        [SetUp]
        public void SetUp()
        {
            stubPlayerSettings = MockRepository.GenerateStub<IPlayerSettings>();
            stubPhysicsComponent = MockRepository.GenerateStub<IPhysicsComponent>();
            stubBoundaryCollider = MockRepository.GenerateStub<IBoundaryCollider>();
            stubRailGun = MockRepository.GenerateStub<IRailGun>();
            stubTimer = MockRepository.GenerateStub<ITimer>();
            player = new BasePlayer(stubPlayerSettings, stubPhysicsComponent, stubBoundaryCollider, stubRailGun, stubTimer);
        }

        // SETUP:
        [Test]
        public void RegistersWithIPhysicsComponentForRelevantEvents()
        {
            stubPhysicsComponent.AssertWasCalled(me => me.CollidedWithWorld += Arg<Action>.Is.Anything);
            stubPhysicsComponent.AssertWasCalled(me => me.WasShot += Arg<Action<IPlayer, int>>.Is.Anything);
        }
        [Test]
        public void SetsCollisionGroup()
        {
            Assert.AreEqual(BasePlayer.CollisionGroup, stubPhysicsComponent.CollisionGroup);
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

        // NETWORK POSITION:
        [Test]
        public void LocalPlayerPositionNotUpdatedFromNetwork()
        {
            player = new LocalPlayer(stubPlayerSettings, stubPhysicsComponent, stubBoundaryCollider, stubRailGun, stubTimer);
            player.Position = new Vector2(100, 200);

            player.UpdatePositionFromNetwork(new Vector2(300, 400), 1f);
            
            Assert.AreEqual(new Vector2(100, 200), player.Position);
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
            player.CurrentWeapon.Stub(me => me.Shoot(Arg<Vector2>.Is.Anything, Arg<Vector2>.Is.Anything) ).Return(new List<IPhysicsComponent>());
            player.Position = new Vector2(10, 20);

            player.Shoot(new Vector2(30, 40));

            player.CurrentWeapon.AssertWasCalled(me => me.Shoot(new Vector2(10, 20), new Vector2(30, 40)));
        }
        [Test]
        public void NotifiesAllShotPhysicsComponents()
        {
            var stubPhysicsComponent1 = MockRepository.GenerateStub<IPhysicsComponent>();
            var stubPhysicsComponent2 = MockRepository.GenerateStub<IPhysicsComponent>();
            stubRailGun.Stub(me => me.Shoot(Arg<Vector2>.Is.Anything, Arg<Vector2>.Is.Anything)).Return(new List<IPhysicsComponent>() { stubPhysicsComponent1, stubPhysicsComponent2 });

            player.Shoot(new Vector2(100, 200));

            stubPhysicsComponent1.AssertWasCalled(me => me.OnWasShot(player, stubRailGun.Damage));
            stubPhysicsComponent2.AssertWasCalled(me => me.OnWasShot(player, stubRailGun.Damage));
        }
        [Test]
        public void GettingShotDamagesThePlayer()
        {
            var shootingPlayer = MockRepository.GenerateStub<IPlayer>();
            shootingPlayer.Stub(me => me.PlayerScore).Return(new PlayerScore());
            bool raisedOnDeath = false;
            Assert.AreEqual(PlayerStatus.Alive, player.Status);
            Assert.AreEqual(100, player.Health);
            player.Died += () => raisedOnDeath = !raisedOnDeath;

            stubPhysicsComponent.Raise(me => me.WasShot += null, shootingPlayer, 50);

            Assert.AreEqual(50, player.Health);

            stubPhysicsComponent.Raise(me => me.WasShot += null, shootingPlayer, 50);

            Assert.AreEqual(PlayerStatus.Dead, player.Status);
            Assert.IsTrue(raisedOnDeath);
            Assert.AreEqual(1, player.PlayerScore.Deaths);
            Assert.AreEqual(1, shootingPlayer.PlayerScore.Kills);
        }
    }
}
