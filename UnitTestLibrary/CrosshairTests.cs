using System;
using NUnit.Framework;
using Frenetic;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Rhino.Mocks;
using Frenetic.Graphics;
using Microsoft.Xna.Framework.Graphics;

namespace UnitTestLibrary
{
    [TestFixture]
    public class CrosshairTests
    {
        // **********
        // CROSSHAIR:
        // **********
        [Test]
        public void CrosshairViewPositionMatchesMousePosition()
        {
            Crosshair crosshair = new Crosshair(null);

            Vector2 mousePosition = new Vector2(Mouse.GetState().X, Mouse.GetState().Y);
            Assert.AreEqual(mousePosition, crosshair.ViewPosition);
        }

        [Test]
        public void CrosshairWorldPositionUsesCameraToTransform()
        {
            var stubCamera = MockRepository.GenerateStub<ICamera>();
            Crosshair crosshair = new Crosshair(stubCamera);
            Vector2 mousePosition = new Vector2(Mouse.GetState().X, Mouse.GetState().Y);
            stubCamera.Stub(x => x.ConvertToWorldCoordinates(Arg<Vector2>.Is.Equal(mousePosition))).Return(new Vector2(100, 200));

            Assert.AreEqual(new Vector2(100, 200), crosshair.WorldPosition);
        }

        [Test]
        public void CrosshairHasSize()
        {
            Crosshair crosshair = new Crosshair(null);

            crosshair.Size = 10;

            Assert.AreEqual(10, crosshair.Size);
        }
        
        // **************
        // CROSSHAIRVIEW:
        // **************
        [Test]
        public void CrosshairViewDrawsCorrectly()
        {
            var stubCamera = MockRepository.GenerateStub<ICamera>();
            Crosshair crosshair = new Crosshair(stubCamera);
            crosshair.Size = 10;
            var stubTexture = MockRepository.GenerateStub<ITexture>();
            var stubSpriteBatch = MockRepository.GenerateStub<ISpriteBatch>();
            CrosshairView crosshairView = new CrosshairView(crosshair, stubSpriteBatch, stubTexture);
            
            crosshairView.Generate();

            stubSpriteBatch.AssertWasCalled(x => x.Begin());
            stubSpriteBatch.AssertWasCalled(x => x.Draw
                                        (
                                        Arg<ITexture>.Is.Equal(stubTexture),
                                        Arg<Rectangle>.Is.Equal(new Rectangle((int)crosshair.ViewPosition.X - 5, (int)crosshair.ViewPosition.Y - 5, 10, 10)),
                                        Arg<Color>.Is.Equal(Color.White)
                                        ));
            stubSpriteBatch.AssertWasCalled(x => x.End());


        }
    }
}
