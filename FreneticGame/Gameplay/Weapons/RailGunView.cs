using System;
using Frenetic.Graphics.Effects;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Frenetic.Player;

namespace Frenetic.Gameplay.Weapons
{
    public class RailGunView : BaseWeaponView, IWeaponView
    {
        public RailGunView(IPlayerList playerList, Effect.Factory particleEffectFactory) : base(playerList)
        {
            this.ParticleEffectFactory = particleEffectFactory;
        }

        #region IRailGunView Members

        public void Draw(Matrix translationMatrix)
        {
            foreach (KeyValuePair<IWeapons, IEffect> railGunInfo in this.railGuns)
            {
                var railGun = railGunInfo.Key;
                var particleEffect = railGunInfo.Value;

                if (railGun.Shots.Count > particleEffect.ShotsDrawn)
                {
                    particleEffect.ShotsDrawn = railGun.Shots.Count;

                    Shot shot = railGun.Shots[particleEffect.ShotsDrawn - 1];
                    particleEffect.Trigger(shot.StartPoint, shot.EndPoint);
                }

                particleEffect.Draw(ref translationMatrix);
            }
        }

        #endregion

        protected override void HandleNewPlayer(IPlayer newPlayer)
        {
            this.railGuns.Add(newPlayer.Weapons, this.ParticleEffectFactory());
        }

        Effect.Factory ParticleEffectFactory;

        Dictionary<IWeapons, IEffect> railGuns = new Dictionary<IWeapons, IEffect>();
    }
}
