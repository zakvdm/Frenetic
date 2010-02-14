using System;

using Frenetic;

using NUnit.Framework;
using Rhino.Mocks;
using Microsoft.Xna.Framework;
using Frenetic.Player;
using Frenetic.Gameplay.Weapons;
using Frenetic.Graphics.Effects;

namespace UnitTestLibrary
{
    [TestFixture]
    public class RocketLauncherViewTests
    {
        IWeapons stubWeapons;
        IEffect mockEffects;

        RocketLauncherView view;

        [SetUp]
        public void Setup()
        {
            stubWeapons = MockRepository.GenerateStub<IWeapons>();
            stubWeapons.Stub(me => me[WeaponType.RocketLauncher]).Return(new RocketLauncher(null));
            ((RocketLauncher)stubWeapons[WeaponType.RocketLauncher]).Rockets.Add(new Rocket(Vector2.Zero, Vector2.Zero, new Frenetic.Physics.DummyPhysicsComponent()));
            mockEffects = MockRepository.GenerateStub<IEffect>();
            view = new RocketLauncherView(mockEffects);
        }
        [Test]
        public void ShouldDrawTrailForRocketsThatAreAlive()
        {
            view.DrawWeapon(stubWeapons);

            mockEffects.AssertWasCalled(me => me.Trigger(EffectType.RocketTrail));
        }
        [Test]
        public void ShouldDrawExplosionWhenRocketsAreNotAlive()
        {
            ((RocketLauncher)stubWeapons[WeaponType.RocketLauncher]).Rockets[0].IsAlive = false;

            view.DrawWeapon(stubWeapons);

            mockEffects.AssertWasCalled(me => me.Trigger(EffectType.RocketExplosion));
        }
    }
}
