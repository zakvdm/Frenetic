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
using System.Collections.Generic;

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
                            _serverPhysicsSimulator = new FarseerPhysicsSimulator(new PhysicsSimulator(Gravity));
                            return _serverPhysicsSimulator;
                    }).As<IPhysicsSimulator>().ContainerScoped();

            builder.Register<PhysicsSettings>().SingletonScoped();

            //IPhysicsComponent
            builder.Register((c, p) => { return CreatePhysicsComponent<FarseerPhysicsComponent>(c, p, false); }).As<IPhysicsComponent>().FactoryScoped();
            builder.Register((c, p) => { return CreatePhysicsComponent<LocalPlayerFarseerPhysicsComponent>(c, p, true); }).As<LocalPlayerFarseerPhysicsComponent>().ContainerScoped();
            builder.RegisterGeneratedFactory<FarseerPhysicsComponent.Factory>(new TypedService(typeof(IPhysicsComponent))).ContainerScoped();

            builder.Register<PhysicsController>().ContainerScoped();

            builder.Register<FarseerRayCaster>().As<IRayCaster>().ContainerScoped();
        }

        PhysicsComponentType CreatePhysicsComponent<PhysicsComponentType>(IContext container, IEnumerable<Parameter> parameters, bool localPlayer) where PhysicsComponentType : IPhysicsComponent
        {
            Vector2 size;
            object tmp;
            if (parameters.Count() > 0)
            {
                size = parameters.Named<Vector2>("size");
            }
            else
            {
                size = new Vector2(20f, 30f);
            }

            var simulator = container.Resolve<IPhysicsSimulator>().PhysicsSimulator;

            var vertices = Vertices.CreateSimpleRectangle(size.X, size.Y);
            var body = BodyFactory.Instance.CreatePolygonBody(simulator, vertices, 1000f);
            var geom = GeomFactory.Instance.CreateSATPolygonGeom(simulator, body, vertices, 1)[0];

            body.MomentOfInertia = float.MaxValue;

            simulator.ProcessAddedAndRemoved();

            if (typeof(PhysicsComponentType) == typeof(FarseerPhysicsComponent))
            {
                return (PhysicsComponentType)(IPhysicsComponent)new FarseerPhysicsComponent(body, geom);
            }
            return (PhysicsComponentType)(IPhysicsComponent)new LocalPlayerFarseerPhysicsComponent(body, geom);
        }

        IPhysicsSimulator _clientPhysicsSimulator;
        IPhysicsSimulator _serverPhysicsSimulator;
    }
}
