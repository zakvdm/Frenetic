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
        IMediator stubMediator;

        [SetUp]
        public void SetUp()
        {
            stubMediator = MockRepository.GenerateStub<IMediator>();
            console = new CommandConsole(stubMediator, new Log<string>());
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

            stubMediator.AssertWasCalled(x => x.Set("command", "1"));
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

            stubMediator.AssertWasCalled(x => x.Set("command", "arg1 arg2 arg3"));
        }

        [Test]
        public void IgnoresInvalidInput()
        {
            string badinput = "/";

            console.ProcessInput(badinput);

            stubMediator.AssertWasNotCalled(x => x.Set(Arg<string>.Is.Anything, Arg<string>.Is.Anything));
            stubMediator.AssertWasNotCalled(x => x.Get(Arg<string>.Is.Anything));
        }

        // TryToCompleteCurrentInput:
        [Test]
        public void CompletesCommandWhenUnambiguousCompletionAvailable()
        {
            stubMediator.Stub(x => x.AvailableProperties).Return(new List<string>() { "Eat", "Die" });

            string completed = console.TryToCompleteInput("E");

            Assert.AreEqual("/Eat ", completed);
        }

        [Test]
        public void CompleteHandlesASlashInFront()
        {
            stubMediator.Stub(x => x.AvailableProperties).Return(new List<string>() { "Eat" });

            string completed = console.TryToCompleteInput("/E");

            Assert.AreEqual("/Eat ", completed);
        }
        [Test]
        public void CompleteIgnoresCaseVariations()
        {
            stubMediator.Stub(x => x.AvailableProperties).Return(new List<string>() { "EaT" });

            string completed = console.TryToCompleteInput("eAt");

            Assert.AreEqual("/EaT ", completed);
        }
        [Test]
        public void HandlesCurrentInputLongerThanCommandWithoutFailing()
        {
            stubMediator.Stub(x => x.AvailableProperties).Return(new List<string>() { "Eat" });

            string completed = console.TryToCompleteInput("Eat shit");

            Assert.AreEqual("/Eat shit", completed);
        }
        [Test]
        public void DoesAPartialCompleteWhenAppropriate()
        {
            stubMediator.Stub(x => x.AvailableProperties).Return(new List<string>() { "GiveX", "GiveY" });

            string completed = console.TryToCompleteInput("gi");

            Assert.AreEqual("/Give", completed);
        }

        [Test]
        public void HandlesANullListOfPossibleCompletions()
        {
            stubMediator.Stub(x => x.AvailableProperties).Return(new List<string>());

            string completed = console.TryToCompleteInput("a");

            Assert.AreEqual("a", completed);
        }

        // FindPossibleInputCompletions:
        [Test]
        public void ReturnsListOfAllPossibleCompletions()
        {
            stubMediator.Stub(x => x.AvailableProperties).Return(new List<string>() { "Eat", "Endear", "Die" });

            Log<string> possibleCommands = console.FindPossibleInputCompletions("E");

            Assert.AreEqual(2, possibleCommands.Count);
            Assert.IsTrue(possibleCommands.Exists((x) => x.ToString() == "Eat"));
            Assert.IsTrue(possibleCommands.Exists((x) => x.ToString() == "Endear"));
        }

        [Test]
        public void DoesntReturnAnyCompletionsWhenCurrentInputIsEmpty()
        {
            stubMediator.Stub(x => x.AvailableProperties).Return(new List<string>() { "Eat" });

            Log<string> possibleCommands = console.FindPossibleInputCompletions("");

            Assert.IsNull(possibleCommands);
        }
    }
}
