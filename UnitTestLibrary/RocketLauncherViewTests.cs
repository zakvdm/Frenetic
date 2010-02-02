using System;

using Frenetic;

using NUnit.Framework;
using Rhino.Mocks;
using Microsoft.Xna.Framework;
using Frenetic.Player;
using Frenetic.Weapons;
using Frenetic.Graphics.Effects;

namespace UnitTestLibrary
{
    [TestFixture]
    public class RocketLauncherViewTests
    {
        IPlayer stubPlayer;
        PlayerList playerList;
        IEffect mockEffects;
        IPlayerController mockPlayerController;

        RocketLauncherView view;

        [SetUp]
        public void Setup()
        {
            stubPlayer = MockRepository.GenerateStub<IPlayer>();
            stubPlayer.Stub(me => me.CurrentWeapon).Return(new RocketLauncher(null));
            ((RocketLauncher)stubPlayer.CurrentWeapon).Rockets.Add(new Rocket(Vector2.Zero, Vector2.Zero, new Frenetic.Physics.DummyPhysicsComponent()));
            playerList = new PlayerList() { stubPlayer };
            mockEffects = MockRepository.GenerateStub<IEffect>();
            mockPlayerController = MockRepository.GenerateStub<IPlayerController>();
            view = new RocketLauncherView(mockPlayerController, playerList, mockEffects);
        }
        [Test]
        public void ShouldDrawTrailForRocketsThatAreAlive()
        {
            view.Draw(Matrix.Identity);

            mockEffects.AssertWasCalled(me => me.Trigger(EffectType.RocketTrail));
        }
        [Test]
        public void ShouldDrawExplosionWhenRocketsAreNotAlive()
        {
            ((RocketLauncher)stubPlayer.CurrentWeapon).Rockets[0].IsAlive = false;

            view.Draw(Matrix.Identity);

            mockEffects.AssertWasCalled(me => me.Trigger(EffectType.RocketExplosion));
        }
        [Test]
        public void ShouldClearDeadProjectilesAfterDrawingExplosions()
        {
            ((RocketLauncher)stubPlayer.CurrentWeapon).Rockets[0].IsAlive = false;

            view.Draw(Matrix.Identity);

            mockPlayerController.AssertWasCalled(me => me.RemoveDeadProjectiles());
        }
    }
}
