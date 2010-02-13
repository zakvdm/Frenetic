using System;
using Microsoft.Xna.Framework;

namespace Frenetic.Graphics.Effects
{
    public enum EffectType
    {
        Rail,
        RocketTrail,
        RocketExplosion
    }
    public interface IEffect
    {
        [Obsolete]
        int ShotsDrawn { get; set; }

        Vector2 Position { get; set; }
        void Trigger(EffectType effectType);

        void Draw(ref Matrix transform);
        void Update(float totalSeconds, float elapsedSeconds);
    }
    public interface ILineEffect : IEffect
    {
        int Length { get; set; }
        float Angle { get; set; }
    }
    public class LineEffect
    {
        public delegate ILineEffect Factory();
    }
}
