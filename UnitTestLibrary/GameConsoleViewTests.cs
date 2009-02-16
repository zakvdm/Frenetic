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
    public class GameConsoleViewTests
    {
        [Test]
        public void OnlyDrawsConsoleWhenItsActive()
        {
            GameConsole console = new GameConsole(null);
            var stubSpriteBatch = MockRepository.GenerateStub<ISpriteBatch>();
            GameConsoleView consoleView = new GameConsoleView(console, new Rectangle(), new Rectangle(), new Rectangle(), stubSpriteBatch, null, null);

            console.Active = false;
            consoleView.Generate();

            stubSpriteBatch.AssertWasNotCalled(x => x.Draw(Arg<ITexture>.Is.Anything, Arg<Rectangle>.Is.Anything, Arg<Color>.Is.Anything, Arg<float>.Is.Anything));
        }

        [Test]
        public void DrawsCommandLogWindow()
        {
            GameConsole console = new GameConsole(null);
            var stubSpriteBatch = MockRepository.GenerateStub<ISpriteBatch>();
            var stubTexture = MockRepository.GenerateStub<ITexture>();
            GameConsoleView consoleView = new GameConsoleView(console, new Rectangle(), new Rectangle(0, 0, 1, 1), new Rectangle(), stubSpriteBatch, stubTexture, null);

            console.Active = true;
            consoleView.Generate();

            stubSpriteBatch.AssertWasCalled(x => x.Begin());
            stubSpriteBatch.AssertWasCalled(x => x.Draw(Arg<ITexture>.Is.Equal(stubTexture), 
                        Arg<Rectangle>.Is.Equal(new Rectangle(0, 0, 1, 1)), Arg<Color>.Is.Anything, Arg<float>.Is.Equal(0f)));
            stubSpriteBatch.AssertWasCalled(x => x.End());
        }

        [Test]
        public void DrawsMessageLogWindow()
        {
            GameConsole console = new GameConsole(null);
            var stubSpriteBatch = MockRepository.GenerateStub<ISpriteBatch>();
            var stubTexture = MockRepository.GenerateStub<ITexture>();
            GameConsoleView consoleView = new GameConsoleView(console, new Rectangle(), new Rectangle(), new Rectangle(0, 0, 100, 200), stubSpriteBatch, stubTexture, null);
            console.Active = true;
            console.MessageLog.Add("message1"); // Won't draw an empty window...

            consoleView.Generate();

            stubSpriteBatch.AssertWasCalled(x => x.Draw(Arg<ITexture>.Is.Equal(stubTexture),
                        Arg<Rectangle>.Is.Equal(new Rectangle(0, 0, 100, 200)), Arg<Color>.Is.Anything, Arg<float>.Is.Equal(0f)));
        }

        [Test]
        public void OnlyDrawsMessageWindowWhenThereAreMessages()
        {
            GameConsole console = new GameConsole(null);
            var stubSpriteBatch = MockRepository.GenerateStub<ISpriteBatch>();
            var stubTexture = MockRepository.GenerateStub<ITexture>();
            GameConsoleView consoleView = new GameConsoleView(console, new Rectangle(), new Rectangle(), new Rectangle(0, 0, 100, 200), stubSpriteBatch, stubTexture, null);

            console.Active = true;
            consoleView.Generate();

            stubSpriteBatch.AssertWasNotCalled(x => x.Draw(Arg<ITexture>.Is.Equal(stubTexture),
                        Arg<Rectangle>.Is.Equal(new Rectangle(0, 0, 100, 200)), Arg<Color>.Is.Anything, Arg<float>.Is.Equal(0f)));
        }

        [Test]
        public void DrawsInputWindow()
        {
            GameConsole console = new GameConsole(null);
            var stubSpriteBatch = MockRepository.GenerateStub<ISpriteBatch>();
            var stubTexture = MockRepository.GenerateStub<ITexture>();
            GameConsoleView consoleView = new GameConsoleView(console, new Rectangle(10, 20, 100, 200), new Rectangle(), new Rectangle(), stubSpriteBatch, stubTexture, null);
            console.Active = true;

            consoleView.Generate();

            stubSpriteBatch.AssertWasCalled(x => x.Draw(Arg<ITexture>.Is.Equal(stubTexture),
                    Arg<Rectangle>.Is.Equal(new Rectangle(10, 20, 100, 200)), Arg<Color>.Is.Anything, Arg<float>.Is.Equal(0f)));
        }

        [Test]
        public void DrawsCurrentInput()
        {
            IGameConsole stubConsole = new GameConsole(null);
            var stubSpriteBatch = MockRepository.GenerateStub<ISpriteBatch>();
            var stubFont = MockRepository.GenerateStub<IFont>();
            GameConsoleView consoleView = new GameConsoleView(stubConsole, new Rectangle(10, 20, 100, 200), new Rectangle(), new Rectangle(), stubSpriteBatch, null, stubFont);

            stubConsole.Active = true;
            consoleView.CursorText = "> ";
            stubConsole.CurrentInput = "Hey there";
            consoleView.Generate();

            stubSpriteBatch.AssertWasCalled(x => x.DrawText(Arg<IFont>.Is.Equal(stubFont), Arg<string>.Is.Equal("> Hey there"), Arg<Vector2>.Is.Equal(new Vector2(10 + 20, 220 - 20)), Arg<Color>.Is.Anything));
        }


        [Test]
        public void DrawsCommandLog()
        {
            IGameConsole stubConsole = MockRepository.GenerateStub<IGameConsole>();
            var stubSpriteBatch = MockRepository.GenerateStub<ISpriteBatch>();
            var stubFont = MockRepository.GenerateStub<IFont>();
            GameConsoleView consoleView = new GameConsoleView(stubConsole, new Rectangle(), new Rectangle(0, 0, 100, 100), new Rectangle(), stubSpriteBatch, null, stubFont);
            stubFont.LineSpacing = 10;
            stubConsole.Active = true;
            stubConsole.CommandLog = new List<string>();
            stubConsole.CommandLog.Add("You suck.");
            stubConsole.CommandLog.Add("I suck? Screw you!");
            
            consoleView.Generate();

            stubSpriteBatch.AssertWasCalled(x => x.DrawText(Arg<IFont>.Is.Equal(stubFont), Arg<string>.Is.Equal("You suck."), Arg<Vector2>.Is.Equal(new Vector2(20, 80 - (2 * stubFont.LineSpacing))), Arg<Color>.Is.Anything));
            stubSpriteBatch.AssertWasCalled(x => x.DrawText(Arg<IFont>.Is.Equal(stubFont), Arg<string>.Is.Equal("I suck? Screw you!"), Arg<Vector2>.Is.Equal(new Vector2(20, 80 - stubFont.LineSpacing)), Arg<Color>.Is.Anything));
        }

        [Test]
        public void DrawsMessageLog()
        {
            GameConsole console = new GameConsole(null);
            var stubSpriteBatch = MockRepository.GenerateStub<ISpriteBatch>();
            var stubFont = MockRepository.GenerateStub<IFont>();
            GameConsoleView consoleView = new GameConsoleView(console, new Rectangle(), new Rectangle(), new Rectangle(100, 200, 100, 100), stubSpriteBatch, null, stubFont);
            stubFont.LineSpacing = 10;

            console.Active = true;
            console.MessageLog.Add("You suck.");
            console.MessageLog.Add("I suck? Screw you!");
            consoleView.Generate();

            stubSpriteBatch.AssertWasCalled(x => x.DrawText(Arg<IFont>.Is.Equal(stubFont), Arg<string>.Is.Equal("You suck."), Arg<Vector2>.Is.Equal(new Vector2(100 + 20, 300 - 20 - (2 * stubFont.LineSpacing))), Arg<Color>.Is.Anything));
            stubSpriteBatch.AssertWasCalled(x => x.DrawText(Arg<IFont>.Is.Equal(stubFont), Arg<string>.Is.Equal("I suck? Screw you!"), Arg<Vector2>.Is.Equal(new Vector2(100 + 20, 300 - 20 - stubFont.LineSpacing)), Arg<Color>.Is.Anything));
        }

        [Test]
        public void DoesntDrawPossibleCompletionWindowWhenNoneAvailable()
        {
            var stubConsole = MockRepository.GenerateStub<IGameConsole>();
            var stubSpriteBatch = MockRepository.GenerateStub<ISpriteBatch>();
            var stubFont = MockRepository.GenerateStub<IFont>();
            Rectangle commandWindow = new Rectangle(10, 20, 100, 200);
            Rectangle messageWindow = new Rectangle(30, 40, 300, 400);
            GameConsoleView consoleView = new GameConsoleView(stubConsole, new Rectangle(), commandWindow, messageWindow, stubSpriteBatch, null, stubFont);
            stubConsole.Active = true;
            List<Command> possibleCommands = new List<Command>();
            stubConsole.Stub(x => x.FindPossibleInputCompletions()).Return(possibleCommands);

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
            var stubConsole = MockRepository.GenerateStub<IGameConsole>();
            stubConsole.MessageLog = new List<string>();
            stubConsole.CommandLog = new List<string>();
            Rectangle commandWindow = new Rectangle(10, 20, 100, 200);
            Rectangle messageWindow = new Rectangle(30, 40, 300, 400);
            GameConsoleView consoleView = new GameConsoleView(stubConsole, new Rectangle(), commandWindow, messageWindow, stubSpriteBatch, null, stubFont);
            stubFont.LineSpacing = 10;
            stubConsole.Active = true;
            List<Command> possibleCommands = new List<Command>();
            possibleCommands.Add(new Command("Hey"));
            possibleCommands.Add(new Command("Yo"));
            stubConsole.Stub(x => x.FindPossibleInputCompletions()).Return(possibleCommands);

            consoleView.Generate();

            // Assuming the text offset is 20
            
            // Draw the background:
            Rectangle possibleCommandsWindow = new Rectangle(commandWindow.Right + consoleView.EdgeGap, commandWindow.Bottom - 2*20 - (2 * stubFont.LineSpacing), messageWindow.Width, 2*20 + (2 * stubFont.LineSpacing));
            stubSpriteBatch.AssertWasCalled(x => x.Draw(Arg<ITexture>.Is.Anything, Arg<Rectangle>.Is.Equal(possibleCommandsWindow), Arg<Color>.Is.Anything, Arg<float>.Is.Equal(0f)));

            stubSpriteBatch.AssertWasCalled(x => x.DrawText(Arg<IFont>.Is.Equal(stubFont), Arg<string>.Is.Equal("Hey"), Arg<Vector2>.Is.Equal(new Vector2(possibleCommandsWindow.Left + 20, possibleCommandsWindow.Bottom - 20 - (2 * stubFont.LineSpacing))), Arg<Color>.Is.Anything));
            stubSpriteBatch.AssertWasCalled(x => x.DrawText(Arg<IFont>.Is.Equal(stubFont), Arg<string>.Is.Equal("Yo"), Arg<Vector2>.Is.Equal(new Vector2(possibleCommandsWindow.Left + 20, possibleCommandsWindow.Bottom - 20 - stubFont.LineSpacing)), Arg<Color>.Is.Anything));
        }
    }
}
