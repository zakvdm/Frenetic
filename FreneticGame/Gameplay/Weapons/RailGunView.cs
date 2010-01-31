using System;
using Frenetic.Graphics;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Frenetic.Player;

namespace Frenetic.Weapons
{
    public class RailGunView : BaseWeaponView, IWeaponView
    {
        public RailGunView(IPlayerList playerList, MercuryParticleEffect.Factory particleEffectFactory) : base(playerList)
        {
            _particleEffectFactory = particleEffectFactory;
        }

        #region IRailGunView Members

        public void Draw(Matrix translationMatrix)
        {
            foreach (KeyValuePair<IWeapon, IEffect> railGunInfo in this.RailGuns)
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
            this.RailGuns.Add(newPlayer.CurrentWeapon, _particleEffectFactory());
        }

        MercuryParticleEffect.Factory _particleEffectFactory;

        Dictionary<IWeapon, IEffect> RailGuns = new Dictionary<IWeapon, IEffect>();
    }
}
