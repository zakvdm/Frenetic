using System;
using Frenetic.Graphics.Effects;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Frenetic.Player;

namespace Frenetic.Gameplay.Weapons
{
    public class RailGunView : IWeaponView
    {
        public RailGunView(IPlayerList playerList, ILineEffect lineEffect)
        {
            this.playerList = playerList;
            this.lineEffect = lineEffect;
        }

        #region IRailGunView Members

        public void DrawWeapon(IWeapons weapons)
        {
            var railGun = weapons[WeaponType.RailGun] as RailGun;

            DrawRailgun(weapons, railGun);
        }

        public void DrawEffects(Matrix translationMatrix)
        {
            this.lineEffect.Draw(ref translationMatrix);
        }

        #endregion

        void DrawRailgun(IWeapons weapons, RailGun railgun)
        {
            if (weapons.Shots.Count > this.lineEffect.ShotsDrawn)
            {
                this.lineEffect.ShotsDrawn = weapons.Shots.Count;

                Shot shot = weapons.Shots[this.lineEffect.ShotsDrawn - 1];

                SetAndTriggerEffectParameters(shot);
            }
        }

        public void SetAndTriggerEffectParameters(Shot shot)
        {
            Vector2 lineAtOrigin = (shot.EndPoint - shot.StartPoint);
            Vector2 midPointFromOrigin = lineAtOrigin / 2;
            float length = lineAtOrigin.Length();
            float angle = (float)Math.Atan2(lineAtOrigin.Y, lineAtOrigin.X);
            Vector2 midPoint = midPointFromOrigin + shot.StartPoint;

            this.lineEffect.Position = midPoint;
            this.lineEffect.Length = Math.Max((int)length, 1);
            this.lineEffect.Angle = angle;

            this.lineEffect.Trigger(EffectType.Rail);
        }

        IPlayerList playerList;
        ILineEffect lineEffect;
    }
}
