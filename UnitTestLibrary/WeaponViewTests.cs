using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Frenetic.Weapons;
using Frenetic.Player;
using Rhino.Mocks;
using Frenetic.Graphics;

namespace UnitTestLibrary
{
    [TestFixture]
    public class WeaponViewTests
    {
        IPlayerList playerList;
        RailGunView railGunView;
        RocketLauncher rocketLauncherView;
        bool effectFactoryWasUsed;
        IPlayer stubPlayer;
        [SetUp]
        public void SetUp()
        {
            playerList = MockRepository.GenerateStub<IPlayerList>();
            effectFactoryWasUsed = false;
            stubPlayer = MockRepository.GenerateStub<IPlayer>();
            stubPlayer.Stub(me => me.CurrentWeapon).Return(MockRepository.GenerateStub<IWeapon>());

            railGunView = new RailGunView(playerList, () => { effectFactoryWasUsed = true; return MockRepository.GenerateStub<IEffect>(); });
            rocketLauncherView = new RocketLauncherView(playerList);
        }
        [Test]
        public void WeaponViewRegistersForPlayerAddedEvent()
        {
            playerList.AssertWasCalled(me => me.PlayerAdded += Arg<Action<IPlayer>>.Is.Anything);
        }
        [Test]
        public void AddsANewRailGunForNewPlayers()
        {
            playerList.Raise(me => me.PlayerAdded += null, stubPlayer);

            Assert.IsTrue(effectFactoryWasUsed);
        }

        [Test]
        public void AddsANewRocketLauncherForNewPlayers()
        {
            playerList.Raise(me => me.PlayerAdded += null, stubPlayer);

            throw new NotImplementedException();
        }
    }
}
