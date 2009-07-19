using System;
using NUnit.Framework;
using Frenetic.Gameplay.HUD;
using Rhino.Mocks;
using Frenetic.Engine.Overlay;
using Frenetic.UserInput;
using Microsoft.Xna.Framework.Input;

namespace UnitTestLibrary
{
    [TestFixture]
    public class HudControllerTests
    {
        [Test]
        public void HoldingTabTogglesScoreViewOn()
        {
            var scoreView = new ScoreOverlayView(null, Microsoft.Xna.Framework.Rectangle.Empty, null);
            var stubKeyboard = MockRepository.GenerateStub<IKeyboard>();
            HudController controller = new HudController(scoreView, stubKeyboard);
            stubKeyboard.Stub(x => x.IsKeyDown(Keys.Tab)).Return(true);

            controller.Process(1);

            Assert.IsTrue(scoreView.Visible);
        }
    }
}
