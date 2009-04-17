using System;
using Autofac.Builder;
using Autofac;
using FarseerGames.FarseerPhysics;
using FarseerGames.FarseerPhysics.Factories;
using Frenetic.Physics;
using FarseerGames.FarseerPhysics.Dynamics;
using FarseerGames.FarseerPhysics.Collisions;
using Microsoft.Xna.Framework;
using System.Linq;

namespace Frenetic.Autofac
{
    class PhysicsModule : Module
    {
        public Vector2 Gravity { get; set; }

        protected override void Load(ContainerBuilder builder)
        {
            _clientPhysicsSimulator = new FarseerPhysicsSimulator(new PhysicsSimulator(Gravity));
            builder.Register(
                    (c, p) =>
                    {
                        if (p.ToList<Parameter>().Count == 0)
                            return _clientPhysicsSimulator;
                        else
                            return new FarseerPhysicsSimulator(new PhysicsSimulator(Gravity));
                    }).As<IPhysicsSimulator>().ContainerScoped();

            builder.Register<PhysicsSettings>().SingletonScoped();
            // Body:
            builder.Register((c, p) => BodyFactory.Instance.CreateRectangleBody(c.Resolve<IPhysicsSimulator>().PhysicsSimulator, p.Named<float>("width"), p.Named<float>("height"), p.Named<float>("mass"))).FactoryScoped();
            // Geom:
            builder.Register((c, p) => GeomFactory.Instance.CreateRectangleGeom(c.Resolve<IPhysicsSimulator>().PhysicsSimulator, p.Named<Body>("body"), p.Named<float>("width"), p.Named<float>("height"))).FactoryScoped();
            // IPhysicsComponent:
            builder.Register((c, p) =>
            {
                var width = new NamedParameter("width", 50f);
                var height = new NamedParameter("height", 50f);
                var mass = new NamedParameter("mass", 100f);
                var bod = c.Resolve<Body>(width, height, mass);
                var body = new NamedParameter("body", bod);
                var geom = c.Resolve<Geom>(body, width, height, mass);
                return new FarseerPhysicsComponent(bod, geom);
            }).As<IPhysicsComponent>().FactoryScoped();
            builder.Register<PhysicsController>().ContainerScoped();
        }

        IPhysicsSimulator _clientPhysicsSimulator;
    }
}
