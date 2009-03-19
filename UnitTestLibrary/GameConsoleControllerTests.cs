using System;
using NUnit.Framework;
using Frenetic;
using Rhino.Mocks;
using Microsoft.Xna.Framework.Input;
using Frenetic.UserInput;

namespace UnitTestLibrary
{
    [TestFixture]
    public class GameConsoleControllerTests
    {
        [Test]
        public void TildeActivatesConsole()
        {
            GameConsole console = new GameConsole(null);
            var stubKeyboard = MockRepository.GenerateStub<IKeyboard>();
            GameConsoleController consoleController = new GameConsoleController(console, stubKeyboard);

            Assert.IsFalse(console.Active);
            stubKeyboard.Stub(x => x.IsKeyDown(Keys.OemTilde)).Return(true);

            consoleController.Process(1);

            Assert.IsTrue(console.Active);
        }

        [Test]
        public void TildeTogglesConsoleOnAndOff()
        {
            GameConsole console = new GameConsole(null);
            var stubKeyboard = MockRepository.GenerateStub<IKeyboard>();
            GameConsoleController consoleController = new GameConsoleController(console, stubKeyboard);

            stubKeyboard.Stub(x => x.IsKeyDown(Keys.OemTilde)).Return(true);

            consoleController.Process(1);   // ACTIVE
            consoleController.Process(1);   // Back to INACTIVE

            Assert.IsFalse(console.Active);
        }

        [Test]
        public void TildeKeyMustBeReleasedAndRepressedToToggleConsole()
        {
            GameConsole console = new GameConsole(null);
            var stubKeyboard = MockRepository.GenerateStub<IKeyboard>();
            GameConsoleController consoleController = new GameConsoleController(console, stubKeyboard);

            stubKeyboard.Stub(x => x.IsKeyDown(Keys.OemTilde)).Return(true);
            stubKeyboard.Stub(x => x.WasKeyDown(Keys.OemTilde)).Return(true);

            consoleController.Process(1); 
            Assert.IsFalse(console.Active);
        }

        [Test]
        public void ConsoleSavesKeyboardState()
        {
            GameConsole console = new GameConsole(null);
            console.Active = true;
            var stubKeyboard = MockRepository.GenerateStub<IKeyboard>();
            GameConsoleController consoleController = new GameConsoleController(console, stubKeyboard);

            consoleController.Process(1);

            stubKeyboard.AssertWasCalled(x => x.SaveState());
        }

        [Test]
        public void ActiveConsoleLocksKeyboard()
        {
            GameConsole console = new GameConsole(null);
            var stubKeyboard = MockRepository.GenerateStub<IKeyboard>();
            console.Active = true;
            GameConsoleController consoleController = new GameConsoleController(console, stubKeyboard);

            consoleController.Process(1);

            stubKeyboard.AssertWasCalled(x => x.Lock());
        }

        [Test]
        public void InactiveConsoleDoesntLockKeyboard()
        {
            GameConsole console = new GameConsole(null);
            var stubKeyboard = MockRepository.GenerateStub<IKeyboard>();
            console.Active = false;
            GameConsoleController consoleController = new GameConsoleController(console, stubKeyboard);

            consoleController.Process(1);

            stubKeyboard.AssertWasNotCalled(x => x.Lock());
        }

        [Test]
        public void DeactivatingConsoleDOESSaveKeyboardState()
        {
            GameConsole console = new GameConsole(null);
            var stubKeyboard = MockRepository.GenerateStub<IKeyboard>();
            stubKeyboard.Stub(x => x.IsKeyDown(Keys.OemTilde)).Return(true);
            console.Active = true;
            GameConsoleController consoleController = new GameConsoleController(console, stubKeyboard);

            consoleController.Process(1);

            stubKeyboard.AssertWasCalled(x => x.SaveState());
        }

        [Test]
        public void AppendsPressedKeysToCurrentCommand()
        {
            GameConsole console = new GameConsole(null);
            var stubKeyboard = MockRepository.GenerateStub<IKeyboard>();
            stubKeyboard.Stub(x => x.IsKeyDown(Keys.A)).Return(true);
            console.Active = true;
            GameConsoleController consoleController = new GameConsoleController(console, stubKeyboard);
            Assert.AreEqual("", console.CurrentInput);

            consoleController.Process(1);

            Assert.AreEqual("a", console.CurrentInput);

            consoleController.Process(1);

            Assert.AreEqual("aa", console.CurrentInput);
        }

        [Test]
        public void LastKeyStateMustBeUpBeforeKeyPressAccepted()
        {
            GameConsole console = new GameConsole(null);
            var stubKeyboard = MockRepository.GenerateStub<IKeyboard>();
            stubKeyboard.Stub(x => x.IsKeyDown(Keys.B)).Return(true);
            console.Active = true;
            GameConsoleController consoleController = new GameConsoleController(console, stubKeyboard);
            Assert.AreEqual("", console.CurrentInput);

            consoleController.Process(1);

            Assert.AreEqual("b", console.CurrentInput);

            stubKeyboard.Stub(x => x.WasKeyDown(Keys.B)).Return(true);
            consoleController.Process(1);

            Assert.AreEqual("b", console.CurrentInput);
        }

