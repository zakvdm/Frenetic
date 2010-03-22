using System;
using Frenetic.Player;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Frenetic.Gameplay.Weapons
{
    public interface IWeaponDrawer
    {
        void Draw(Matrix translationMatrix);
    }
    public class WeaponDrawer : IWeaponDrawer
    {
        public WeaponDrawer(IPlayerController playerController, IPlayerList playerList, List<IWeaponView> weaponViews)
        {
            this.playerController = playerController;
            this.playerList = playerList;
            this.weaponViews = weaponViews;
        }

        #region IWeaponView Members

        public void Draw(Matrix translationMatrix)
        {
            foreach (var player in this.playerList)
            {
                foreach (var weaponView in weaponViews)
                {
                    weaponView.DrawWeapon(player.Weapons);
                }
            
                UpdatePlayerWeaponsAfterDraw(player.Weapons);
            }
            foreach (var weaponView in weaponViews)
            {
                weaponView.DrawEffects(translationMatrix);
            }

        }

        #endregion

        private void UpdatePlayerWeaponsAfterDraw(IWeapons weapons)
        {
            // Once we've drawn explosions, we clear the dead projectiles.
            weapons.RemoveDeadProjectiles();
        }

        IPlayerController playerController;
        IPlayerList playerList;
        List<IWeaponView> weaponViews;
    }
}
