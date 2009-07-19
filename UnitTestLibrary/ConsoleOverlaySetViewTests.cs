using System;
using NUnit.Framework;
using Frenetic;
using Rhino.Mocks;
using Frenetic.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Frenetic.Engine.Overlay;

namespace UnitTestLibrary
{
    [TestFixture]
    public class ConsoleOverlaySetViewTests
    {
        Rectangle inputWindow = new Rectangle(5, 10, 15, 20);
        Rectangle commandWindow = new Rectangle(0, 0, 1, 1);
        Rectangle messageWindow = new Rectangle(20, 20, 5, 5);
        IOverlayView stubInputView;
        IOverlayView stubCommandConsoleView;
        IOverlayView stubMessageConsoleView;
        IOverlayView stubPossibleCommandsView;
        ISpriteBatch stubSpriteBatch;
        ITexture stubTexture;
        IFont stubFont;
        ConsoleOverlaySetView hudView;
        [SetUp]
        public void SetUp()
        {
            stubInputView = MockRepository.GenerateStub<IOverlayView>();
            stubCommandConsoleView = MockRepository.GenerateStub<IOverlayView>();
            stubMessageConsoleView = MockRepository.GenerateStub<IOverlayView>();
            stubPossibleCommandsView = MockRepository.GenerateStub<IOverlayView>();
            stubSpriteBatch = MockRepository.GenerateStub<ISpriteBatch>();
            stubTexture = MockRepository.GenerateStub<ITexture>();
            stubFont = MockRepository.GenerateStub<IFont>();
            hudView = new ConsoleOverlaySetView(stubInputView, stubCommandConsoleView, stubMessageConsoleView, stubPossibleCommandsView, stubSpriteBatch, stubTexture, stubFont);
        }

        

        [Test]
        public void OnlyDrawsVisibleHudViews()
        {
            hudView.Generate();

            stubSpriteBatch.AssertWasNotCalled(me => me.Begin());
        }

        [Test]
        public void DrawsHudViews()
        {
            stubInputView.Visible = true;
            stubInputView.Window = new Rectangle(10, 20, 30, 40);

            hudView.Generate();

            // WINDOW:
            stubSpriteBatch.AssertWasCalled(me => me.Draw(Arg<ITexture>.Is.Equal(stubTexture),
                    Arg<Rectangle>.Is.Equal(stubInputView.Window), Arg<Color>.Is.Anything, Arg<float>.Is.Equal(0f)));

            // VIEW:
            stubInputView.AssertWasCalled(me => me.Draw(stubSpriteBatch));
        }

        [Test]
        public void CallsBeginAndEndWhenDrawingHudViews()
        {
            stubInputView.Visible = true;

            hudView.Generate();

            stubSpriteBatch.AssertWasCalled(x => x.Begin());
            stubSpriteBatch.AssertWasCalled(x => x.End());
        }

    }
}
