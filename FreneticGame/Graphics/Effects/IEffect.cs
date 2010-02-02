using System;
using Microsoft.Xna.Framework;

namespace Frenetic.Graphics.Effects
{
    public enum EffectType
    {
        RocketTrail,
        RocketExplosion
    }
    public interface IEffect
    {
        [Obsolete]
        int ShotsDrawn { get; set; }

        Vector2 Position { get; set; }
        void Trigger(EffectType effectType);

        [Obsolete]
        void Trigger(Vector2 startPoint, Vector2 endPoint);
        void Draw(ref Matrix transform);
        void Update(float totalSeconds, float elapsedSeconds);
    }
    public class Effect
    {
        public delegate IEffect Factory();
    }
}
