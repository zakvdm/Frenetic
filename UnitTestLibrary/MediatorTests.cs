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
            mediator.Register("TestPropertyGet", (parameter) => "worked!");

            Assert.AreEqual("worked!", mediator.Get("TestPropertyGet"));
        }

        [Test]
        public void CanGetAListOfPossibleCommands()
        {
            Mediator mediator = new Mediator();
            mediator.Register("Property1", null);
            mediator.Register("Method1", null);

            List<string> possibleCommands = mediator.AvailableCommands;

            Assert.AreEqual(2, possibleCommands.Count);
            Assert.IsTrue(possibleCommands.Exists((searchString) => searchString == "Property1"));
            Assert.IsTrue(possibleCommands.Exists((searchString) => searchString == "Method1"));
        }

        [Test]
        public void CanCallAMethod()
        {
            Mediator mediator = new Mediator();
            mediator.Register("TestMethod", (parameter) => parameter);

            Assert.AreEqual("boo", mediator.Do("TestMethod", "boo"));
        }

        [Test]
        public void ChecksIfCommandExists()
        {
            Mediator mediator = new Mediator();

            Assert.IsNull(mediator.Get("NonExistentProperty"));
            Assert.IsNull(mediator.Do("NonExistentMethod", "argument1"));
        }

        [Test]
        public void KeepsWeakReferencesToCommands()
        {
            Func<string, string> MethodDelegate = new Func<string,string>(Method);
            Mediator mediator = new Mediator();
            mediator.Register("Method", Method);

            Assert.AreEqual("works", mediator.Get("Method"));

            MethodDelegate = null;
            GC.Collect();

            Assert.IsNull(mediator.Get("Method"));
        }
        private string Method(string parameter)
        {
            return "works";
        }

        [Test]
        public void WeakReferenceWorksForLamdaExpressions()
        {
            // WORKS:
            Func<string, string> MethodDelegate = (input) => { if (this == null) return "broken"; else return "works"; };
            Mediator mediator = new Mediator();
            mediator.Register("Method", MethodDelegate);

            Assert.AreEqual("works", mediator.Get("Method"));

            MethodDelegate = null;
            GC.Collect();

            Assert.IsNull(mediator.Get("Method"));


            // NOW TRY THIS:
            MethodDelegate = (input) => { return "works"; };
            mediator.Register("Method", MethodDelegate);

            Assert.AreEqual("works", mediator.Get("Method"));

            MethodDelegate = null;
            GC.Collect();

            Assert.IsNull(mediator.Get("Method"));

            // NOTE THIS TEST DOESN'T WORK CURRENTLY BECAUSE OF THE LAMBDA EXPRESSION... IS THERE A WAY TO FIX THIS?
        }
    }
}
