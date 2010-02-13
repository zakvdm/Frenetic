using System;
using ProjectMercury.Renderers;
using ProjectMercury.Emitters;
using Microsoft.Xna.Framework;

namespace Frenetic.Graphics.Effects
{
    public class MercuryLineParticleEffect : ILineEffect
    {
        public MercuryLineParticleEffect(Renderer renderer, Emitter emitter)
        {
            _renderer = renderer;
            _emitter = (LineEmitter)emitter;
        }

        public Vector2 Position { get; set; }
        public int Length
        {
            get { return _emitter.Length; }
            set { _emitter.Length = value; }
        }
        public float Angle
        {
            get { return _emitter.Angle; }
            set { _emitter.Angle = value; }
        }
        public int ShotsDrawn { get; set; }

        public void Trigger(EffectType effectType)
        {
            _emitter.Trigger(this.Position);
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
