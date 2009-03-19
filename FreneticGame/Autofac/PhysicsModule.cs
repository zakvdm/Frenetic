using System;
using Autofac.Builder;
using Autofac;
using FarseerGames.FarseerPhysics;
using FarseerGames.FarseerPhysics.Factories;
using Frenetic.Physics;
using FarseerGames.FarseerPhysics.Dynamics;
using FarseerGames.FarseerPhysics.Collisions;
using Microsoft.Xna.Framework;

namespace Frenetic.Autofac
{
    class PhysicsModule : Module
    {
        public Vector2 Gravity { get; set; }

        protected override void Load(ContainerBuilder builder)
        {
            builder.Register<PhysicsSettings>().SingletonScoped();
            PhysicsSimulator physicsSimulator = new PhysicsSimulator(Gravity);
            builder.Register<PhysicsSimulator>(physicsSimulator).SingletonScoped();
            // Body:
            builder.Register((c, p) => BodyFactory.Instance.CreateRectangleBody(c.Resolve<PhysicsSimulator>(), p.Named<float>("width"), p.Named<float>("height"), p.Named<float>("mass"))).FactoryScoped();
            // Geom:
            builder.Register((c, p) => GeomFactory.Instance.CreateRectangleGeom(c.Resolve<PhysicsSimulator>(), p.Named<Body>("body"), p.Named<float>("width"), p.Named<float>("height"))).FactoryScoped();
            // IPhysicsComponent:
            builder.Register((c, p) =>
            {
                var width = new NamedParameter("width", 50f);
                var height = new NamedParameter("height", 50f);
                var mass = new NamedParameter("mass", 100f);
                var bod = c.Resolve<Body>(width, height, mass);
                var body = new NamedParameter("body", bod);
                var geom = c.Resolve<Geom>(body, width, height, mass);
                return (IPhysicsComponent)new FarseerPhysicsComponent(bod, geom);
            }).FactoryScoped();
            builder.Register<FarseerPhysicsController>().SingletonScoped();
        }
    }
}
