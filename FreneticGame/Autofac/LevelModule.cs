using System;
using Autofac;
using Autofac.Builder;
using Frenetic.Gameplay.Level;
using Microsoft.Xna.Framework;
using Frenetic.Physics;

namespace Frenetic.Autofac
{
    public class LevelModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register((container, parameters) =>
            {
                var pos = parameters.Named<Vector2>("position");
                var size = parameters.Named<Vector2>("size");
                var physics = container.Resolve<IPhysicsComponent>(new NamedParameter("size", size));
                physics.Position = pos;
                return new LevelPiece(physics);
            }).FactoryScoped();

            builder.RegisterGeneratedFactory<LevelPiece.Factory>(new TypedService(typeof(LevelPiece)));
            builder.Register<DumbLevelLoader>().As<ILevelLoader>().ContainerScoped();
            builder.Register<Frenetic.Gameplay.Level.Level>().As<ILevel>().ContainerScoped();
            builder.Register<LevelController>().ContainerScoped();
            builder.Register<LevelView>().ContainerScoped();

            builder.Register<VisibilityView>().ContainerScoped();

            builder.Register<PlayerRespawner>().As<IPlayerRespawner>().SingletonScoped();
        }
    }
}
