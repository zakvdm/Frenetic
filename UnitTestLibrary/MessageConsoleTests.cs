using System;
using NUnit.Framework;
using Frenetic;
using Rhino.Mocks;

namespace UnitTestLibrary
{
    [TestFixture]
    public class MessageConsoleTests
    {
        [Test]
        public void NewChatsAreAddedToPendingLog()
        {
            MessageConsole console = new MessageConsole(new Log<ChatMessage>(), new Log<ChatMessage>());
            
            console.ProcessInput("message1");
            console.ProcessInput("message2");

            Assert.AreEqual(0, console.Log.Count);
            Assert.AreEqual(2, console.PendingLog.Count);
        }
    }
}
