using System;
using NUnit.Framework;
using Frenetic;

namespace UnitTestLibrary
{
    [TestFixture]
    public class MessageConsoleTests
    {
        [Test]
        public void MessagesAreAddedToInputLogNotNormalLog()
        {
            // everything not prefixed with "/" is a message
            MessageConsole console = new MessageConsole(new MessageLog(), new MessageLog());
            
            console.ProcessInput("message1");
            console.ProcessInput("message2");

            Assert.AreEqual(0, console.Log.Count);
        }

        [Test]
        public void HasNewMessagesWorks()
        {
            MessageConsole console = new MessageConsole(null, new MessageLog());

            console.ProcessInput("old msg");
            console.ProcessInput("new msg");

            Assert.IsTrue(console.HasNewMessages);

            Assert.AreEqual("old msg", console.GetNewMessage());
        }
    }
}
