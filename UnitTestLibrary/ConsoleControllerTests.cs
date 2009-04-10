using System;
using NUnit.Framework;
using Frenetic;
using Rhino.Mocks;
using Microsoft.Xna.Framework.Input;
using Frenetic.UserInput;

namespace UnitTestLibrary
{
    [TestFixture]
    public class ConsoleControllerTests
    {
        ICommandConsole stubCommandConsole;
        IMessageConsole stubMessageConsole;
        IKeyboard stubKeyboard;
        ConsoleController consoleController;
        [SetUp]
        public void SetUp()
        {
            stubCommandConsole = MockRepository.GenerateStub<ICommandConsole>();
            stubMessageConsole = MockRepository.GenerateStub<IMessageConsole>();
            stubKeyboard = MockRepository.GenerateStub<IKeyboard>();

            consoleController = new ConsoleController(stubCommandConsole, stubMessageConsole, stubKeyboard);
        }

        [Test]
        public void TildeActivatesConsoles()
        {
            stubKeyboard.Stub(x => x.IsKeyDown(Keys.OemTilde)).Return(true);
            stubCommandConsole.Active = false;
            stubMessageConsole.Active = false;

            consoleController.Process(1);

            Assert.IsTrue(stubCommandConsole.Active);
            Assert.IsTrue(stubMessageConsole.Active);
        }

        [Test]
        public void TildeTogglesConsolesOnAndOff()
        {
            stubKeyboard.Stub(x => x.IsKeyDown(Keys.OemTilde)).Return(true);
            stubCommandConsole.Active = false;
            stubMessageConsole.Active = false;

            consoleController.Process(1);   // ACTIVE
            consoleController.Process(1);   // Back to INACTIVE

            Assert.IsFalse(stubCommandConsole.Active);
            Assert.IsFalse(stubMessageConsole.Active);
        }

        [Test]
        public void TildeKeyMustBeReleasedAndRepressedToToggleConsoles()
        {
            stubKeyboard.Stub(x => x.IsKeyDown(Keys.OemTilde)).Return(true);
            stubKeyboard.Stub(x => x.WasKeyDown(Keys.OemTilde)).Return(true);
            stubCommandConsole.Active = false;
            stubMessageConsole.Active = false;

            consoleController.Process(1); 

            Assert.IsFalse(stubCommandConsole.Active);
            Assert.IsFalse(stubMessageConsole.Active);
        }

        [Test]
        public void ConsoleSavesKeyboardState()
        {
            stubCommandConsole.Active = true;
            stubMessageConsole.Active = true;

            consoleController.Process(1);

            stubKeyboard.AssertWasCalled(x => x.SaveState());
        }

        [Test]
        public void ActiveConsoleLocksKeyboard()
        {
            stubCommandConsole.Active = true;

            consoleController.Process(1);

            stubKeyboard.AssertWasCalled(x => x.Lock());
        }

        [Test]
        public void InactiveConsoleDoesntLockKeyboard()
        {
            stubCommandConsole.Active = false;
            stubMessageConsole.Active = false;

            consoleController.Process(1);

            stubKeyboard.AssertWasNotCalled(x => x.Lock());
        }

        [Test]
        public void DeactivatingConsoleDOESSaveKeyboardState()
        {
            stubKeyboard.Stub(x => x.IsKeyDown(Keys.OemTilde)).Return(true);
            stubCommandConsole.Active = true;
            ConsoleController consoleController = new ConsoleController(stubCommandConsole, stubMessageConsole, stubKeyboard);

            consoleController.Process(1);

            stubKeyboard.AssertWasCalled(x => x.SaveState());
        }

        [Test]
        public void AppendsPressedKeysToCurrentCommand()
        {
            stubKeyboard.Stub(x => x.IsKeyDown(Keys.A)).Return(true);
            stubCommandConsole.Active = true;
            Assert.AreEqual("", consoleController.CurrentInput);

            consoleController.Process(1);

            Assert.AreEqual("a", consoleController.CurrentInput);

            consoleController.Process(1);

            Assert.AreEqual("aa", consoleController.CurrentInput);
        }

