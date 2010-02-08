using System;
using Autofac.Builder;
using Frenetic.Player;
using System.Collections.Generic;
using Frenetic.Physics;
using Frenetic.Graphics;
using Autofac;
using Frenetic.Gameplay.Weapons;
using Frenetic.Engine;

namespace Frenetic.Autofac
{
    public class PlayerModule : Module
    {
        public int ScreenWidth { get; set; }
        public int ScreenHeight { get; set; }

        protected override void Load(ContainerBuilder builder)
        {
            builder.Register<PlayerUpdater>().ContainerScoped();
            builder.Register<NetworkPlayerSettings>().As<IPlayerSettings>().FactoryScoped();
            builder.Register<LocalPlayerSettings>().SingletonScoped();

            builder.Register<NetworkPlayer>().As<IPlayer>().FactoryScoped();
            builder.Register((c,p) => new LocalPlayer(c.Resolve<LocalPlayerSettings>(), c.Resolve<LocalPlayerFarseerPhysicsComponent>(), c.Resolve<IBoundaryCollider>(), c.Resolve<IWeapon>(), c.Resolve<IWeapons>(), c.Resolve<ITimer>())).As<LocalPlayer>().FactoryScoped();

            builder.Register<PlayerList>().As<IPlayerList>().ContainerScoped();
            builder.Register<PlayerView>().ContainerScoped();
            builder.Register((c) => (IBoundaryCollider)new WorldBoundaryCollider(this.ScreenWidth, this.ScreenHeight));
            builder.Register<KeyboardPlayerController>().As<IPlayerController>().ContainerScoped();

            builder.Register<XnaTextureBank<PlayerTexture>>().As<ITextureBank<PlayerTexture>>().SingletonScoped();
        }
    }
}
