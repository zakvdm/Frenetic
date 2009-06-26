using System;
using Autofac.Builder;
using Frenetic.Weapons;
using ProjectMercury.Renderers;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using ProjectMercury.Emitters;
using Microsoft.Xna.Framework.Content;
using Frenetic.Graphics;

namespace Frenetic.Autofac
{
    public class WeaponsModule : Module
    {
        public GraphicsDeviceManager GraphicsDeviceService { get; set; }
        public ContentManager ContentManager { get; set; }

        protected override void Load(ContainerBuilder builder)
        {
            builder.Register((container, parameters) =>
                {
                    //Renderer renderer = new SpriteBatchRenderer() { BlendMode = SpriteBlendMode.Additive, GraphicsDeviceService = this.GraphicsDeviceService };
                    Renderer renderer = new SpriteBatchRenderer() { BlendMode = SpriteBlendMode.AlphaBlend, GraphicsDeviceService = this.GraphicsDeviceService };
                    renderer.LoadContent(this.ContentManager);
                    return renderer;
                }).SingletonScoped();

            builder.Register((container, parameters) =>
                {
                    Emitter emitter = this.ContentManager.Load<Emitter>("Effects\\line");
                    emitter.LoadContent(this.ContentManager);
                    emitter.Initialize();
                    return emitter;
                }).SingletonScoped();

            builder.Register<EffectUpdater>().ContainerScoped();

            builder.Register<MercuryParticleEffect>().SingletonScoped();
            builder.Register<MercuryParticleEffect>().As<IEffect>().SingletonScoped();

            // WEAPONS:
            builder.Register<RailGun>().As<IRailGun>().FactoryScoped();
            builder.Register<RailGunView>().As<IRailGunView>().ContainerScoped();
        }
    }
}