        [Test]
        public void LastKeyStateMustBeUpBeforeKeyPressAccepted()
        {
            stubKeyboard.Stub(x => x.IsKeyDown(Keys.B)).Return(true);
            stubCommandConsole.Active = true;
            Assert.AreEqual("", consoleController.CurrentInput);

            consoleController.Process(1);

            Assert.AreEqual("b", consoleController.CurrentInput);

            stubKeyboard.Stub(x => x.WasKeyDown(Keys.B)).Return(true);
            consoleController.Process(1);

            Assert.AreEqual("b", consoleController.CurrentInput);
        }

        // Process Input:
        [Test]
        public void CommandInputGetsSentToCommandConsoleCorrectly()
        {
            stubCommandConsole.Active = true;
            stubKeyboard.Stub(x => x.IsKeyDown(Keys.Enter)).Return(true);
            consoleController.CurrentInput = "/hello";

            consoleController.Process(1);

            stubCommandConsole.AssertWasCalled(x => x.ProcessInput("/hello"));
        }
        [Test]
        public void NonCommandsAreSentToMessageConsole()
        {
            stubMessageConsole.Active = true;
            stubKeyboard.Stub(x => x.IsKeyDown(Keys.Enter)).Return(true);
            consoleController.CurrentInput = "hello";

            consoleController.Process(1);

            stubMessageConsole.AssertWasCalled(x => x.ProcessInput("hello"));
        }
        [Test]
        public void ClearsCurrentInputAfterProcessing()
        {
            stubCommandConsole.Active = true;
            stubKeyboard.Stub(x => x.IsKeyDown(Keys.Enter)).Return(true);
            consoleController.CurrentInput = "tmp";

            consoleController.Process(1);

            Assert.AreEqual("", consoleController.CurrentInput);
        }

        [Test]
        public void AcceptsUpperCaseLetters()
        {
            stubCommandConsole.Active = true;
            stubKeyboard.Stub(x => x.IsKeyDown(Keys.C)).Return(true);
            stubKeyboard.Stub(x => x.IsKeyDown(Keys.LeftShift)).Return(true);

            consoleController.Process(1);

            Assert.AreEqual("C", consoleController.CurrentInput);
        }

        [Test]
        public void AcceptsNumbers()
        {
            stubCommandConsole.Active = true;
            stubKeyboard.Stub(x => x.IsKeyDown(Keys.D1)).Return(true);

            consoleController.Process(1);

            Assert.AreEqual("1", consoleController.CurrentInput);
        }

        [Test]
        public void AcceptsNumPadNumbers()
        {
            stubCommandConsole.Active = true;
            stubKeyboard.Stub(x => x.IsKeyDown(Keys.NumPad6)).Return(true);

            consoleController.Process(1);

            Assert.AreEqual("6", consoleController.CurrentInput);
        }

        [Test]
        public void AcceptsSpaces()
        {
            stubCommandConsole.Active = true;
            stubKeyboard.Stub(x => x.IsKeyDown(Keys.Space)).Return(true);

            consoleController.Process(1);

            Assert.AreEqual(" ", consoleController.CurrentInput);
        }

        [Test]
        public void CanUseBackspace()
        {
            stubCommandConsole.Active = true;
            consoleController.CurrentInput = "smelly";
            stubKeyboard.Stub(x => x.IsKeyDown(Keys.Back)).Return(true);

            consoleController.Process(1);

            Assert.AreEqual("smell", consoleController.CurrentInput);
        }

        [Test]
        public void BackspaceHandlesEmptyStringCorrectly()
        {
            stubCommandConsole.Active = true;
            stubKeyboard.Stub(x => x.IsKeyDown(Keys.Back)).Return(true);
            consoleController.CurrentInput = "";

            consoleController.Process(1);

            Assert.AreEqual("", consoleController.CurrentInput);
        }

        [Test]
        public void TabTriesToAutocompleteCurrentCommand()
        {
            stubCommandConsole.Active = true;
            stubKeyboard.Stub(x => x.IsKeyDown(Keys.Tab)).Return(true);
            consoleController.CurrentInput = "";

            consoleController.Process(1);

            stubCommandConsole.AssertWasCalled(x => x.TryToCompleteInput(""));
        }
    }
}
