using System;
using Autofac.Builder;
using Frenetic.Weapons;
using ProjectMercury.Renderers;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using ProjectMercury.Emitters;
using Microsoft.Xna.Framework.Content;
using Frenetic.Graphics.Effects;
using Autofac;

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
                    Renderer renderer = new SpriteBatchRenderer() { BlendMode = SpriteBlendMode.Additive, GraphicsDeviceService = this.GraphicsDeviceService };
                    //Renderer renderer = new SpriteBatchRenderer() { BlendMode = SpriteBlendMode.AlphaBlend, GraphicsDeviceService = this.GraphicsDeviceService };
                    renderer.LoadContent(this.ContentManager);
                    return renderer;
                }).SingletonScoped();

            builder.Register((container, parameters) =>
                {
                    var emitterName = parameters.TypedAs<string>();
                    Emitter emitter = this.ContentManager.Load<Emitter>("Effects\\" + emitterName);
                    emitter.LoadContent(this.ContentManager);
                    emitter.Initialize();
                    System.Diagnostics.Debug.Assert(emitter.ParticleTexture != null, "Emitter MUST have a texture otherwise nothing will be drawn!", "Probably need to specify the ParticleTextureAssetName xml tag in the Effect .em file.");
                    return emitter;
                }).FactoryScoped();

            builder.Register<EffectUpdater>().ContainerScoped();

            builder.Register<MercuryParticleEffect>().As<IEffect>().FactoryScoped();
            builder.Register<MercuryPointParticleEffect>
                ( c => new MercuryPointParticleEffect
                    (
                        c.Resolve<Renderer>(),
                        c.Resolve<Emitter>(new TypedParameter(typeof(string), "point")),
                        c.Resolve<Emitter>(new TypedParameter(typeof(string), "explosion"))
                    )).As<IEffect>().SingletonScoped();
            //builder.RegisterGeneratedFactory<Frenetic.Graphics.Effects.Effect.Factory>(new TypedService(typeof(IEffect)));

            // WEAPONS:
            builder.Register<RailGun>().As<IWeapon>().FactoryScoped();
            builder.Register<RocketLauncher>().As<IWeapon>().FactoryScoped();
            
            builder.Register<Rocket>().FactoryScoped();
            builder.RegisterGeneratedFactory<Rocket.Factory>(new TypedService(typeof(Rocket)));

            builder.Register<RailGunView>().As<IWeaponView>().FactoryScoped();
            builder.Register<RocketLauncherView>().As<IWeaponView>().FactoryScoped();
        }
    }
}
