using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Frenetic.Gameplay.Weapons;
using Frenetic.Player;
using Rhino.Mocks;
using Frenetic.Graphics.Effects;
using Microsoft.Xna.Framework;

namespace UnitTestLibrary
{
    [TestFixture]
    public class WeaponDrawerTests
    {
        IPlayerController stubPlayerController;
        IPlayerList playerList;
        List<IWeaponView> weaponViews;
        WeaponDrawer view;
        [SetUp]
        public void SetUp()
        {
            stubPlayerController = MockRepository.GenerateStub<IPlayerController>();
            playerList = new PlayerList();
            playerList.Add(MockRepository.GenerateStub<IPlayer>());
            playerList.Players[0].Stub(me => me.Weapons).Return(MockRepository.GenerateStub<IWeapons>());
            playerList.Add(MockRepository.GenerateStub<IPlayer>());
            playerList.Players[1].Stub(me => me.Weapons).Return(MockRepository.GenerateStub<IWeapons>());
            weaponViews = new List<IWeaponView>();
            weaponViews.Add(MockRepository.GenerateStub<IWeaponView>());
            weaponViews.Add(MockRepository.GenerateStub<IWeaponView>());
            view = new WeaponDrawer(stubPlayerController, playerList, weaponViews);
        }
        [Test]
        public void ShouldCallDrawForEachWeaponViewForEachPlayer()
        {
            view.Draw(Matrix.Identity);

            foreach (var player in playerList)
            {
                weaponViews[0].AssertWasCalled(me => me.DrawWeapon(player.Weapons));
                weaponViews[1].AssertWasCalled(me => me.DrawWeapon(player.Weapons));
            }
        }
        [Test]
        public void ShouldCallDrawEffectsForEachWeaponView()
        {
            var translationMatrix = Matrix.CreateTranslation(new Vector3(100, 200, 300));
            view.Draw(translationMatrix);

            weaponViews[0].AssertWasCalled(me => me.DrawEffects(translationMatrix));
            weaponViews[1].AssertWasCalled(me => me.DrawEffects(translationMatrix));
        }
        [Test]
        public void ShouldClearDeadProjectiles()
        {
            view.Draw(Matrix.Identity);

            stubPlayerController.AssertWasCalled(me => me.RemoveDeadProjectiles());
        }
    }
}
