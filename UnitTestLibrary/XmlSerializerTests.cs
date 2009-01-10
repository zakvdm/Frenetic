using System;
using System.Xml.Serialization;

using NUnit.Framework;

using System.IO;

using Microsoft.Xna.Framework;
using Frenetic;

namespace UnitTestLibrary
{
    [TestFixture]
    public class XmlSerializerTests
    {
        public class TestClass
        {
            private int testInt;
            public int TestInt
            {
                get { return testInt; }
                set { testInt = value; }
            }
        }
        [Test]
        public void SerializeTrivialClassTest()
        {
            TestClass player = new TestClass();

            player.TestInt = 5;

            XmlSerializer serializer = new XmlSerializer(typeof(TestClass));
            MemoryStream memoryStream = new MemoryStream();

            serializer.Serialize(memoryStream, player);

            memoryStream.Position = 0;
            TestClass playerRebuilt = serializer.Deserialize(memoryStream) as TestClass;

            Assert.AreEqual(5, playerRebuilt.TestInt);
        }

        [Test]
        public void SerializePlayerTest()
        {
            OldPlayer player = new OldPlayer();

            player.Position = new Vector2(300, 5);

            XmlSerializer serializer = new XmlSerializer(typeof(OldPlayer));
            MemoryStream memoryStream = new MemoryStream();

            serializer.Serialize(memoryStream, player);

            memoryStream.Position = 0;

            OldPlayer playerRebuilt = serializer.Deserialize(memoryStream) as OldPlayer;

            Assert.AreEqual(5, playerRebuilt.Position.Y);
            Assert.AreEqual(300, playerRebuilt.Position.X);
        }
    }
}
