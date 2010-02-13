using System;
using Autofac.Builder;
using Frenetic.Gameplay.Weapons;
using ProjectMercury.Renderers;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using ProjectMercury.Emitters;
using Microsoft.Xna.Framework.Content;
using Frenetic.Graphics.Effects;
using Autofac;
using System.Collections.Generic;
using Frenetic.Player;

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

            builder.Register<MercuryLineParticleEffect>
                (c => new MercuryLineParticleEffect
                    (
                        c.Resolve<Renderer>(),
                        c.Resolve<Emitter>(new TypedParameter(typeof(string), "line"))
                    )).As<ILineEffect>().SingletonScoped();
            builder.Register<MercuryPointParticleEffect>
                (c => new MercuryPointParticleEffect
                    (
                        c.Resolve<Renderer>(),
                        c.Resolve<Emitter>(new TypedParameter(typeof(string), "point")),
                        c.Resolve<Emitter>(new TypedParameter(typeof(string), "explosion"))
                    )).As<IEffect>().SingletonScoped();
            builder.RegisterGeneratedFactory<Frenetic.Graphics.Effects.LineEffect.Factory>(new TypedService(typeof(ILineEffect)));
            builder.Register<EffectUpdater>().ContainerScoped();

            // WEAPONS:
            builder.Register<RailGun>().As<RailGun>().FactoryScoped();
            builder.Register<RocketLauncher>().As<RocketLauncher>().FactoryScoped();

            builder.Register((container) =>
                {
                    Dictionary<WeaponType, IWeapon> weapons = new Dictionary<WeaponType, IWeapon>();
                    weapons.Add(WeaponType.RailGun, container.Resolve<RailGun>());
                    weapons.Add(WeaponType.RocketLauncher, container.Resolve<RocketLauncher>());

                    return new WeaponList(weapons);
                }).As<IWeapons>().ContainerScoped();
            
            builder.Register<Rocket>().FactoryScoped();
            builder.RegisterGeneratedFactory<Rocket.Factory>(new TypedService(typeof(Rocket)));

            builder.Register<RocketLauncherView>().FactoryScoped();
            builder.Register<RailGunView>().FactoryScoped();

            builder.Register((container) =>
                {
                    List<IWeaponView> weaponViews = new List<IWeaponView>() { container.Resolve<RocketLauncherView>(), container.Resolve<RailGunView>() };
                    return new WeaponDrawer(container.Resolve<IPlayerController>(), container.Resolve<IPlayerList>(), weaponViews);
                }).As<IWeaponDrawer>().ContainerScoped();
        }
    }
}
