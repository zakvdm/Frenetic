using System;
using Frenetic.Graphics;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Frenetic.Player;

namespace Frenetic.Weapons
{
    public class RailGunView : IRailGunView
    {
        public RailGunView(List<IPlayer> playerList, MercuryParticleEffect.Factory particleEffectFactory)
        {
            _players = playerList;
            _particleEffectFactory = particleEffectFactory;
        }

        #region IRailGunView Members

        public void Draw(Matrix translationMatrix)
        {
            LookForNewPlayers();

            foreach (KeyValuePair<IRailGun, MercuryParticleEffect> railGunInfo in _railGuns)
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

        void LookForNewPlayers()
        {
            foreach (IPlayer player in _players)
            {
                if (!_railGuns.ContainsKey(player.CurrentWeapon))
                {
                    _railGuns.Add(player.CurrentWeapon, _particleEffectFactory());
                }
            }
        }

        List<IPlayer> _players;
        MercuryParticleEffect.Factory _particleEffectFactory;

        Dictionary<IRailGun, MercuryParticleEffect> _railGuns = new Dictionary<IRailGun, MercuryParticleEffect>();
    }
}
