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

namespace UnitTestLibrary
{
    [TestFixture]
    public class PlayerTests
    {
        Frenetic.Player.Player player;
        IPhysicsComponent stubPhysicsComponent;
        IBoundaryCollider stubBoundaryCollider;

        [SetUp]
        public void SetUp()
        {
            stubPhysicsComponent = MockRepository.GenerateStub<IPhysicsComponent>();
            stubBoundaryCollider = MockRepository.GenerateStub<IBoundaryCollider>();
            player = new Frenetic.Player.Player(stubPhysicsComponent, stubBoundaryCollider);
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
        public void CanSerialiseAndDeserialisePlayerPosition()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Frenetic.Player.Player));
            player.Position = new Vector2(100, 200);
            MemoryStream stream = new MemoryStream();

            serializer.Serialize(stream, player);
            stream.Position = 0;
            Player rebuiltPlayer = (Player)serializer.Deserialize(stream);

            Assert.AreEqual(player.Position, rebuiltPlayer.Position);
        }

        [Test]
        public void SerializeAndDeserializeClearsInternalInstances()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Player));
            player.Position = new Vector2(100, 200);
            MemoryStream stream = new MemoryStream();

            serializer.Serialize(stream, player);
            stream.Position = 0;
            Player rebuiltPlayer = (Player)serializer.Deserialize(stream);
            rebuiltPlayer.Position = new Vector2(1, 2);

            stubPhysicsComponent.AssertWasNotCalled(x => x.Position = new Vector2(1, 2));
        }

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

        [Test]
        public void CanGiveThePlayerAWeapon()
        {
            RailGun railGun = new RailGun(null);
            player.AddWeapon(railGun);

            Assert.AreEqual(railGun, player.CurrentWeapon);
        }

        [Test]
        public void ShootCallsShootOnCurrentWeapon()
        {
            var stubRailGun = MockRepository.GenerateStub<IRailGun>();
            player.AddWeapon(stubRailGun);
            player.Position = new Vector2(10, 20);

            player.Shoot(new Vector2(30, 40));

            stubRailGun.AssertWasCalled(me => me.Shoot(new Vector2(10, 20), new Vector2(30, 40)));
        }

    }
}
