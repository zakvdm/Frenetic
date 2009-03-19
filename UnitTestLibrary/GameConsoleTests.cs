using System;
using NUnit.Framework;
using Frenetic;
using System.Collections.Generic;
using Rhino.Mocks;

namespace UnitTestLibrary
{
    [TestFixture]
    public class GameConsoleTests
    {
        [Test]
        public void CanBeActivatedAndDeactivated()
        {
            GameConsole console = new GameConsole(null);
            Assert.IsFalse(console.Active);

            console.Active = true;
            Assert.IsTrue(console.Active);
        }

        [Test]
        public void ForwardsCommandsToMediator()
        {
            var stubMediator = MockRepository.GenerateStub<IMediator>();
            GameConsole console = new GameConsole(stubMediator);

            console.CurrentInput = "/command 1";
            console.ProcessInput();

            stubMediator.AssertWasCalled(x => x.Do("command", "1"));
        }

        [Test]
        public void ForwardsPropertyGetsToMediator()
        {
            var stubMediator = MockRepository.GenerateStub<IMediator>();
            GameConsole console = new GameConsole(stubMediator);

            console.CurrentInput = "/property";
            console.ProcessInput();

            stubMediator.AssertWasCalled(x => x.Get("property"));
        }

        [Test]
        public void HandlesSpacesAndMultipleParameters()
        {
            var stubMediator = MockRepository.GenerateStub<IMediator>();
            GameConsole console = new GameConsole(stubMediator);

            console.CurrentInput = "/command  arg1   arg2 arg3";
            console.ProcessInput();

            stubMediator.AssertWasCalled(x => x.Do("command", "arg1 arg2 arg3"));
        }

        [Test]
        public void KeepsCommandsInCommandLog()
        {
            // COMMANDS need a prefixing "/"
            GameConsole console = new GameConsole(MockRepository.GenerateStub<IMediator>());
            console.CurrentInput = "/command1";
            console.ProcessInput();
            console.CurrentInput = "/command2";
            console.ProcessInput();

            Assert.AreEqual(2, console.CommandLog.Count);
            Assert.AreEqual("command1", console.CommandLog[0]);
            Assert.AreEqual("command2", console.CommandLog[1]);
        }

        [Test]
        public void KeepsMessagesInMessageLog()
        {
            // everything not prefixed with "/" is a message
            GameConsole console = new GameConsole(null);
            console.CurrentInput = "message1";
            console.ProcessInput();
            console.CurrentInput = "message2";
            console.ProcessInput();

            Assert.AreEqual(2, console.MessageLog.Count);
            Assert.AreEqual("message1", console.MessageLog[0]);
            Assert.AreEqual("message2", console.MessageLog[1]);
        }

        // TryToCompleteCurrentInput:
        [Test]
        public void CompletesCommandWhenUnambiguousCompletionAvailable()
        {
            Mediator mediator = new Mediator();
            mediator.Register("Eat", (input) => "");
            mediator.Register("Die", (input) => "");
            GameConsole console = new GameConsole(mediator);
            console.CurrentInput = "E";

            console.TryToCompleteCurrentInput();

            Assert.AreEqual("/Eat ", console.CurrentInput);
        }

        [Test]
        public void CompleteHandlesASlashInFront()
        {
            Mediator mediator = new Mediator();
            mediator.Register("Eat", (input) => "");
            GameConsole console = new GameConsole(mediator);

            console.CurrentInput = "/E";

            console.TryToCompleteCurrentInput();

            Assert.AreEqual("/Eat ", console.CurrentInput);
        }
        [Test]
        public void CompleteIgnoresCaseVariations()
        {
            Mediator mediator = new Mediator();
            mediator.Register("EaT", (x) => "");
            GameConsole console = new GameConsole(mediator);
            console.CurrentInput = "eAt";

            console.TryToCompleteCurrentInput();

            Assert.AreEqual("/EaT ", console.CurrentInput);
        }
        [Test]
        public void HandlesCurrentInputLongerThanCommandWithoutFailing()
        {
            Mediator mediator = new Mediator();
            mediator.Register("Eat", (x) => "");
            GameConsole console = new GameConsole(mediator);
            console.CurrentInput = "Eat shit";

            console.TryToCompleteCurrentInput();

            Assert.AreEqual("/Eat shit", console.CurrentInput);
        }
        [Test]
        public void DoesAPartialCompleteWhenAppropriate()
        {
            Mediator mediator = new Mediator();
            mediator.Register("GiveX", (x) => "");
            mediator.Register("GiveY", (x) => "");
            GameConsole console = new GameConsole(mediator);
            console.CurrentInput = "gi";

            console.TryToCompleteCurrentInput();

            Assert.AreEqual("/Give", console.CurrentInput);
        }

        [Test]
        public void HandlesANullListOfPossibleCompletions()
        {
            GameConsole console = new GameConsole(null);

            console.TryToCompleteCurrentInput();

            Assert.AreEqual(0, console.CurrentInput.Length);
        }

        // FindPossibleInputCompletions:
        [Test]
        public void ReturnsListOfAllPossibleCompletions()
        {
            Mediator mediator = new Mediator();
            mediator.Register("Eat", (x) => "");
            mediator.Register("Endear", (x) => "");
            mediator.Register("Die", (x) => "");
            GameConsole console = new GameConsole(mediator);
            console.CurrentInput = "E";

            List<string> possibleCommands = console.FindPossibleInputCompletions();

            Assert.AreEqual(2, possibleCommands.Count);
            Assert.IsTrue(possibleCommands.Exists((x) => x.ToString() == "Eat"));
            Assert.IsTrue(possibleCommands.Exists((x) => x.ToString() == "Endear"));
        }

        [Test]
        public void DoesntReturnAnyCompletionsWhenCurrentInputIsEmpty()
        {
            Mediator mediator = new Mediator();
            mediator.Register("Eat", (x) => "");
            GameConsole console = new GameConsole(mediator);
            console.CurrentInput = "";

            List<string> possibleCommands = console.FindPossibleInputCompletions();

            Assert.IsNull(possibleCommands);
        }

        [Test]
        public void FindPossibleCompletionsDoesntRemoveSlash()
        {
            Mediator mediator = new Mediator();
            mediator.Register("Eat", (x) => "");
            GameConsole console = new GameConsole(mediator);
            console.CurrentInput = "/B";

            List<string> possibleCommands = console.FindPossibleInputCompletions();

            Assert.AreEqual("/B", console.CurrentInput);
        }

        
    }
}
