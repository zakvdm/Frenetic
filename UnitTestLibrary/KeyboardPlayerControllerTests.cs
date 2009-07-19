﻿using System;

using Frenetic;

using NUnit.Framework;
using Frenetic.Physics;
using Rhino.Mocks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using FarseerGames.FarseerPhysics.Dynamics;
using Frenetic.UserInput;
using Frenetic.Player;

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
        public void ProcessCallsPlayerUpdate()
        {
            kpc.Process(1);

            stubPlayer.AssertWasCalled(x => x.Update());
        }

        [Test]
        public void PressingTheSpacebarExertsJumpImpulseOnPlayer()
        {
            stubKeyboard.Stub(k => k.IsKeyDown(Arg<Keys>.Is.Equal(Keys.Space))).Return(true);

            kpc.Process(1);
            stubPlayer.AssertWasCalled(p => p.Jump());
        }

        [Test]
        public void PlayerCannotDoubleJump()
        {
            var lessThanJumpDelay = KeyboardPlayerController.JumpTimer / 3;
            var moreThanJumpDelay = KeyboardPlayerController.JumpTimer - (lessThanJumpDelay / 2);

            int callCount = 0;
            stubKeyboard.Stub(k => k.IsKeyDown(Arg<Keys>.Is.Equal(Keys.Space))).Return(true);
            stubPlayer.Stub(p => p.Jump()).Do(new Func<bool>(() => { callCount++; return true; }));
            kpc.Process(KeyboardPlayerController.JumpTimer);
            Assert.AreEqual(1, callCount);
            kpc.Process(lessThanJumpDelay);
            Assert.AreEqual(1, callCount);
            kpc.Process(moreThanJumpDelay);
            Assert.AreEqual(2, callCount);
        }

        [Test]
        public void PressingTheLeftArrowExertsMovementForceOnPlayer()
        {
            stubKeyboard.Stub(k => k.IsKeyDown(Arg<Keys>.Is.Equal(Keys.Left))).Return(true);

            kpc.Process(1);
            stubPlayer.AssertWasCalled(p => p.MoveLeft());
        }

        [Test]
        public void PressingTheRightArrowExertsMovementForceOnPlayer()
        {
            stubKeyboard.Stub(k => k.IsKeyDown(Arg<Keys>.Is.Equal(Keys.Right))).Return(true);

            kpc.Process(1);
            stubPlayer.AssertWasCalled(p => p.MoveRight());
        }   

        [Test]
        public void PressingTheLeftMouseButtonCreatesPendingShotOnPlayer()
        {
            stubMouse.Stub(m => m.LeftButtonIsDown()).Return(true);
            stubCrosshair.Stub(c => c.WorldPosition).Return(Vector2.UnitX);

            kpc.Process(1);

            Assert.AreEqual(Vector2.UnitX, stubPlayer.PendingShot);           
        }

        [Test]
        public void PressingTheLeftMouseButtonASecondTimeTooSoonDoesNotRetriggerTheShot()
        {
            var lessThanShootDelay = KeyboardPlayerController.ShootTimer / 2;
            var moreThanShootDelay = KeyboardPlayerController.ShootTimer - (lessThanShootDelay / 3);
            stubMouse.Stub(m => m.LeftButtonIsDown()).Return(true);
            stubCrosshair.Stub(c => c.WorldPosition).Return(Vector2.UnitX);

            kpc.Process(KeyboardPlayerController.ShootTimer);
            stubPlayer.PendingShot = Vector2.Zero;

            kpc.Process(lessThanShootDelay);
            Assert.AreEqual(Vector2.Zero, stubPlayer.PendingShot);
            kpc.Process(moreThanShootDelay);
            Assert.AreEqual(Vector2.UnitX, stubPlayer.PendingShot);
        }

        IPlayer stubPlayer;
        IKeyboard stubKeyboard;
        IMouse stubMouse;
        ICrosshair stubCrosshair;
        KeyboardPlayerController kpc;
    }
}
