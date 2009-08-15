using System;
using Frenetic;
using NUnit.Framework;

namespace UnitTestLibrary
{
    [TestFixture]
    public class ChatMessageTests
    {
        [Test]
        public void TurnsToStringCorrectly()
        {
            ChatMessage chatMsg = new ChatMessage() { ClientName = "me", Message = "my message" };

            Assert.AreEqual("[me] my message", chatMsg.ToString());
        }

        [Test]
        public void CopyConstructorCopiesDeep()
        {
            ChatMessage msg1 = new ChatMessage() { ClientName = "terence", Message = "foo" };
            
            ChatMessage copy = new ChatMessage(msg1);
            msg1.ClientName = "zak";
            msg1.Message = "bar";

            Assert.AreEqual("terence", copy.ClientName);
            Assert.AreEqual("foo", copy.Message);
        }
    }
}
