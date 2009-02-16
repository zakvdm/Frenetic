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
        public void KeepsCommandsInCommandLog()
        {
            // COMMANDS need a prefixing "/"
            GameConsole console = new GameConsole(null);
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
            List<Command> commandList = new List<Command>();
            commandList.Add(new Command("Eat"));
            commandList.Add(new Command("Die"));
            GameConsole console = new GameConsole(commandList);
            console.CurrentInput = "E";

            console.TryToCompleteCurrentInput();

            Assert.AreEqual("/Eat", console.CurrentInput);
        }
        [Test]
        public void CompleteHandlesASlashInFront()
        {
            List<Command> commandList = new List<Command>();
            commandList.Add(new Command("Eat"));
            GameConsole console = new GameConsole(commandList);
            console.CurrentInput = "/E";

            console.TryToCompleteCurrentInput();

            Assert.AreEqual("/Eat", console.CurrentInput);
        }
        [Test]
        public void CompleteIgnoresCaseVariations()
        {
            List<Command> commandList = new List<Command>();
            commandList.Add(new Command("EaT"));
            GameConsole console = new GameConsole(commandList);
            console.CurrentInput = "eAt";

            console.TryToCompleteCurrentInput();

            Assert.AreEqual("/EaT", console.CurrentInput);
        }
        [Test]
        public void HandlesCurrentInputLongerThanCommandWithoutFailing()
        {
            List<Command> commandList = new List<Command>();
            commandList.Add(new Command("Eat"));
            GameConsole console = new GameConsole(commandList);
            console.CurrentInput = "Eat shit";

            console.TryToCompleteCurrentInput();

            Assert.AreEqual("/Eat shit", console.CurrentInput);
        }
        [Test]
        public void DoesAPartialCompleteWhenAppropriate()
        {
            List<Command> commandList = new List<Command>();
            commandList.Add(new Command("GiveX"));
            commandList.Add(new Command("GiveY"));
            GameConsole console = new GameConsole(commandList);
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
            List<Command> commandList = new List<Command>();
            commandList.Add(new Command("Eat"));
            commandList.Add(new Command("Endear"));
            commandList.Add(new Command("Die"));
            GameConsole console = new GameConsole(commandList);
            console.CurrentInput = "E";

            List<Command> possibleCommands = console.FindPossibleInputCompletions();

            Assert.AreEqual(2, possibleCommands.Count);
            Assert.IsTrue(possibleCommands.Exists((x) => x.ToString() == "Eat"));
            Assert.IsTrue(possibleCommands.Exists((x) => x.ToString() == "Endear"));
        }

        [Test]
        public void DoesntReturnAnyCompletionsWhenCurrentInputIsEmpty()
        {
            List<Command> commandList = new List<Command>();
            commandList.Add(new Command("Eat"));
            GameConsole console = new GameConsole(commandList);
            console.CurrentInput = "";

            List<Command> possibleCommands = console.FindPossibleInputCompletions();

            Assert.IsNull(possibleCommands);
        }

        [Test]
        public void FindPossibleCompletionsDoesntRemoveSlash()
        {
            List<Command> commandList = new List<Command>();
            commandList.Add(new Command("Eat"));
            GameConsole console = new GameConsole(commandList);
            console.CurrentInput = "/B";

            List<Command> possibleCommands = console.FindPossibleInputCompletions();

            Assert.AreEqual("/B", console.CurrentInput);
        }

        
    }
}
