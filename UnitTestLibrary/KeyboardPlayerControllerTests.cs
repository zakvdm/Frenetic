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
using Frenetic.Gameplay.Weapons;

namespace UnitTestLibrary
{
    [TestFixture]
    public class KeyboardPlayerControllerTests
    {
        IPlayer stubPlayer;
        IGameInput stubGameInput;
        ICrosshair stubCrosshair;
        IPlayerRespawner stubRespawner;
        KeyboardPlayerController kpc;
        [SetUp]
        public void SetUp()
        {
            stubPlayer = MockRepository.GenerateStub<IPlayer>();
            stubGameInput = MockRepository.GenerateStub<IGameInput>();
            stubCrosshair = MockRepository.GenerateStub<ICrosshair>();
            stubRespawner = MockRepository.GenerateStub<IPlayerRespawner>();
            kpc = new KeyboardPlayerController(stubPlayer, stubGameInput, stubCrosshair, stubRespawner);
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
            var stubWeapons = MockRepository.GenerateStub<IWeapons>();
            stubPlayer.Stub(me => me.Weapons).Return(stubWeapons);

            kpc.RemoveDeadProjectiles();

            stubWeapons.AssertWasCalled(me => me.RemoveDeadProjectiles());
        }

        [Test]
        public void ShouldJumpWhenJumpKeyIsPressed()
        {
            stubGameInput.Stub(gi => gi.IsGameKeyDown(Arg<GameKey>.Is.Equal(GameKey.Jump))).Return(true);

            kpc.Process(1);

            stubPlayer.AssertWasCalled(p => p.Jump());
        }

        [Test]
        public void ShouldNotLetPlayerDoubleJump()
        {
            var lessThanJumpDelay = KeyboardPlayerController.JumpTimer / 3;
            var moreThanJumpDelay = KeyboardPlayerController.JumpTimer - (lessThanJumpDelay / 2);
            int callCount = 0;
            stubGameInput.Stub(gi => gi.IsGameKeyDown(Arg<GameKey>.Is.Equal(GameKey.Jump))).Return(true);
            stubPlayer.Stub(p => p.Jump()).Do(new Func<bool>(() => { callCount++; return true; }));

            kpc.Process(KeyboardPlayerController.JumpTimer);

            Assert.AreEqual(1, callCount);
            kpc.Process(lessThanJumpDelay);
            Assert.AreEqual(1, callCount);
            kpc.Process(moreThanJumpDelay);
            Assert.AreEqual(2, callCount);
        }

        [Test]
        public void ShouldMoveLeftWhenMoveLeftKeyIsPressed()
        {
            stubGameInput.Stub(gi => gi.IsGameKeyDown(Arg<GameKey>.Is.Equal(GameKey.MoveLeft))).Return(true);

            kpc.Process(1);

            stubPlayer.AssertWasCalled(p => p.MoveLeft());
        }

        [Test]
        public void ShouldMoveRightWhenMoveRightKeyIsPressed()
        {
            stubGameInput.Stub(gi => gi.IsGameKeyDown(Arg<GameKey>.Is.Equal(GameKey.MoveRight))).Return(true);

            kpc.Process(1);

            stubPlayer.AssertWasCalled(p => p.MoveRight());
        }

        [Test]
        public void ShouldCreatePendingShotOnAlivePlayersWhenPressingShootButton()
        {
            stubGameInput.Stub(gi => gi.IsGameKeyDown(Arg<GameKey>.Is.Equal(GameKey.Shoot))).Return(true);
            stubCrosshair.Stub(c => c.WorldPosition).Return(Vector2.UnitX);
            stubPlayer.Status = PlayerStatus.Alive;

            kpc.Process(1);

            Assert.AreEqual(Vector2.UnitX, stubPlayer.PendingShot);           
        }
        [Test]
        public void ShouldNotLetPlayerShootAgainBeforeReloaded()
        {
            var lessThanShootDelay = KeyboardPlayerController.ShootTimer / 2;
            var moreThanShootDelay = KeyboardPlayerController.ShootTimer - (lessThanShootDelay / 3);
            stubGameInput.Stub(gi => gi.IsGameKeyDown(Arg<GameKey>.Is.Equal(GameKey.Shoot))).Return(true);
            stubCrosshair.Stub(c => c.WorldPosition).Return(Vector2.UnitX);

            kpc.Process(KeyboardPlayerController.ShootTimer);
            stubPlayer.PendingShot = Vector2.Zero;

            kpc.Process(lessThanShootDelay);
            Assert.AreEqual(Vector2.Zero, stubPlayer.PendingShot);
            kpc.Process(moreThanShootDelay);
            Assert.AreEqual(Vector2.UnitX, stubPlayer.PendingShot);
        }

        [Test]
        public void ShouldRespawnDeadPlayersWhenPressingShootKey()
        {
            stubGameInput.Stub(gi => gi.IsGameKeyDown(Arg<GameKey>.Is.Equal(GameKey.Shoot))).Return(true);
            stubPlayer.Status = PlayerStatus.Dead;

            kpc.Process(100);

            stubRespawner.AssertWasCalled(me => me.RespawnPlayer(stubPlayer));
        }
        [Test]
        public void ShouldStartTimerWhenRespawning()
        {
            stubGameInput.Stub(gi => gi.IsGameKeyDown(Arg<GameKey>.Is.Equal(GameKey.Shoot))).Return(true);
            stubPlayer.Status = PlayerStatus.Dead;
            stubPlayer.PendingShot = null;

            kpc.Process(1);
            stubPlayer.Status = PlayerStatus.Alive;
            kpc.Process(KeyboardPlayerController.ShootTimer / 2);

            Assert.IsNull(stubPlayer.PendingShot);
        }

        // WEAPON SWITCHING:
        [Test]
        public void ShouldChangeCurrentWeaponWhenButtonIsPressed()
        {
            var mockWeapons = MockRepository.GenerateStub<IWeapons>();
            stubPlayer.Stub(player => player.Weapons).Return(mockWeapons);
            stubGameInput.Stub(gi => gi.IsGameKeyDown(Arg<GameKey>.Is.Equal(GameKey.RocketLauncher))).Return(true);

            kpc.Process(1);

            mockWeapons.AssertWasCalled(weapons => weapons.ChangeWeapon(WeaponType.RocketLauncher));
            mockWeapons.AssertWasNotCalled(weapons => weapons.ChangeWeapon(WeaponType.RailGun));
        }
    }
}
