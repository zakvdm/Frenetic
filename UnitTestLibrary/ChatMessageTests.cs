using System;
using Frenetic;
using NUnit.Framework;

namespace UnitTestLibrary
{
    [TestFixture]
    public class ChatMessageTests
    {
        [Test]
        public void ImplementsEqualsAndNotEqualsOperator()
        {
            ChatMessage chatMsg1 = new ChatMessage() { ClientName = "zak", Snap = 12, Message = "hey" };
            ChatMessage chatMsg2 = new ChatMessage() { ClientName = "zak", Snap = 12, Message = "hey" };

            Assert.IsFalse(chatMsg1 != chatMsg2);
            Assert.IsTrue(chatMsg1 == chatMsg2);

            chatMsg1.ClientName = "notzak";

            Assert.IsTrue(chatMsg1 != chatMsg2);
            Assert.IsFalse(chatMsg1 == chatMsg2);

            chatMsg1.ClientName = "zak";
            chatMsg2.Snap = 13;

            Assert.IsTrue(chatMsg1 != chatMsg2);
            Assert.IsFalse(chatMsg1 == chatMsg2);

            chatMsg2.Snap = 12;
            chatMsg1.Message = "nothey";

            Assert.IsTrue(chatMsg1 != chatMsg2);
            Assert.IsFalse(chatMsg1 == chatMsg2);
        }

        [Test]
        public void TurnsToStringCorrectly()
        {
            ChatMessage chatMsg = new ChatMessage() { ClientName = "me", Snap = 4, Message = "my message" };

            Assert.AreEqual("[me] my message", chatMsg.ToString());
        }

        [Test]
        public void EqualsWorksCorrectly()
        {
            ChatMessage chatMsg1 = new ChatMessage() { ClientName = "zak", Snap = 1, Message = "yay!" };
            ChatMessage chatMsg2 = chatMsg1;

            Assert.IsTrue(chatMsg1.Equals(chatMsg2));

            chatMsg2 = new ChatMessage() { ClientName = "zak", Snap = 1, Message = "yay!" };

            Assert.IsFalse(chatMsg1.Equals(chatMsg2)); // It's a reference equals...
        }

        [Test]
        public void CopyConstructorCopiesDeep()
        {
            ChatMessage msg1 = new ChatMessage() { ClientName = "terence", Snap = 100, Message = "foo" };
            
            ChatMessage copy = new ChatMessage(msg1);
            msg1.ClientName = "zak";
            msg1.Snap = 200;
            msg1.Message = "bar";

            Assert.AreEqual("terence", copy.ClientName);
            Assert.AreEqual(100, copy.Snap);
            Assert.AreEqual("foo", copy.Message);
        }
    }
}
