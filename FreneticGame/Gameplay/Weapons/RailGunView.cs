using System;
using Frenetic.Graphics;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Frenetic.Player;

namespace Frenetic.Weapons
{
    public class RailGunView : IRailGunView
    {
        public RailGunView(IPlayerList playerList, MercuryParticleEffect.Factory particleEffectFactory)
        {
            _playersList = playerList;
            _particleEffectFactory = particleEffectFactory;
        }

        #region IRailGunView Members

        public void Draw(Matrix translationMatrix)
        {
            LookForNewPlayers();

            foreach (KeyValuePair<IWeapon, MercuryParticleEffect> railGunInfo in _railGuns)
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
            foreach (IPlayer player in _playersList.Players)
            {
                if (!_railGuns.ContainsKey(player.CurrentWeapon))
                {
                    _railGuns.Add(player.CurrentWeapon, _particleEffectFactory());
                }
            }
        }

        IPlayerList _playersList;
        MercuryParticleEffect.Factory _particleEffectFactory;

        Dictionary<IWeapon, MercuryParticleEffect> _railGuns = new Dictionary<IWeapon, MercuryParticleEffect>();
    }
}
