using System;
using NUnit.Framework;
using Frenetic;
using System.Collections.Generic;
using Rhino.Mocks;
using Frenetic.UserInput;

namespace UnitTestLibrary
{
    [TestFixture]
    public class CommandConsoleTests
    {
        CommandConsole console;
        CommandConsole consoleWithMediator;
        IMediator mediator;
        IMediator stubMediator;

        [SetUp]
        public void SetUp()
        {
            stubMediator = MockRepository.GenerateStub<IMediator>();
            mediator = new Mediator();
            console = new CommandConsole(stubMediator, new Log<string>());
            consoleWithMediator = new CommandConsole(mediator, new Log<string>());
        }

        [Test]
        public void CanBeActivatedAndDeactivated()
        {
            Assert.IsFalse(console.Active);

            console.Active = true;

            Assert.IsTrue(console.Active);
        }

        [Test]
        public void ForwardsCommandsToMediator()
        {
            console.ProcessInput("/command 1");

            stubMediator.AssertWasCalled(x => x.Do("command", "1"));
        }

        [Test]
        public void ForwardsPropertyGetsToMediator()
        {
            console.ProcessInput("/property");

            stubMediator.AssertWasCalled(x => x.Get("property"));
        }

        [Test]
        public void HandlesSpacesAndMultipleParameters()
        {
            console.ProcessInput("/command  arg1   arg2 arg3");

            stubMediator.AssertWasCalled(x => x.Do("command", "arg1 arg2 arg3"));
        }

        [Test]
        public void KeepsCommandsInCommandLog()
        {
            // COMMANDS need a prefixing "/"
            console.ProcessInput("/command1");
            console.ProcessInput("/command2");

            Assert.AreEqual(2, console.Log.Count);
            Assert.AreEqual("command1", console.Log[1]);
            Assert.AreEqual("command2", console.Log[0]);
        }

        // TryToCompleteCurrentInput:
        [Test]
        public void CompletesCommandWhenUnambiguousCompletionAvailable()
        {
            mediator.Register("Eat", (input) => "");
            mediator.Register("Die", (input) => "");

            string completed = consoleWithMediator.TryToCompleteInput("E");

            Assert.AreEqual("/Eat ", completed);
        }

        [Test]
        public void CompleteHandlesASlashInFront()
        {
            mediator.Register("Eat", (input) => "");

            string completed = consoleWithMediator.TryToCompleteInput("/E");

            Assert.AreEqual("/Eat ", completed);
        }
        [Test]
        public void CompleteIgnoresCaseVariations()
        {
            mediator.Register("EaT", (x) => "");

            string completed = consoleWithMediator.TryToCompleteInput("eAt");

            Assert.AreEqual("/EaT ", completed);
        }
        [Test]
        public void HandlesCurrentInputLongerThanCommandWithoutFailing()
        {
            mediator.Register("Eat", (x) => "");

            string completed = consoleWithMediator.TryToCompleteInput("Eat shit");

            Assert.AreEqual("/Eat shit", completed);
        }
        [Test]
        public void DoesAPartialCompleteWhenAppropriate()
        {
            mediator.Register("GiveX", (x) => "");
            mediator.Register("GiveY", (x) => "");

            string completed = consoleWithMediator.TryToCompleteInput("gi");

            Assert.AreEqual("/Give", completed);
        }

        [Test]
        public void HandlesANullListOfPossibleCompletions()
        {
            string completed = consoleWithMediator.TryToCompleteInput("a");

            Assert.AreEqual("a", completed);
        }

        // FindPossibleInputCompletions:
        [Test]
        public void ReturnsListOfAllPossibleCompletions()
        {
            mediator.Register("Eat", (x) => "");
            mediator.Register("Endear", (x) => "");
            mediator.Register("Die", (x) => "");

            Log<string> possibleCommands = consoleWithMediator.FindPossibleInputCompletions("E");

            Assert.AreEqual(2, possibleCommands.Count);
            Assert.IsTrue(possibleCommands.Exists((x) => x.ToString() == "Eat"));
            Assert.IsTrue(possibleCommands.Exists((x) => x.ToString() == "Endear"));
        }

        [Test]
        public void DoesntReturnAnyCompletionsWhenCurrentInputIsEmpty()
        {
            mediator.Register("Eat", (x) => "");

            Log<string> possibleCommands = consoleWithMediator.FindPossibleInputCompletions("");

            Assert.IsNull(possibleCommands);
        }
    }
}
