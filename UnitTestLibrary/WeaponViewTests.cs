using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Frenetic.Weapons;
using Frenetic.Player;
using Rhino.Mocks;
using Frenetic.Graphics.Effects;

namespace UnitTestLibrary
{
    [TestFixture]
    public class WeaponViewTests
    {
        IPlayerList playerList;
        RailGunView railGunView;
        bool effectFactoryWasUsed;
        IPlayer stubPlayer;
        [SetUp]
        public void SetUp()
        {
            playerList = MockRepository.GenerateStub<IPlayerList>();
            effectFactoryWasUsed = false;
            stubPlayer = MockRepository.GenerateStub<IPlayer>();
            stubPlayer.Stub(me => me.CurrentWeapon).Return(MockRepository.GenerateStub<IWeapon>());

            Effect.Factory factoryMethod = () => { effectFactoryWasUsed = true; return MockRepository.GenerateStub<IEffect>(); };
            railGunView = new RailGunView(playerList, factoryMethod);
        }
        [Test]
        public void AddsANewRailGunForNewPlayers()
        {
            playerList.Raise(me => me.PlayerAdded += null, stubPlayer);

            Assert.IsTrue(effectFactoryWasUsed);
        }
    }
}
