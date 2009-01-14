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
        public void UpdateCallsIntegrate()
        {
            var stubIntegrator = MockRepository.GenerateStub<IIntegrator>();
            Player player = new Player(1, stubIntegrator, MockRepository.GenerateStub<IBoundaryCollider>());

            player.Update();

            stubIntegrator.AssertWasCalled(x => x.Integrate(Arg<Vector2>.Is.Equal(player.Position)));
        }

        [Test]
        public void UpdateCallsMoveWithinBoundary()
        {
            var stubBoundaryCollider = MockRepository.GenerateStub<IBoundaryCollider>();
            Player player = new Player(1, MockRepository.GenerateStub<IIntegrator>(), stubBoundaryCollider);

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
    }
}
