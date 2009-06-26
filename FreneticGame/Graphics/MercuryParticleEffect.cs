using System;
using ProjectMercury.Renderers;
using ProjectMercury.Emitters;
using Microsoft.Xna.Framework;

namespace Frenetic.Graphics
{
    public class MercuryParticleEffect : IEffect
    {
        public MercuryParticleEffect(Renderer renderer, Emitter emitter)
        {
            _renderer = renderer;
            _emitter = (LineEmitter)emitter;
        }

        public void Trigger(Vector2 startPoint, Vector2 endPoint)
        {
            Vector2 lineAtOrigin = (endPoint - startPoint);
            Vector2 midPointFromOrigin = lineAtOrigin / 2;
            float length = lineAtOrigin.Length();
            float angle = (float)Math.Atan2(lineAtOrigin.Y, lineAtOrigin.X);
            Vector2 midPoint = midPointFromOrigin + startPoint;

            _emitter.Length = (int)length;
            _emitter.Angle = angle;

            _emitter.Trigger(midPoint);
        }

        public void Draw(ref Matrix transform)
        {
            _renderer.RenderEmitter(_emitter, ref transform);
        }

        public void Update(float totalSeconds, float elapsedSeconds)
        {
            //_emitter.Update((float)gameTime.TotalGameTime.TotalSeconds, (float)gameTime.ElapsedGameTime.TotalSeconds);
            _emitter.Update(totalSeconds, elapsedSeconds);
        }

        Renderer _renderer;
        LineEmitter _emitter;
    }
}
