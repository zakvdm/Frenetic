using System;
using NUnit.Framework;
using Frenetic;
using Rhino.Mocks;
using Microsoft.Xna.Framework.Input;
using Frenetic.UserInput;
using Frenetic.Engine.Overlay;

namespace UnitTestLibrary
{
    [TestFixture]
    public class ConsoleControllerTests
    {
        InputLine inputLine;
        ICommandConsole stubCommandConsole;
        IMessageConsole stubMessageConsole;
        IOverlayView stubInputView;
        IOverlayView stubCommandConsoleView;
        IOverlayView stubMessageConsoleView;
        IOverlayView stubPossibleCommandsView;
        IKeyboard stubKeyboard;
        ConsoleController consoleController;
        [SetUp]
        public void SetUp()
        {
            inputLine = new InputLine();
            stubCommandConsole = MockRepository.GenerateStub<ICommandConsole>();
            stubMessageConsole = MockRepository.GenerateStub<IMessageConsole>();
            stubInputView = MockRepository.GenerateStub<IOverlayView>();
            stubCommandConsoleView = MockRepository.GenerateStub<IOverlayView>();
            stubMessageConsoleView = MockRepository.GenerateStub<IOverlayView>();
            stubPossibleCommandsView = MockRepository.GenerateStub<IOverlayView>();
            stubKeyboard = MockRepository.GenerateStub<IKeyboard>();

            consoleController = new ConsoleController(inputLine, stubCommandConsole, stubMessageConsole, stubInputView, stubCommandConsoleView, stubMessageConsoleView, stubPossibleCommandsView, stubKeyboard);
        }

        [Test]
        public void TildeActivatesHudViews()
        {
            stubKeyboard.Stub(x => x.IsKeyDown(Keys.OemTilde)).Return(true);
            stubCommandConsole.Active = false;
            stubMessageConsole.Active = false;
            stubInputView.Visible = false;

            consoleController.Process(1);

            Assert.IsTrue(stubCommandConsole.Active);
            Assert.IsTrue(stubMessageConsole.Active);
            Assert.IsTrue(stubInputView.Visible);
            Assert.IsTrue(stubCommandConsoleView.Visible);
            Assert.IsTrue(stubMessageConsoleView.Visible);
            Assert.IsTrue(stubPossibleCommandsView.Visible);
        }

        [Test]
        public void TildeTogglesHudViewsOnAndOff()
        {
            stubKeyboard.Stub(x => x.IsKeyDown(Keys.OemTilde)).Return(true);
            stubCommandConsole.Active = false;
            stubMessageConsole.Active = false;

            consoleController.Process(1);   // ACTIVE
            consoleController.Process(1);   // Back to INACTIVE

            Assert.IsFalse(stubCommandConsole.Active);
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

            consoleController.Process(1);

            stubKeyboard.AssertWasCalled(x => x.SaveState());
        }

        [Test]
        public void AppendsPressedKeysToCurrentCommand()
        {
            stubKeyboard.Stub(x => x.IsKeyDown(Keys.A)).Return(true);
            stubCommandConsole.Active = true;
            Assert.AreEqual("", inputLine.CurrentInput);

            consoleController.Process(1);

            Assert.AreEqual("a", inputLine.CurrentInput);

            consoleController.Process(1);

            Assert.AreEqual("aa", inputLine.CurrentInput);
        }

        [Test]
        public void LastKeyStateMustBeUpBeforeKeyPressAccepted()
        {
            stubKeyboard.Stub(x => x.IsKeyDown(Keys.B)).Return(true);
            stubCommandConsole.Active = true;
            Assert.AreEqual("", inputLine.CurrentInput);

            consoleController.Process(1);

            Assert.AreEqual("b", inputLine.CurrentInput);

            stubKeyboard.Stub(x => x.WasKeyDown(Keys.B)).Return(true);
            consoleController.Process(1);

            Assert.AreEqual("b", inputLine.CurrentInput);
        }