        [Test]
        public void EnterCallsProcessOnCurrentCommand()
        {
            GameConsole console = new GameConsole(null);
            console.Active = true;
            console.CurrentInput = "hello";
            var stubKeyboard = MockRepository.GenerateStub<IKeyboard>();
            stubKeyboard.Stub(x => x.IsKeyDown(Keys.Enter)).Return(true);
            GameConsoleController consoleController = new GameConsoleController(console, stubKeyboard);
            Assert.AreEqual(0, console.CommandLog.Count);

            consoleController.Process(1);

            Assert.AreEqual(1, console.MessageLog.Count);
            Assert.AreEqual("hello", console.MessageLog[0]);
        }

        [Test]
        public void AcceptsUpperCaseLetters()
        {
            GameConsole console = new GameConsole(null);
            console.Active = true;
            var stubKeyboard = MockRepository.GenerateStub<IKeyboard>();
            stubKeyboard.Stub(x => x.IsKeyDown(Keys.C)).Return(true);
            stubKeyboard.Stub(x => x.IsKeyDown(Keys.LeftShift)).Return(true);
            GameConsoleController consoleController = new GameConsoleController(console, stubKeyboard);

            consoleController.Process(1);

            Assert.AreEqual("C", console.CurrentInput);
        }

        [Test]
        public void AcceptsNumbers()
        {
            GameConsole console = new GameConsole(null);
            console.Active = true;
            var stubKeyboard = MockRepository.GenerateStub<IKeyboard>();
            stubKeyboard.Stub(x => x.IsKeyDown(Keys.D1)).Return(true);
            GameConsoleController consoleController = new GameConsoleController(console, stubKeyboard);

            consoleController.Process(1);

            Assert.AreEqual("1", console.CurrentInput);
        }

        [Test]
        public void AcceptsNumPadNumbers()
        {
            GameConsole console = new GameConsole(null);
            console.Active = true;
            var stubKeyboard = MockRepository.GenerateStub<IKeyboard>();
            stubKeyboard.Stub(x => x.IsKeyDown(Keys.NumPad6)).Return(true);
            GameConsoleController consoleController = new GameConsoleController(console, stubKeyboard);

            consoleController.Process(1);

            Assert.AreEqual("6", console.CurrentInput);
        }

        [Test]
        public void AcceptsSpaces()
        {
            GameConsole console = new GameConsole(null);
            console.Active = true;
            var stubKeyboard = MockRepository.GenerateStub<IKeyboard>();
            stubKeyboard.Stub(x => x.IsKeyDown(Keys.Space)).Return(true);
            GameConsoleController consoleController = new GameConsoleController(console, stubKeyboard);

            consoleController.Process(1);

            Assert.AreEqual(" ", console.CurrentInput);
        }

        [Test]
        public void CanUseBackspace()
        {
            GameConsole console = new GameConsole(null);
            console.Active = true;
            console.CurrentInput = "smelly";
            var stubKeyboard = MockRepository.GenerateStub<IKeyboard>();
            stubKeyboard.Stub(x => x.IsKeyDown(Keys.Back)).Return(true);
            GameConsoleController consoleController = new GameConsoleController(console, stubKeyboard);

            consoleController.Process(1);

            Assert.AreEqual("smell", console.CurrentInput);
        }

        [Test]
        public void BackspaceHandlesEmptyStringCorrectly()
        {
            GameConsole console = new GameConsole(null);
            console.Active = true;
            console.CurrentInput = "";
            var stubKeyboard = MockRepository.GenerateStub<IKeyboard>();
            stubKeyboard.Stub(x => x.IsKeyDown(Keys.Back)).Return(true);
            GameConsoleController consoleController = new GameConsoleController(console, stubKeyboard);

            consoleController.Process(1);

            Assert.AreEqual("", console.CurrentInput);
        }

        [Test]
        public void TabTriesToAutocompleteCurrentCommand()
        {
            IGameConsole stubConsole = MockRepository.GenerateStub<IGameConsole>();
            stubConsole.CurrentInput = "";
            var stubKeyboard = MockRepository.GenerateStub<IKeyboard>();
            stubKeyboard.Stub(x => x.IsKeyDown(Keys.Tab)).Return(true);
            GameConsoleController consoleController = new GameConsoleController(stubConsole, stubKeyboard);
            stubConsole.Active = true;

            consoleController.Process(1);

            stubConsole.AssertWasCalled(x => x.TryToCompleteCurrentInput());
        }
    }
}
