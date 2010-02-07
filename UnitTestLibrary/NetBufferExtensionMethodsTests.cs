using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Frenetic.Network.Lidgren;
using Lidgren.Network;
using Frenetic.Player;
using Microsoft.Xna.Framework;
using Frenetic.Gameplay.Weapons;
using Frenetic.Gameplay;
using Frenetic.Network;

namespace UnitTestLibrary
{
    [TestFixture]
    public class NetBufferExtensionMethodsTests
    {
        NetBuffer buffer;
        [SetUp]
        public void SetUp()
        {
            buffer = new NetBuffer();
        }

        [Test]
        [ExpectedException(ExceptionType=typeof(NotImplementedException))]
        public void ThrowsExceptionWhenTryingToSerializeAnUnsoppertedType()
        {
            buffer.Write(new Message() { Items = new List<Item>() { new Item() { Type = ItemType.Event } } });
        }
        [Test]
        public void CanSerializeAndDeserializeAMessageWithManyItems()
        {
            var item1 = new Item() { Type = ItemType.Player, Data = new PlayerState() };
            var item2 = new Item() { Type = ItemType.PlayerSettings, Data = new NetworkPlayerSettings() };
            var item3 = new Item() { Type = ItemType.ChatLog, Data = new List<Frenetic.ChatMessage>() { new Frenetic.ChatMessage() { ClientName = "1", Message = "hello" } } };
            var msg = new Message() { Items = new List<Item>() { item1, item3, item1, item2 } };
            buffer.Write(msg);

            Console.WriteLine("Serializing Message with 2 x PlayerState, 1 x ChatMessage and PlayerSettings took " + buffer.Data.Length + " bytes");
            var output = buffer.ReadMessage();

            Assert.AreEqual(4, output.Items.Count);
        }

        [Test]
        public void CanSerializeAndDeserializeItemTypes()
        {
            buffer.Write(new Item() { ClientID = 20, Type = ItemType.Player, Data = new PlayerState() });
            Assert.AreEqual(typeof(PlayerState), buffer.ReadItem().Data.GetType());

            buffer.Write(new Item() { ClientID = 100, Type = ItemType.PlayerSettings, Data = new NetworkPlayerSettings() });
            Assert.AreEqual(typeof(NetworkPlayerSettings), buffer.ReadItem().Data.GetType());

            buffer.Write(new Item() { ClientID = 10, Type = ItemType.PlayerInput, Data = new PlayerInput() });
            Assert.AreEqual(typeof(PlayerInput), buffer.ReadItem().Data.GetType());
            
            buffer.Write(new Item() { ClientID = 10, Type = ItemType.ChatLog, Data = new List<Frenetic.ChatMessage>() });
            Assert.AreEqual(typeof(List<Frenetic.ChatMessage>), buffer.ReadItem().Data.GetType());
        }
        [Test]
        public void CanSerializeAndDeserializeThePlayerJoinAndDisconnectMessages()
        {
            buffer.Write(new Item() { Type = ItemType.NewClient, Data = 3 });
            buffer.Write(new Item() { Type = ItemType.SuccessfulJoin, Data = 4 });
            buffer.Write(new Item() { Type = ItemType.DisconnectingClient, Data = 500 });

            Assert.AreEqual(3, buffer.ReadItem().Data);
            Assert.AreEqual(4, buffer.ReadItem().Data);
            Assert.AreEqual(500, buffer.ReadItem().Data);
        }

        [Test]
        public void CanSerializeAndDeserializePlayerInput()
        {
            buffer.Write(new PlayerInput()
                            {
                                PendingStatus = null,
                                Position = new Vector2(100, 200),
                                PendingShot = new Vector2(300, 400)
                            }
                        );
            buffer.Write(new PlayerInput()
                            {
                                PendingStatus = PlayerStatus.Dead,
                                Position = new Vector2(500, 600),
                                PendingShot = null
                            }
                        );

            var output = buffer.ReadPlayerInput();

            Assert.IsNull(output.PendingStatus);
            Assert.AreEqual(new Vector2(100, 200), output.Position);
            Assert.AreEqual(new Vector2(300, 400), output.PendingShot);

            output = buffer.ReadPlayerInput();

            Assert.AreEqual(PlayerStatus.Dead, output.PendingStatus);
            Assert.IsNull(output.PendingShot);
        }

        [Test]
        public void CanSerializeAndDeserializePlayerState()
        {
            buffer.Write(new PlayerState() 
                            { 
                                Health = 96,
                                Status = PlayerStatus.Alive, 
                                Position = new Vector2(5, 10), 
                                NewShots = new List<Shot>() { new Shot() { EndPoint = Vector2.UnitX, StartPoint = Vector2.UnitY }, new Shot() { EndPoint = Vector2.Zero, StartPoint = Vector2.One} }, 
                                Score = new PlayerScore() { Deaths = 5, Kills = 1 } 
                            }
                        );

            Console.WriteLine("Serializing PlayerState took " + buffer.Data.Length + " bytes");
            var output = buffer.ReadPlayerState();

            Assert.AreEqual(96, output.Health);
            Assert.AreEqual(PlayerStatus.Alive, output.Status);
            Assert.AreEqual(new Vector2(5, 10), output.Position);
            Assert.AreEqual(2, output.NewShots.Count);
            Assert.AreEqual(new Shot() { EndPoint = Vector2.UnitX, StartPoint = Vector2.UnitY }, output.NewShots[0]);
            Assert.AreEqual(new Shot() { EndPoint = Vector2.Zero, StartPoint = Vector2.One }, output.NewShots[1]);
            Assert.AreEqual(5, output.Score.Deaths);
            Assert.AreEqual(1, output.Score.Kills);
        }

        [Test]
        public void CanSerializeAndDeserializePlayerSettings()
        {
            buffer.Write(new LocalPlayerSettings()
                            {
                                Color = Microsoft.Xna.Framework.Graphics.Color.DarkOliveGreen,
                                Name = "test",
                                Texture = Frenetic.Graphics.PlayerTexture.Blank
                            }
                        );

            Console.WriteLine("Serializing PlayerSettings took " + buffer.Data.Length + " bytes");
            var output = buffer.ReadPlayerSettings();

            Assert.AreEqual(Microsoft.Xna.Framework.Graphics.Color.DarkOliveGreen, output.Color);
            Assert.AreEqual("test", output.Name);
            Assert.AreEqual(Frenetic.Graphics.PlayerTexture.Blank, output.Texture);
        }

        [Test]
        public void CanSerializeAndDeserializeAChatLog()
        {
            buffer.Write(new List<Frenetic.ChatMessage>() { new Frenetic.ChatMessage() { ClientName = "1", Message = "hello" }, new Frenetic.ChatMessage() { ClientName = "2", Message = "hi"} });

            var output = buffer.ReadChatLog();

            Assert.AreEqual(2, output.Count);
            Assert.AreEqual("1", output[0].ClientName);
            Assert.AreEqual("hi", output[1].Message);
        }
    }
}
