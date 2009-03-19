using System;

using Frenetic;

using NUnit.Framework;
using Frenetic.Physics;
using Rhino.Mocks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using FarseerGames.FarseerPhysics.Dynamics;
using Frenetic.UserInput;

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
            stubMouse = MockRepository.GenerateStub<IMouse>();
            stubCrosshair = MockRepository.GenerateStub<ICrosshair>();
            kpc = new KeyboardPlayerController(stubPlayer, stubKeyboard, stubMouse, stubCrosshair);
        }

        [Test]
        public void CanConstruct()
        {
            Player player = new Player(1, null, null);
            KeyboardPlayerController kpc = new KeyboardPlayerController(player, stubKeyboard, stubMouse, stubCrosshair);
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
        public void PlayerCannotDoubleJump()
        {
            var fourHundredMilliseconds = new TimeSpan(0, 0, 0, 0, 400).Ticks;
            var eightHundredMilliseconds = 2 * fourHundredMilliseconds;
            int callCount = 0;
            stubKeyboard.Stub(k => k.IsKeyDown(Arg<Keys>.Is.Equal(Keys.Space))).Return(true);
            stubPlayer.Stub(p => p.Jump()).Do(new Func<bool>(() => { callCount++; return true; }));
            kpc.Process(1);
            Assert.AreEqual(1, callCount);
            kpc.Process(fourHundredMilliseconds);
            Assert.AreEqual(1, callCount);
            kpc.Process(eightHundredMilliseconds);
            Assert.AreEqual(2, callCount);
            kpc.Process(fourHundredMilliseconds);
            Assert.AreEqual(2, callCount);
            kpc.Process(eightHundredMilliseconds);
            Assert.AreEqual(3, callCount);
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

        [Test]
        public void PressingTheLeftMouseButtonGeneratesShotEvent()
        {
            stubMouse.Stub(m => m.LeftButtonIsDown()).Return(true);
            stubCrosshair.Stub(c => c.WorldPosition).Return(Vector2.UnitX);
            kpc.Process(1);
            stubPlayer.AssertWasCalled(p => p.Shoot(Vector2.UnitX));
        }

        [Test]
        public void PressingTheLeftMouseButtonASecondTimeTooSoonDoesNotRetriggerTheShot()
        {
            var oneHundredMilliseconds = new TimeSpan(0, 0, 0, 0, 100).Ticks;
            var twoHundredMilliseconds = 2 * oneHundredMilliseconds;
            int callCount = 0;
            stubMouse.Stub(m => m.LeftButtonIsDown()).Return(true);
            stubCrosshair.Stub(c => c.WorldPosition).Return(Vector2.UnitX);
            stubPlayer.Stub(p => p.Shoot(Vector2.UnitX)).Do(new Action<Vector2>((pos) => callCount++));
            kpc.Process(1);
            Assert.AreEqual(1, callCount);
            kpc.Process(oneHundredMilliseconds);
            Assert.AreEqual(1, callCount);
            kpc.Process(twoHundredMilliseconds);
            Assert.AreEqual(2, callCount);
            kpc.Process(oneHundredMilliseconds);
            Assert.AreEqual(2, callCount);
            kpc.Process(twoHundredMilliseconds);
            Assert.AreEqual(3, callCount);
        }

        IPlayer stubPlayer;
        IKeyboard stubKeyboard;
        IMouse stubMouse;
        ICrosshair stubCrosshair;
        KeyboardPlayerController kpc;
    }
}
