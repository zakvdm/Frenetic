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
        [Test]
        public void CanBeActivatedAndDeactivated()
        {
            CommandConsole console = new CommandConsole(null, null);
            Assert.IsFalse(console.Active);

            console.Active = true;
            Assert.IsTrue(console.Active);
        }

        [Test]
        public void ForwardsCommandsToMediator()
        {
            var stubMediator = MockRepository.GenerateStub<IMediator>();
            CommandConsole console = new CommandConsole(stubMediator, new MessageLog());

            console.ProcessInput("/command 1");

            stubMediator.AssertWasCalled(x => x.Do("command", "1"));
        }

        [Test]
        public void ForwardsPropertyGetsToMediator()
        {
            var stubMediator = MockRepository.GenerateStub<IMediator>();
            CommandConsole console = new CommandConsole(stubMediator, new MessageLog());

            console.ProcessInput("/property");

            stubMediator.AssertWasCalled(x => x.Get("property"));
        }

        [Test]
        public void HandlesSpacesAndMultipleParameters()
        {
            var stubMediator = MockRepository.GenerateStub<IMediator>();
            CommandConsole console = new CommandConsole(stubMediator, new MessageLog());

            console.ProcessInput("/command  arg1   arg2 arg3");

            stubMediator.AssertWasCalled(x => x.Do("command", "arg1 arg2 arg3"));
        }

        [Test]
        public void KeepsCommandsInCommandLog()
        {
            // COMMANDS need a prefixing "/"
            CommandConsole console = new CommandConsole(MockRepository.GenerateStub<IMediator>(), new MessageLog());
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
            Mediator mediator = new Mediator();
            mediator.Register("Eat", (input) => "");
            mediator.Register("Die", (input) => "");
            CommandConsole console = new CommandConsole(mediator, new MessageLog());

            string completed = console.TryToCompleteInput("E");

            Assert.AreEqual("/Eat ", completed);
        }

        [Test]
        public void CompleteHandlesASlashInFront()
        {
            Mediator mediator = new Mediator();
            mediator.Register("Eat", (input) => "");
            CommandConsole console = new CommandConsole(mediator, new MessageLog());

            string completed = console.TryToCompleteInput("/E");

            Assert.AreEqual("/Eat ", completed);
        }
        [Test]
        public void CompleteIgnoresCaseVariations()
        {
            Mediator mediator = new Mediator();
            mediator.Register("EaT", (x) => "");
            CommandConsole console = new CommandConsole(mediator, new MessageLog());

            string completed = console.TryToCompleteInput("eAt");

            Assert.AreEqual("/EaT ", completed);
        }
        [Test]
        public void HandlesCurrentInputLongerThanCommandWithoutFailing()
        {
            Mediator mediator = new Mediator();
            mediator.Register("Eat", (x) => "");
            CommandConsole console = new CommandConsole(mediator, new MessageLog());

            string completed = console.TryToCompleteInput("Eat shit");

            Assert.AreEqual("/Eat shit", completed);
        }
        [Test]
        public void DoesAPartialCompleteWhenAppropriate()
        {
            Mediator mediator = new Mediator();
            mediator.Register("GiveX", (x) => "");
            mediator.Register("GiveY", (x) => "");
            CommandConsole console = new CommandConsole(mediator, new MessageLog());

            string completed = console.TryToCompleteInput("gi");

            Assert.AreEqual("/Give", completed);
        }

        [Test]
        public void HandlesANullListOfPossibleCompletions()
        {
            CommandConsole console = new CommandConsole(new Mediator(), new MessageLog());

            string completed = console.TryToCompleteInput("a");

            Assert.AreEqual("a", completed);
        }

        // FindPossibleInputCompletions:
        [Test]
        public void ReturnsListOfAllPossibleCompletions()
        {
            Mediator mediator = new Mediator();
            mediator.Register("Eat", (x) => "");
            mediator.Register("Endear", (x) => "");
            mediator.Register("Die", (x) => "");
            CommandConsole console = new CommandConsole(mediator, new MessageLog());

            MessageLog possibleCommands = console.FindPossibleInputCompletions("E");

            Assert.AreEqual(2, possibleCommands.Count);
            Assert.IsTrue(possibleCommands.Exists((x) => x.ToString() == "Eat"));
            Assert.IsTrue(possibleCommands.Exists((x) => x.ToString() == "Endear"));
        }

        [Test]
        public void DoesntReturnAnyCompletionsWhenCurrentInputIsEmpty()
        {
            Mediator mediator = new Mediator();
            mediator.Register("Eat", (x) => "");
            CommandConsole console = new CommandConsole(mediator, new MessageLog());

            MessageLog possibleCommands = console.FindPossibleInputCompletions("");

            Assert.IsNull(possibleCommands);
        }
    }
}
