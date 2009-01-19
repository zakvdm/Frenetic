using System;

using Frenetic.Level;

using Autofac.Builder;
using Autofac;

using NUnit.Framework;
using Microsoft.Xna.Framework;
using FarseerGames.FarseerPhysics;
using FarseerGames.FarseerPhysics.Factories;
using FarseerGames.FarseerPhysics.Dynamics;
using FarseerGames.FarseerPhysics.Collisions;
using Frenetic.Physics;

namespace UnitTestLibrary
{
    [TestFixture]
    public class LevelPieceTests
    {
        [Test]
        public void CanMakeALevelPieceFactoryWithAutofac()
        {
            var builder = new ContainerBuilder();
            builder.Register<LevelPiece>().FactoryScoped();
            builder.RegisterGeneratedFactory<LevelPiece.Factory>(new TypedService(typeof(LevelPiece)));
            // PHYSICS:
            PhysicsSimulator physicsSimulator = new PhysicsSimulator();
            builder.Register<PhysicsSimulator>(physicsSimulator).SingletonScoped();
            // Body:
            builder.Register((c, p) => BodyFactory.Instance.CreateRectangleBody(c.Resolve<PhysicsSimulator>(), p.Named<float>("width"), p.Named<float>("height"), p.Named<float>("mass"))).FactoryScoped();
            // Geom:
            builder.Register((c, p) => GeomFactory.Instance.CreateRectangleGeom(p.Named<Body>("body"), p.Named<float>("width"), p.Named<float>("height"))).FactoryScoped();
            // IPhysicsComponent:
            builder.Register((c, p) =>
            {
                var width = new NamedParameter("width", 100f);
                var height = new NamedParameter("height", 200f);
                var mass = new NamedParameter("mass", 100f);
                var bod = c.Resolve<Body>(width, height, mass);
                var body = new NamedParameter("body", bod);
                var geom = c.Resolve<Geom>(body, width, height, mass);
                return (IPhysicsComponent)new FarseerPhysicsComponent(bod, geom);
            }).FactoryScoped();
            builder.Register<FarseerPhysicsController>().SingletonScoped();

            var container = builder.Build();
            var levelPieceFactory = container.Resolve<LevelPiece.Factory>();

            LevelPiece levelPiece1 = levelPieceFactory(new Vector2(100, 200), new Vector2(1, 2));
            LevelPiece levelPiece2 = levelPieceFactory(new Vector2(300, 400), new Vector2(3, 4));

            Assert.AreEqual(new Vector2(100, 200), levelPiece1.Position);
            Assert.AreEqual(new Vector2(1, 2), levelPiece1.Size);
            Assert.AreEqual(new Vector2(300, 400), levelPiece2.Position);
            Assert.AreEqual(new Vector2(3, 4), levelPiece2.Size);
        }
    }
}
