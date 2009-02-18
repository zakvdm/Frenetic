using System;
using NUnit.Framework;
using Frenetic;
using System.Collections.Generic;

namespace UnitTestLibrary
{
    [TestFixture]
    public class MediatorTests
    {
        [Test]
        public void CanGetProperties()
        {
            Mediator mediator = new Mediator();
            mediator.Commands.Add("TestPropertyGet", (parameter) => "worked!");

            Assert.AreEqual("worked!", mediator.Get("TestPropertyGet"));
        }

        [Test]
        public void CanGetAListOfPossibleCommands()
        {
            Mediator mediator = new Mediator();
            mediator.Commands.Add("Property1", null);
            mediator.Commands.Add("Method1", null);

            List<string> possibleCommands = mediator.AvailableCommands;

            Assert.AreEqual(2, possibleCommands.Count);
            Assert.IsTrue(possibleCommands.Exists((searchString) => searchString == "Property1"));
            Assert.IsTrue(possibleCommands.Exists((searchString) => searchString == "Method1"));
        }

        [Test]
        public void CanCallAMethod()
        {
            Mediator mediator = new Mediator();
            mediator.Commands.Add("TestMethod", (parameter) => parameter);

            Assert.AreEqual("boo", mediator.Do("TestMethod", "boo"));
        }

        [Test]
        public void ChecksIfCommandExists()
        {
            Mediator mediator = new Mediator();

            Assert.IsNull(mediator.Get("NonExistentProperty"));
            Assert.IsNull(mediator.Do("NonExistentMethod", "argument1"));
        }
    }
}
