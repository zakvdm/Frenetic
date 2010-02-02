using System;

using Frenetic;

using NUnit.Framework;
using Frenetic.Physics;
using Rhino.Mocks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using FarseerGames.FarseerPhysics.Dynamics;
using Frenetic.UserInput;
using Frenetic.Player;
using Frenetic.Gameplay.Level;
using Frenetic.Weapons;

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
            stubRespawner = MockRepository.GenerateStub<IPlayerRespawner>();
            kpc = new KeyboardPlayerController(stubPlayer, stubKeyboard, stubMouse, stubCrosshair, stubRespawner);
        }

        [Test]
        public void ProcessCallsPlayerUpdate()
        {
            kpc.Process(1);

            stubPlayer.AssertWasCalled(x => x.Update());
        }

        [Test]
        public void ShouldDeleteAllDeadProjectiles()
        {
            stubPlayer.Stub(me => me.CurrentWeapon).Return(new RocketLauncher(null));
            var aliveRocket = new Rocket(Vector2.Zero, Vector2.Zero, MockRepository.GenerateStub<IPhysicsComponent>()) { IsAlive = true }; 
            var deadRocket = new Rocket(Vector2.Zero, Vector2.Zero, MockRepository.GenerateStub<IPhysicsComponent>()) { IsAlive = false }; 
            ((RocketLauncher)stubPlayer.CurrentWeapon).Rockets.AddRange(new System.Collections.Generic.List<Rocket>() { aliveRocket, deadRocket });

            kpc.RemoveDeadProjectiles();

            Assert.AreEqual(1, ((RocketLauncher)stubPlayer.CurrentWeapon).Rockets.Count);
            Assert.AreEqual(aliveRocket, ((RocketLauncher)stubPlayer.CurrentWeapon).Rockets[0]);
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
        public void PressingTheShootButtonCreatesPendingShotOnPlayerWhenPlayerIsAlive()
        {
            stubMouse.Stub(m => m.LeftButtonIsDown()).Return(true);
            stubCrosshair.Stub(c => c.WorldPosition).Return(Vector2.UnitX);
            stubPlayer.Status = PlayerStatus.Alive;

            kpc.Process(1);

            Assert.AreEqual(Vector2.UnitX, stubPlayer.PendingShot);           
        }
        [Test]
        public void PressingTheShootButtonASecondTimeTooSoonDoesNotRetriggerTheShot()
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

        [Test]
        public void PressingTheShootButtonWhenThePlayerIsDeadDoesARespawn()
        {
            stubMouse.Stub(m => m.LeftButtonIsDown()).Return(true);
            stubPlayer.Status = PlayerStatus.Dead;

            kpc.Process(100);

            stubRespawner.AssertWasCalled(me => me.RespawnPlayer(stubPlayer));
        }
        [Test]
        public void RespawnRequestStartsShootTimer()
        {
            stubMouse.Stub(m => m.LeftButtonIsDown()).Return(true);
            stubPlayer.Status = PlayerStatus.Dead;
            stubPlayer.PendingShot = null;

            kpc.Process(1);
            stubPlayer.Status = PlayerStatus.Alive;
            kpc.Process(KeyboardPlayerController.ShootTimer / 2);

            Assert.IsNull(stubPlayer.PendingShot);
        }

        IPlayer stubPlayer;
        IKeyboard stubKeyboard;
        IMouse stubMouse;
        ICrosshair stubCrosshair;
        IPlayerRespawner stubRespawner;
        KeyboardPlayerController kpc;
    }
}
