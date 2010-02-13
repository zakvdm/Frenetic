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
        IPlayer stubPlayer;
        PlayerList playerList;
        ILineEffect mockEffect;
        IPlayerController mockPlayerController;

        RailGunView view;

        [SetUp]
        public void Setup()
        {
            stubPlayer = MockRepository.GenerateStub<IPlayer>();
            stubPlayer.Stub(me => me.Weapons).Return(MockRepository.GenerateStub<IWeapons>());
            stubPlayer.Weapons.Stub(me => me[WeaponType.RailGun]).Return(new RailGun(null));
            playerList = new PlayerList() { stubPlayer };
            mockEffect = MockRepository.GenerateStub<ILineEffect>();
            mockPlayerController = MockRepository.GenerateStub<IPlayerController>();
            view = new RailGunView(playerList, mockEffect);
        }

        [Test]
        public void ShouldSetLineEffectParametersCorrectly()
        {
            var startPoint = Vector2.One;
            var endPoint = new Vector2(3, 5);

            view.SetAndTriggerEffectParameters(new Shot(startPoint, endPoint));

            Assert.AreEqual(startPoint + ((endPoint - startPoint) / 2), mockEffect.Position);
            Assert.AreEqual(1.1f, mockEffect.Angle, 0.05f);
            Assert.AreEqual(4, mockEffect.Length);
            mockEffect.AssertWasCalled(effect => effect.Trigger(EffectType.Rail));
        }
    }
}
