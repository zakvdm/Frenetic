using System;
using System.Xml.Serialization;
using System.IO;
using Microsoft.Xna.Framework;

using Frenetic;

using NUnit.Framework;

namespace UnitTestLibrary
{
    [TestFixture]
    public class PlayerTests
    {
        [Test]
        public void CanGetAndSetPlayerPosition()
        {
            Player player = new Player(1);
            player.Position = new Vector2(100, 200);

            Assert.AreEqual(1, player.ID);
            Assert.AreEqual(new Vector2(100, 200), player.Position);
        }

        [Test]
        public void CanSerialiseAndDeserialisePlayerPosition()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Player));
            Player player = new Player(1);
            player.Position = new Vector2(100, 200);
            MemoryStream stream = new MemoryStream();

            serializer.Serialize(stream, player);
            stream.Position = 0;
            Player rebuiltPlayer = (Player)serializer.Deserialize(stream);

            Assert.AreEqual(player.Position, rebuiltPlayer.Position);
        }
    }
}
