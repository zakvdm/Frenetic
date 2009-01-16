using System;
using System.Xml.Serialization;
using System.IO;
using Microsoft.Xna.Framework;

using Frenetic;
using Frenetic.Physics;

using NUnit.Framework;
using Rhino.Mocks;

namespace UnitTestLibrary
{
    [TestFixture]
    public class PlayerTests
    {
        [Test]
        public void RequiresAPhysicsComponent()
        {
            Player player = new Player(1, null, MockRepository.GenerateMock<IBoundaryCollider>());
            Assert.IsNotNull(player);
        }

        [Test]
        public void PositionImplementedInTermsOfPhysicsComponent()
        {
            var stubPhysicsComponent = MockRepository.GenerateStub<IPhysicsComponent>();
            stubPhysicsComponent.Position = new Vector2(100, 200);
            Player player = new Player(1, stubPhysicsComponent, MockRepository.GenerateMock<IBoundaryCollider>());

            Assert.AreEqual(new Vector2(100, 200), player.Position);
        }

        [Test]
        public void UpdateCallsMoveWithinBoundary()
        {
            var stubBoundaryCollider = MockRepository.GenerateStub<IBoundaryCollider>();
            Player player = new Player(1, MockRepository.GenerateStub<IPhysicsComponent>(), stubBoundaryCollider);

            player.Update();

            stubBoundaryCollider.AssertWasCalled(x => x.MoveWithinBoundary(Arg<Vector2>.Is.Equal(player.Position)));
        }

        [Test]
        public void CanSerialiseAndDeserialisePlayerPosition()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Player));
            Player player = new Player(1, null, null);
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
            var stubPhysicsComponent = MockRepository.GenerateStub<IPhysicsComponent>();
            Player player = new Player(1, stubPhysicsComponent, null);
            player.Position = new Vector2(100, 200);
            MemoryStream stream = new MemoryStream();

            serializer.Serialize(stream, player);
            stream.Position = 0;
            Player rebuiltPlayer = (Player)serializer.Deserialize(stream);
            rebuiltPlayer.Position = new Vector2(1, 2);

            // TODO: WHY THE HELL DOESN'T THIS WORK????
            //stubPhysicsComponent.AssertWasCalled(x => x.Position = new Vector2(100, 200));
            stubPhysicsComponent.AssertWasNotCalled(x => x.Position = new Vector2(1, 2));
        }
    }
}
