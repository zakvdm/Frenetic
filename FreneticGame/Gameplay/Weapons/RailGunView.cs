using System;
using Frenetic.Graphics.Effects;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Frenetic.Player;

namespace Frenetic.Gameplay.Weapons
{
    public class RailGunView : IWeaponView
    {
        public RailGunView(ILineEffect lineEffect)
        {
            this.lineEffect = lineEffect;
        }

        #region IRailGunView Members

        public void DrawWeapon(IWeapons weapons)
        {
            var railGun = weapons[WeaponType.RailGun] as RailGun;

            foreach (var slug in railGun.Slugs)
            {
                SetAndTriggerEffectParameters(slug);
            }
        }

        public void DrawEffects(Matrix translationMatrix)
        {
            this.lineEffect.Draw(ref translationMatrix);
        }

        #endregion

        public void SetAndTriggerEffectParameters(Slug slug)
        {
            Vector2 lineAtOrigin = (slug.EndPoint - slug.StartPoint);
            Vector2 midPointFromOrigin = lineAtOrigin / 2;
            float length = lineAtOrigin.Length();
            float angle = (float)Math.Atan2(lineAtOrigin.Y, lineAtOrigin.X);
            Vector2 midPoint = midPointFromOrigin + slug.StartPoint;

            this.lineEffect.Position = midPoint;
            this.lineEffect.Length = Math.Max((int)length, 1);
            this.lineEffect.Angle = angle;

            this.lineEffect.Trigger(EffectType.Rail);
        }

        ILineEffect lineEffect;
    }
}
