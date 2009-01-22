using System;

using Frenetic;

using NUnit.Framework;
using Frenetic.Physics;
using Rhino.Mocks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using FarseerGames.FarseerPhysics.Dynamics;

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
            kpc.Process(1);

            stubPlayer.AssertWasCalled(x => x.Update());
        }

        [Test]
        public void PressingTheSpacebarExertsJumpImpulseOnPlayer()
        {
            stubKeyboard.Stub(k => k.IsKeyDown(Arg<Keys>.Is.Equal(Keys.Space))).Return(true);
            var stubBody = MockRepository.GenerateStub<Body>();

            kpc.Process(1);
            stubPlayer.AssertWasCalled(p => p.Jump());
        }

        [Test]
        public void PressingTheLeftArrowExertsMovementForceOnPlayer()
        {
            stubKeyboard.Stub(k => k.IsKeyDown(Arg<Keys>.Is.Equal(Keys.Left))).Return(true);
            var stubBody = MockRepository.GenerateStub<Body>();

            kpc.Process(1);
            stubPlayer.AssertWasCalled(p => p.MoveLeft());
        }

        [Test]
        public void PressingTheRightArrowExertsMovementForceOnPlayer()
        {
            stubKeyboard.Stub(k => k.IsKeyDown(Arg<Keys>.Is.Equal(Keys.Right))).Return(true);
            var stubBody = MockRepository.GenerateStub<Body>();

            kpc.Process(1);
            stubPlayer.AssertWasCalled(p => p.MoveRight());
        }

        IPlayer stubPlayer;
        IKeyboard stubKeyboard;
        KeyboardPlayerController kpc;
    }
}
