using System;
using Frenetic.Graphics;
using Microsoft.Xna.Framework;

namespace Frenetic.Weapons
{
    public class RailGunView : IRailGunView
    {
        public RailGunView(MercuryParticleEffect mercuryParticleEffect)
        {
            _particleEffect = mercuryParticleEffect;
        }

        #region IRailGunView Members

        public void Draw(IRailGun railGun, Matrix translationMatrix)
        {
            if (railGun == null) return;

            if (railGun.Shots.Count > shot_count)
            {
                shot_count = railGun.Shots.Count;

                Shot shot = railGun.Shots[shot_count - 1];
                _particleEffect.Trigger(shot.StartPoint, shot.EndPoint);
            }

            _particleEffect.Draw(ref translationMatrix);
        }

        #endregion

        int shot_count = 0;

        MercuryParticleEffect _particleEffect;
    }
}
