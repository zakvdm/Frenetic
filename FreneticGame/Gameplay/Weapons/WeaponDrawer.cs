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
                    weaponView.DrawEffects(translationMatrix);
                }
            }

            UpdatePlayerAfterDraw();
        }

        #endregion

        private void UpdatePlayerAfterDraw()
        {
            // Once we've drawn explosions, we clear the dead projectiles.
            this.playerController.RemoveDeadProjectiles();
        }

        IPlayerController playerController;
        IPlayerList playerList;
        List<IWeaponView> weaponViews;
    }
}
