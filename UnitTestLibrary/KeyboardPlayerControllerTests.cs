using System;

using Frenetic;

using NUnit.Framework;
using Frenetic.Physics;
using Rhino.Mocks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using FarseerGames.FarseerPhysics.Dynamics;
using FarseerVector2 = FarseerGames.FarseerPhysics.Mathematics.Vector2;

namespace UnitTestLibrary
{
    [TestFixture]
    public class KeyboardPlayerControllerTests
    {
        [SetUp]
        public void SetUp()
        {
            stubPlayer = MockRepository.GenerateStub<IPlayer>();
            stubKeyboard = MockRepository.GenerateStub<IKeyboard>();
            kpc = new KeyboardPlayerController(stubPlayer, stubKeyboard);
        }

        [Test]
        public void CanConstruct()
        {
            Player player = new Player(1, null, null);
            KeyboardPlayerController kpc = new KeyboardPlayerController(player, stubKeyboard);
        }

        [Test]
        public void ProcessCallsPlayerUpdate()
        {
/*<<<<<<< Updated upstream:UnitTestLibrary/KeyboardPlayerControllerTests.cs
            IPlayer stubPlayer = MockRepository.GenerateStub<IPlayer>();
            KeyboardPlayerController kpc = new KeyboardPlayerController(stubPlayer);

            kpc.Process(1);
=======
*/            kpc.Process(1);
//>>>>>>> Stashed changes:UnitTestLibrary/KeyboardPlayerControllerTests.cs

            stubPlayer.AssertWasCalled(x => x.Update());
        }

        [Test]
        public void PressingTheSpacebarExertsJumpForceOnPlayer()
        {
            stubKeyboard.Stub(k => k.IsKeyDown(Arg<Keys>.Is.Equal(Keys.Space))).Return(true);
            var stubBody = MockRepository.GenerateStub<Body>();
            stubPlayer.Stub(p => p.Body).Return(stubBody);

            Assert.AreEqual(new FarseerVector2(0, 0), stubBody.Force);
            kpc.Process(1);
            Assert.AreEqual(new FarseerVector2(0, 10), stubBody.Force);
        }

        IPlayer stubPlayer;
        IKeyboard stubKeyboard;
        KeyboardPlayerController kpc;
    }
}
