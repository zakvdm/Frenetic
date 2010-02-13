using System;
using Microsoft.Xna.Framework;

using Frenetic.Player;
using Frenetic.Graphics.Effects;
using System.Collections.Generic;

namespace Frenetic.Gameplay.Weapons
{
    public class RocketLauncherView : BaseWeaponView, IWeaponView
    {
        public RocketLauncherView(IPlayerController playerController, IPlayerList playerList, IEffect particleEffects)
            : base(playerList)
        {
            this.PlayerController = playerController;
            this.PlayerList = playerList;
            this.ParticleEffects = particleEffects;
        }
        #region IWeaponView Members

        public void Draw(Matrix translationMatrix)
        {
            foreach (var player in this.PlayerList)
            {
                var rocketLauncher = (RocketLauncher)player.Weapons[WeaponType.RocketLauncher];

                foreach (var rocket in rocketLauncher.Rockets)
                {
                    this.ParticleEffects.Position = rocket.Position;
                    if (rocket.IsAlive)
                    {
                        this.ParticleEffects.Trigger(EffectType.RocketTrail);
                    }
                    else
                    {
                        this.ParticleEffects.Trigger(EffectType.RocketExplosion);
                    }
                }

                this.ParticleEffects.Draw(ref translationMatrix);

                UpdatePlayerAfterDraw();
            }
        }

        #endregion

        private void UpdatePlayerAfterDraw()
        {
            // Once we've drawn explosions, we clear the dead projectiles.
            this.PlayerController.RemoveDeadProjectiles();
        }

        protected override void HandleNewPlayer(IPlayer newPlayer)
        {
            //this.PlayersList.Add(newPlayer, this.ParticleEffectFactory());
        }

        IPlayerController PlayerController;
        IPlayerList PlayerList;
        IEffect ParticleEffects;
    }
}
