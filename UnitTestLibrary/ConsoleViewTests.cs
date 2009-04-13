using System;
using NUnit.Framework;
using Frenetic;
using Rhino.Mocks;
using Frenetic.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace UnitTestLibrary
{
    [TestFixture]
    public class ConsoleViewTests
    {
        [Test]
        public void OnlyDrawsCommandConsoleWhenItsActive()
        {
            CommandConsole console = new CommandConsole(null, null);
            var stubSpriteBatch = MockRepository.GenerateStub<ISpriteBatch>();
            ConsoleView consoleView = new ConsoleView(console, MockRepository.GenerateStub<IMessageConsole>(), null, new Rectangle(), new Rectangle(), new Rectangle(), stubSpriteBatch, null, null);

            console.Active = false;
            consoleView.Generate();

            stubSpriteBatch.AssertWasNotCalled(x => x.Draw(Arg<ITexture>.Is.Anything, Arg<Rectangle>.Is.Anything, Arg<Color>.Is.Anything, Arg<float>.Is.Anything));
        }

        [Test]
        public void OnlyDrawsMessageConsoleWhenItsActive()
        {
            MessageConsole console = new MessageConsole(null, null);
            var stubSpriteBatch = MockRepository.GenerateStub<ISpriteBatch>();
            ConsoleView consoleView = new ConsoleView(MockRepository.GenerateStub<ICommandConsole>(), console, null, new Rectangle(), new Rectangle(), new Rectangle(), stubSpriteBatch, null, null);

            console.Active = false;
            consoleView.Generate();

            stubSpriteBatch.AssertWasNotCalled(x => x.Draw(Arg<ITexture>.Is.Anything, Arg<Rectangle>.Is.Anything, Arg<Color>.Is.Anything, Arg<float>.Is.Anything));
        }

        [Test]
        public void DrawsCommandLogWindow()
        {
            var stubCommandConsole = MockRepository.GenerateStub<ICommandConsole>();
            var stubSpriteBatch = MockRepository.GenerateStub<ISpriteBatch>();
            var stubTexture = MockRepository.GenerateStub<ITexture>();
            ConsoleView consoleView = new ConsoleView(stubCommandConsole, MockRepository.GenerateStub<IMessageConsole>(), new ConsoleController(null, null, null), new Rectangle(), new Rectangle(0, 0, 1, 1), new Rectangle(), stubSpriteBatch, stubTexture, null);

            stubCommandConsole.Active = true;
            consoleView.Generate();

            stubSpriteBatch.AssertWasCalled(x => x.Begin());
            stubSpriteBatch.AssertWasCalled(x => x.Draw(Arg<ITexture>.Is.Equal(stubTexture), 
                        Arg<Rectangle>.Is.Equal(new Rectangle(0, 0, 1, 1)), Arg<Color>.Is.Anything, Arg<float>.Is.Equal(0f)));
            stubSpriteBatch.AssertWasCalled(x => x.End());
        }

        [Test]
        public void DrawsMessageLogWindow()
        {
            MessageConsole console = new MessageConsole(new Log<ChatMessage>(), null);
            var stubSpriteBatch = MockRepository.GenerateStub<ISpriteBatch>();
            var stubTexture = MockRepository.GenerateStub<ITexture>();
            ConsoleView consoleView = new ConsoleView(MockRepository.GenerateStub<ICommandConsole>(), console, new ConsoleController(null, null, null), new Rectangle(), new Rectangle(), new Rectangle(0, 0, 100, 200), stubSpriteBatch, stubTexture, null);
            console.Active = true;
            console.Log.AddMessage(new ChatMessage() { Message = "message1" }); // Won't draw an empty window...

            consoleView.Generate();

            stubSpriteBatch.AssertWasCalled(x => x.Draw(Arg<ITexture>.Is.Equal(stubTexture),
                        Arg<Rectangle>.Is.Equal(new Rectangle(0, 0, 100, 200)), Arg<Color>.Is.Anything, Arg<float>.Is.Equal(0f)));
        }

        [Test]
        public void CallsBeginAndEndWhenDrawingMessageLogWindow()
        {
            MessageConsole console = new MessageConsole(new Log<ChatMessage>(), null);
            var stubSpriteBatch = MockRepository.GenerateStub<ISpriteBatch>();
            ConsoleView consoleView = new ConsoleView(MockRepository.GenerateStub<ICommandConsole>(), console, new ConsoleController(null, null, null), new Rectangle(), new Rectangle(), new Rectangle(), stubSpriteBatch, null, null);
            console.Active = true;
            console.Log.AddMessage(new ChatMessage() { Message = "message 1" });

            consoleView.Generate();

            stubSpriteBatch.AssertWasCalled(x => x.Begin());
            stubSpriteBatch.AssertWasCalled(x => x.End());
        }

        [Test]
        public void OnlyDrawsMessageWindowWhenThereAreMessages()
        {
            MessageConsole console = new MessageConsole(new Log<ChatMessage>(), null);
            var stubSpriteBatch = MockRepository.GenerateStub<ISpriteBatch>();
            var stubTexture = MockRepository.GenerateStub<ITexture>();
            ConsoleView consoleView = new ConsoleView(MockRepository.GenerateStub<ICommandConsole>(), console, new ConsoleController(null, null, null), new Rectangle(), new Rectangle(), new Rectangle(0, 0, 100, 200), stubSpriteBatch, stubTexture, null);
            console.Active = true;

            consoleView.Generate();

            stubSpriteBatch.AssertWasNotCalled(x => x.Draw(Arg<ITexture>.Is.Equal(stubTexture),
                        Arg<Rectangle>.Is.Equal(new Rectangle(0, 0, 100, 200)), Arg<Color>.Is.Anything, Arg<float>.Is.Equal(0f)));
        }

        [Test]
        public void DrawsInputWindow()
        {
            var stubCommandConsole = MockRepository.GenerateStub<ICommandConsole>();
            var stubSpriteBatch = MockRepository.GenerateStub<ISpriteBatch>();
            var stubTexture = MockRepository.GenerateStub<ITexture>();
            ConsoleView consoleView = new ConsoleView(stubCommandConsole, MockRepository.GenerateStub<IMessageConsole>(), new ConsoleController(null, null, null), new Rectangle(10, 20, 100, 200), new Rectangle(), new Rectangle(), stubSpriteBatch, stubTexture, null);
            stubCommandConsole.Active = true;

            consoleView.Generate();

            stubSpriteBatch.AssertWasCalled(x => x.Draw(Arg<ITexture>.Is.Equal(stubTexture),
                    Arg<Rectangle>.Is.Equal(new Rectangle(10, 20, 100, 200)), Arg<Color>.Is.Anything, Arg<float>.Is.Equal(0f)));
        }

        [Test]
        public void DrawsCurrentInput()
        {
            ConsoleController consoleController = new ConsoleController(null, null, null);
            var stubConsole = MockRepository.GenerateStub<ICommandConsole>();
            var stubSpriteBatch = MockRepository.GenerateStub<ISpriteBatch>();
            var stubFont = MockRepository.GenerateStub<IFont>();
            ConsoleView consoleView = new ConsoleView(stubConsole, MockRepository.GenerateStub<IMessageConsole>(), consoleController, new Rectangle(10, 20, 100, 200), new Rectangle(), new Rectangle(), stubSpriteBatch, null, stubFont);

            stubConsole.Active = true;
            consoleView.CursorText = "> ";
            consoleController.CurrentInput = "Hey there";
            consoleView.Generate();

            stubSpriteBatch.AssertWasCalled(x => x.DrawText(Arg<IFont>.Is.Equal(stubFont), Arg<string>.Is.Equal("> Hey there"), Arg<Vector2>.Is.Equal(new Vector2(10 + 20, 220 - 20)), Arg<Color>.Is.Anything));
        }


        [Test]
        public void DrawsCommandLog()
        {
            ICommandConsole stubConsole = MockRepository.GenerateStub<ICommandConsole>();
            var stubSpriteBatch = MockRepository.GenerateStub<ISpriteBatch>();
            var stubFont = MockRepository.GenerateStub<IFont>();
            ConsoleView consoleView = new ConsoleView(stubConsole, MockRepository.GenerateStub<IMessageConsole>(), new ConsoleController(null, null, null), new Rectangle(), new Rectangle(0, 0, 100, 100), new Rectangle(), stubSpriteBatch, null, stubFont);
            stubFont.LineSpacing = 10;
            stubConsole.Active = true;
            stubConsole.Log = new Log<string>();
            stubConsole.Log.AddMessage("You suck.");
            stubConsole.Log.AddMessage("I suck? Screw you!");
            
            consoleView.Generate();

            stubSpriteBatch.AssertWasCalled(x => x.DrawText(Arg<IFont>.Is.Equal(stubFont), Arg<string>.Is.Equal("You suck."), Arg<Vector2>.Is.Equal(new Vector2(20, 80 - (2 * stubFont.LineSpacing))), Arg<Color>.Is.Anything));
            stubSpriteBatch.AssertWasCalled(x => x.DrawText(Arg<IFont>.Is.Equal(stubFont), Arg<string>.Is.Equal("I suck? Screw you!"), Arg<Vector2>.Is.Equal(new Vector2(20, 80 - stubFont.LineSpacing)), Arg<Color>.Is.Anything));
        }

        [Test]
        public void DrawsMessageLog()
        {
            var stubMessageConsole = MockRepository.GenerateStub<IMessageConsole>();
            Log<ChatMessage> chatLog = new Log<ChatMessage>();
            stubMessageConsole.Log = chatLog;
            var stubSpriteBatch = MockRepository.GenerateStub<ISpriteBatch>();
            var stubFont = MockRepository.GenerateStub<IFont>();
            ConsoleView consoleView = new ConsoleView(MockRepository.GenerateStub<ICommandConsole>(), stubMessageConsole, null, new Rectangle(), new Rectangle(), new Rectangle(100, 200, 100, 100), stubSpriteBatch, null, stubFont);
            stubFont.LineSpacing = 10;

            stubMessageConsole.Active = true;
            stubMessageConsole.Log.AddMessage(new ChatMessage() { ClientName = "zak", Message = "You suck." });
            stubMessageConsole.Log.AddMessage(new ChatMessage() { ClientName = "terence", Message = "I suck? Screw you!" });

            consoleView.Generate();

            stubSpriteBatch.AssertWasCalled(x => x.DrawText(Arg<IFont>.Is.Equal(stubFont), Arg<string>.Is.Equal("[zak] You suck."), Arg<Vector2>.Is.Equal(new Vector2(100 + 20, 300 - 20 - (2 * stubFont.LineSpacing))), Arg<Color>.Is.Anything));
            stubSpriteBatch.AssertWasCalled(x => x.DrawText(Arg<IFont>.Is.Equal(stubFont), Arg<string>.Is.Equal("[terence] I suck? Screw you!"), Arg<Vector2>.Is.Equal(new Vector2(100 + 20, 300 - 20 - stubFont.LineSpacing)), Arg<Color>.Is.Anything));
        }

        [Test]
        public void DoesntDrawPossibleCompletionWindowWhenNoneAvailable()
        {
            var stubConsole = MockRepository.GenerateStub<ICommandConsole>();
            var stubSpriteBatch = MockRepository.GenerateStub<ISpriteBatch>();
            var stubFont = MockRepository.GenerateStub<IFont>();
            Rectangle commandWindow = new Rectangle(10, 20, 100, 200);
            Rectangle messageWindow = new Rectangle(30, 40, 300, 400);
            ConsoleView consoleView = new ConsoleView(stubConsole, MockRepository.GenerateStub<IMessageConsole>(), new ConsoleController(null, null, null), new Rectangle(), commandWindow, messageWindow, stubSpriteBatch, null, stubFont);
            stubConsole.Active = true;
            Log<string> possibleCommands = new Log<string>();
            stubConsole.Stub(x => x.FindPossibleInputCompletions(Arg<string>.Is.Anything)).Return(possibleCommands);

            consoleView.Generate();

            // Assuming the text offset is 20

            // Draw the background:
            Rectangle possibleCommandsWindow = new Rectangle(commandWindow.Right + consoleView.EdgeGap, commandWindow.Bottom - 2 * 20 - (2 * stubFont.LineSpacing), messageWindow.Width, 2 * 20 + (2 * stubFont.LineSpacing));
            stubSpriteBatch.AssertWasNotCalled(x => x.Draw(Arg<ITexture>.Is.Anything, Arg<Rectangle>.Is.Equal(possibleCommandsWindow), Arg<Color>.Is.Anything, Arg<float>.Is.Equal(0f)));
        }

        [Test]
        public void DrawsPossibleCommandCompletions()
        {
            var stubSpriteBatch = MockRepository.GenerateStub<ISpriteBatch>();
            var stubFont = MockRepository.GenerateStub<IFont>();
            var stubConsole = MockRepository.GenerateStub<ICommandConsole>();
            stubConsole.Log = new Log<string>();
            Rectangle commandWindow = new Rectangle(10, 20, 100, 200);
            Rectangle messageWindow = new Rectangle(30, 40, 300, 400);
            ConsoleView consoleView = new ConsoleView(stubConsole, MockRepository.GenerateStub<IMessageConsole>(), new ConsoleController(null, null, null), new Rectangle(), commandWindow, messageWindow, stubSpriteBatch, null, stubFont);
            stubFont.LineSpacing = 10;
            stubConsole.Active = true;
            Log<string> possibleCommands = new Log<string>();
            possibleCommands.AddMessage("Hey");
            possibleCommands.AddMessage("Yo");
            stubConsole.Stub(x => x.FindPossibleInputCompletions(Arg<string>.Is.Anything)).Return(possibleCommands);

            consoleView.Generate();

            // Assuming the text offset is 20
            
            // Gets possible commands:
            stubConsole.AssertWasCalled(x => x.FindPossibleInputCompletions(Arg<string>.Is.Equal("")));

            // Draw the background:
            Rectangle possibleCommandsWindow = new Rectangle(commandWindow.Right + consoleView.EdgeGap, commandWindow.Bottom - 2*20 - (2 * stubFont.LineSpacing), messageWindow.Width, 2*20 + (2 * stubFont.LineSpacing));
            stubSpriteBatch.AssertWasCalled(x => x.Draw(Arg<ITexture>.Is.Anything, Arg<Rectangle>.Is.Equal(possibleCommandsWindow), Arg<Color>.Is.Anything, Arg<float>.Is.Equal(0f)));

            // Draws the commands:
            stubSpriteBatch.AssertWasCalled(x => x.DrawText(Arg<IFont>.Is.Equal(stubFont), Arg<string>.Is.Equal("Hey"), Arg<Vector2>.Is.Equal(new Vector2(possibleCommandsWindow.Left + 20, possibleCommandsWindow.Bottom - 20 - (2 * stubFont.LineSpacing))), Arg<Color>.Is.Anything));
            stubSpriteBatch.AssertWasCalled(x => x.DrawText(Arg<IFont>.Is.Equal(stubFont), Arg<string>.Is.Equal("Yo"), Arg<Vector2>.Is.Equal(new Vector2(possibleCommandsWindow.Left + 20, possibleCommandsWindow.Bottom - 20 - stubFont.LineSpacing)), Arg<Color>.Is.Anything));
        }
    }
}
