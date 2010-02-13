﻿using System;
using Microsoft.Xna.Framework;

using Frenetic.Player;
using Frenetic.Graphics.Effects;
using System.Collections.Generic;

namespace Frenetic.Gameplay.Weapons
{
    public class RocketLauncherView : IWeaponView
    {
        public RocketLauncherView(IPlayerController playerController, IPlayerList playerList, IEffect particleEffects)
        {
            this.PlayerController = playerController;
            this.PlayerList = playerList;
            this.ParticleEffects = particleEffects;
        }
        #region IWeaponView Members

        public void DrawWeapon(IWeapons weapons)
        {
            var rocketLauncher = weapons[WeaponType.RocketLauncher] as RocketLauncher;

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
        }

        public void DrawEffects(Matrix translationMatrix)
        {
            this.ParticleEffects.Draw(ref translationMatrix);
        }

        #endregion


        IPlayerController PlayerController;
        IPlayerList PlayerList;
        IEffect ParticleEffects;

        #region IWeaponView Members

        public void DrawForPlayer(IPlayer player)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