        // Process Input:
        [Test]
        public void CommandInputGetsSentToCommandConsoleCorrectly()
        {
            stubCommandConsole.Active = true;
            stubKeyboard.Stub(x => x.IsKeyDown(Keys.Enter)).Return(true);
            inputLine.CurrentInput = "/hello";

            consoleController.Process(1);

            stubCommandConsole.AssertWasCalled(x => x.ProcessInput("/hello"));
        }
        [Test]
        public void NonCommandsAreSentToMessageConsole()
        {
            stubMessageConsole.Active = true;
            stubKeyboard.Stub(x => x.IsKeyDown(Keys.Enter)).Return(true);
            inputLine.CurrentInput = "hello";

            consoleController.Process(1);

            stubMessageConsole.AssertWasCalled(x => x.ProcessInput("hello"));
        }
        [Test]
        public void ClearsCurrentInputAfterProcessing()
        {
            stubCommandConsole.Active = true;
            stubKeyboard.Stub(x => x.IsKeyDown(Keys.Enter)).Return(true);
            inputLine.CurrentInput = "tmp";

            consoleController.Process(1);

            Assert.AreEqual("", inputLine.CurrentInput);
        }

        [Test]
        public void AcceptsUpperCaseLetters()
        {
            stubCommandConsole.Active = true;
            stubKeyboard.Stub(x => x.IsKeyDown(Keys.C)).Return(true);
            stubKeyboard.Stub(x => x.IsKeyDown(Keys.LeftShift)).Return(true);

            consoleController.Process(1);

            Assert.AreEqual("C", inputLine.CurrentInput);
        }

        [Test]
        public void AcceptsNumbers()
        {
            stubCommandConsole.Active = true;
            stubKeyboard.Stub(x => x.IsKeyDown(Keys.D1)).Return(true);

            consoleController.Process(1);

            Assert.AreEqual("1", inputLine.CurrentInput);
        }

        [Test]
        public void AcceptsNumPadNumbers()
        {
            stubCommandConsole.Active = true;
            stubKeyboard.Stub(x => x.IsKeyDown(Keys.NumPad6)).Return(true);

            consoleController.Process(1);

            Assert.AreEqual("6", inputLine.CurrentInput);
        }

        [Test]
        public void AcceptsSpaces()
        {
            stubCommandConsole.Active = true;
            stubKeyboard.Stub(x => x.IsKeyDown(Keys.Space)).Return(true);

            consoleController.Process(1);

            Assert.AreEqual(" ", inputLine.CurrentInput);
        }

        [Test]
        public void CanUseBackspace()
        {
            stubCommandConsole.Active = true;
            inputLine.CurrentInput = "smelly";
            stubKeyboard.Stub(x => x.IsKeyDown(Keys.Back)).Return(true);

            consoleController.Process(1);

            Assert.AreEqual("smell", inputLine.CurrentInput);
        }

        [Test]
        public void BackspaceHandlesEmptyStringCorrectly()
        {
            stubCommandConsole.Active = true;
            stubKeyboard.Stub(x => x.IsKeyDown(Keys.Back)).Return(true);
            inputLine.CurrentInput = "";

            consoleController.Process(1);

            Assert.AreEqual("", inputLine.CurrentInput);
        }

        [Test]
        public void TabTriesToAutocompleteCurrentCommand()
        {
            stubCommandConsole.Active = true;
            stubKeyboard.Stub(x => x.IsKeyDown(Keys.Tab)).Return(true);
            inputLine.CurrentInput = "";

            consoleController.Process(1);

            stubCommandConsole.AssertWasCalled(x => x.TryToCompleteInput(""));
        }

        [Test]
        public void TabUpdatesCurrentInputWithPartialCompletion()
        {
            stubCommandConsole.Active = true;
            stubKeyboard.Stub(x => x.IsKeyDown(Keys.Tab)).Return(true);
            stubCommandConsole.Stub(x => x.TryToCompleteInput(Arg<string>.Is.Anything)).Return("/yo");

            consoleController.Process(1);

            Assert.AreEqual("/yo", inputLine.CurrentInput);
        }
    }
}
