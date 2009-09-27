using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Frenetic.Engine.Overlay;
using Microsoft.Xna.Framework;
using Rhino.Mocks;
using Frenetic.Graphics;
using Microsoft.Xna.Framework.Graphics;
using Frenetic;

namespace UnitTestLibrary
{
    [TestFixture]
    public class OverlayViewImplementationTests
    {
        Rectangle rectangle = new Rectangle(100, 200, 300, 400);
        ISpriteBatch stubSpriteBatch;
        IFont stubFont;
        IConsole<string> stubConsole;
        Log<string> log = new Log<string>();
        [SetUp]
        public void SetUp()
        {
            stubSpriteBatch = MockRepository.GenerateStub<ISpriteBatch>();
            stubFont = MockRepository.GenerateStub<IFont>();
            stubConsole = MockRepository.GenerateStub<IConsole<string>>();
            stubConsole.Log = log;
        }

        [Test]
        public void LogHudViewNotVisibleUnlessLogContainsItems()
        {
            stubConsole.Log = new Log<string>();
            LogOverlayView<string> hudView = new LogOverlayView<string>(stubConsole, rectangle, stubFont, Color.Wheat);

            hudView.Visible = true;

            Assert.IsFalse(hudView.Visible);

            stubConsole.Log.Add("test");

            Assert.IsTrue(hudView.Visible);
        }
        [Test]
        public void DrawsLog()
        {
            stubConsole.Log.Add("You suck.");
            stubConsole.Log.Add("I suck? Screw you!");
            LogOverlayView<string> hudView = new LogOverlayView<string>(stubConsole, rectangle, stubFont, Color.Violet);

            hudView.Draw(stubSpriteBatch);

            stubSpriteBatch.AssertWasCalled(x => x.DrawText(Arg<IFont>.Is.Equal(stubFont), Arg<string>.Is.Equal("You suck."), Arg<Vector2>.Is.Equal(new Vector2(rectangle.Left + OverlaySetView.TEXT_OFFSET.X, rectangle.Bottom - OverlaySetView.TEXT_OFFSET.Y - (2 * stubFont.LineSpacing))), Arg<Color>.Is.Equal(Color.Violet), Arg<float>.Is.Anything));
            stubSpriteBatch.AssertWasCalled(x => x.DrawText(Arg<IFont>.Is.Equal(stubFont), Arg<string>.Is.Equal("I suck? Screw you!"), Arg<Vector2>.Is.Equal(new Vector2(rectangle.Left + OverlaySetView.TEXT_OFFSET.X, rectangle.Bottom - OverlaySetView.TEXT_OFFSET.Y - stubFont.LineSpacing)), Arg<Color>.Is.Equal(Color.Violet), Arg<float>.Is.Anything));
        }

        [Test]
        public void DrawsCurrentInput()
        {
            InputLine inputLine = new InputLine();
            inputLine.CurrentInput = "Hey there";
            InputOverlayView hudView = new InputOverlayView(inputLine, rectangle, stubFont);

            hudView.Draw(stubSpriteBatch);

            stubSpriteBatch.AssertWasCalled(x => x.DrawText(Arg<IFont>.Is.Equal(stubFont), Arg<string>.Is.Equal(InputOverlayView.CursorText + "Hey there"), Arg<Vector2>.Is.Equal(new Vector2(rectangle.Left + OverlaySetView.TEXT_OFFSET.X, rectangle.Bottom - OverlaySetView.TEXT_OFFSET.Y)), Arg<Color>.Is.Anything, Arg<float>.Is.Anything));
        }

        [Test]
        public void NotVisibleWhenPossibleCommandsListIsEmpty()
        {
            var stubCommandConsole = MockRepository.GenerateStub<ICommandConsole>();
            stubCommandConsole.Active = true;
            Log<string> possibleCommands = new Log<string>();
            stubCommandConsole.Stub(me => me.FindPossibleInputCompletions(Arg<string>.Is.Anything)).Return(possibleCommands);
            PossibleCommandsLogHudView hudView = new PossibleCommandsLogHudView(new InputLine(), stubCommandConsole, rectangle, stubFont, Color.Blue);

            Assert.IsFalse(hudView.Visible);
        }
        [Test]
        public void ReturnsCorrectPossibleCommandsWindowSize()
        {
            var stubCommandConsole = MockRepository.GenerateStub<ICommandConsole>();
            Log<string> possibleCommands = new Log<string>() { "1", "2", "3" };
            stubCommandConsole.Stub(me => me.FindPossibleInputCompletions(Arg<string>.Is.Anything)).Return(possibleCommands);
            PossibleCommandsLogHudView hudView = new PossibleCommandsLogHudView(new InputLine(), stubCommandConsole, rectangle, stubFont, Color.Blue);

            Assert.AreEqual(rectangle.Width, hudView.Window.Width);
            Assert.AreEqual((int)(2 * OverlaySetView.TEXT_OFFSET.Y + (3 * stubFont.LineSpacing)), hudView.Window.Height);
        }

        [Test]
        public void DrawsPossibleCommandCompletions()
        {
            var stubCommandConsole = MockRepository.GenerateStub<ICommandConsole>();
            Log<string> possibleCommands = new Log<string>() { "Hey", "Yo" };
            stubCommandConsole.Stub(x => x.FindPossibleInputCompletions(Arg<string>.Is.Anything)).Return(possibleCommands);
            PossibleCommandsLogHudView hudView = new PossibleCommandsLogHudView(new InputLine(), stubCommandConsole, rectangle, stubFont, Color.Blue);
            Rectangle possibleCommandsWindow = hudView.Window;

            hudView.Draw(stubSpriteBatch);

            stubSpriteBatch.AssertWasCalled(x => x.DrawText(Arg<IFont>.Is.Equal(stubFont), Arg<string>.Is.Equal("Hey"), Arg<Vector2>.Is.Equal(new Vector2(possibleCommandsWindow.Left + OverlaySetView.TEXT_OFFSET.Y, possibleCommandsWindow.Bottom - OverlaySetView.TEXT_OFFSET.Y - (2 * stubFont.LineSpacing))), Arg<Color>.Is.Equal(Color.Blue), Arg<float>.Is.Anything));
            stubSpriteBatch.AssertWasCalled(x => x.DrawText(Arg<IFont>.Is.Equal(stubFont), Arg<string>.Is.Equal("Yo"), Arg<Vector2>.Is.Equal(new Vector2(possibleCommandsWindow.Left + OverlaySetView.TEXT_OFFSET.Y, possibleCommandsWindow.Bottom - OverlaySetView.TEXT_OFFSET.Y - stubFont.LineSpacing)), Arg<Color>.Is.Equal(Color.Blue), Arg<float>.Is.Anything));
        }
    }
}
