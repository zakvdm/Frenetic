using System;
using ProjectMercury.Renderers;
using ProjectMercury.Emitters;
using Microsoft.Xna.Framework;

namespace Frenetic.Graphics.Effects
{
    public class MercuryPointParticleEffect : IEffect
    {
        public MercuryPointParticleEffect(Renderer renderer, Emitter trailEmitter, Emitter explosionEmitter)
        {
            this.Renderer = renderer;
            this.TrailEmitter = trailEmitter;
            this.ExplosionEmitter = explosionEmitter;
        }

        public Vector2 Position { get; set; }
        public int ShotsDrawn { get; set; }

        public void Trigger(EffectType effectType)
        {
            switch (effectType)
            {
                case EffectType.RocketTrail:
                    {
                        this.TrailEmitter.Trigger(this.Position);
                        break;
                    }
                case EffectType.RocketExplosion:
                    {
                        this.ExplosionEmitter.Trigger(this.Position);
                        break;
                    }
            }
        }

        public void Draw(ref Matrix transform)
        {
            this.Renderer.RenderEmitter(this.TrailEmitter, ref transform);
            this.Renderer.RenderEmitter(this.ExplosionEmitter, ref transform);
        }

        public void Update(float totalSeconds, float elapsedSeconds)
        {
            this.TrailEmitter.Update(totalSeconds, elapsedSeconds);
            this.ExplosionEmitter.Update(totalSeconds, elapsedSeconds);
        }

        Renderer Renderer;
        Emitter TrailEmitter;
        Emitter ExplosionEmitter;
    }
}