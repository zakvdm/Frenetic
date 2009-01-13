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
            Player player = new Player(1, stubIntegrator);
            Vector2 pos = new Vector2(100, 200);
            player.Position = pos;

            player.Update();

            stubIntegrator.AssertWasCalled(x => x.Integrate(Arg<Vector2>.Is.Equal(pos)));
        }

        [Test]
        public void CanSerialiseAndDeserialisePlayerPosition()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Player));
            Player player = new Player(1, null);
            player.Position = new Vector2(100, 200);
            MemoryStream stream = new MemoryStream();

            serializer.Serialize(stream, player);
            stream.Position = 0;
            Player rebuiltPlayer = (Player)serializer.Deserialize(stream);

            Assert.AreEqual(player.Position, rebuiltPlayer.Position);
        }
    }
}
