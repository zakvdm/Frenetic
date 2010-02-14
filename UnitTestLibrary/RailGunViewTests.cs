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
    public class RailGunViewTests
    {
        IWeapons stubWeapons;
        PlayerList playerList;
        ILineEffect mockEffect;
        IPlayerController mockPlayerController;

        RailGunView view;

        [SetUp]
        public void Setup()
        {
            stubWeapons = MockRepository.GenerateStub<IWeapons>();
            stubWeapons.Stub(me => me[WeaponType.RailGun]).Return(new RailGun(null));
            mockEffect = MockRepository.GenerateStub<ILineEffect>();
            view = new RailGunView(mockEffect);
        }

        [Test]
        public void ShouldDrawAllSlugsOnRailgun()
        {
            var railGun = stubWeapons[WeaponType.RailGun] as RailGun;
            railGun.Slugs.Add(new Slug(Vector2.Zero, Vector2.One));
            railGun.Slugs.Add(new Slug(Vector2.Zero, Vector2.One));

            view.DrawWeapon(stubWeapons);

            mockEffect.AssertWasCalled(effect => effect.Trigger(EffectType.Rail), constraint => constraint.Repeat.Twice());
        }

        [Test]
        public void ShouldSetLineEffectParametersCorrectly()
        {
            var startPoint = Vector2.One;
            var endPoint = new Vector2(3, 5);

            view.SetAndTriggerEffectParameters(new Slug(startPoint, endPoint));

            Assert.AreEqual(startPoint + ((endPoint - startPoint) / 2), mockEffect.Position);
            Assert.AreEqual(1.1f, mockEffect.Angle, 0.05f);
            Assert.AreEqual(4, mockEffect.Length);
            mockEffect.AssertWasCalled(effect => effect.Trigger(EffectType.Rail));
        }
    }
}
