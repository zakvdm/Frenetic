using System;
using ProjectMercury.Renderers;
using ProjectMercury.Emitters;
using Microsoft.Xna.Framework;

namespace Frenetic.Graphics.Effects
{
    public class MercuryParticleEffect : IEffect
    {
        public MercuryParticleEffect(Renderer renderer, Emitter emitter)
        {
            _renderer = renderer;
            _emitter = (LineEmitter)emitter;
        }

        public Vector2 Position { get; set; }
        public int ShotsDrawn { get; set; }

        public void Trigger(EffectType effectType)
        {
            throw new NotImplementedException();
        }
        public void Trigger(Vector2 startPoint, Vector2 endPoint)
        {
            Vector2 lineAtOrigin = (endPoint - startPoint);
            Vector2 midPointFromOrigin = lineAtOrigin / 2;
            float length = lineAtOrigin.Length();
            float angle = (float)Math.Atan2(lineAtOrigin.Y, lineAtOrigin.X);
            Vector2 midPoint = midPointFromOrigin + startPoint;

            _emitter.Length = Math.Max((int)length, 1);
            _emitter.Angle = angle;

            _emitter.Trigger(midPoint);
        }

        public void Draw(ref Matrix transform)
        {
            _renderer.RenderEmitter(_emitter, ref transform);
        }

        public void Update(float totalSeconds, float elapsedSeconds)
        {
            _emitter.Update(totalSeconds, elapsedSeconds);
        }

        Renderer _renderer;
        LineEmitter _emitter;
    }
}
