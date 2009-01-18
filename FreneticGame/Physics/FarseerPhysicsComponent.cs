using System;
using FarseerGames.FarseerPhysics.Dynamics;
using FarseerGames.FarseerPhysics.Collisions;
using Microsoft.Xna.Framework;

namespace Frenetic.Physics
{
    public class FarseerPhysicsComponent : IPhysicsComponent
    {
        public FarseerPhysicsComponent(Body body, Geom geom)
        {
            _body = body;
            _geom = geom;
        }

        public bool IsStatic
        {
            get
            {
                return _body.IsStatic;
            }
            set
            {
                _body.IsStatic = value;
            }
        }
        public Vector2 Position
        {
            get
            {
                return new Vector2(_body.Position.X, _body.Position.Y);
            }
            set
            {
                _body.Position = new FarseerGames.FarseerPhysics.Mathematics.Vector2(value.X, value.Y);
                //_body.LinearVelocity = new FarseerGames.FarseerPhysics.Mathematics.Vector2(0, 0);
            }
        }
        public Vector2 Size
        {
            get
            {
                return new Vector2(_geom.AABB.Width, _geom.AABB.Height);
            }
            set
            {
                /*
                Vertices vertices = new Vertices();
                vertices.Add(new FarseerGames.FarseerPhysics.Mathematics.Vector2(-value.X / 2f, -value.Y / 2f));
                vertices.Add(new FarseerGames.FarseerPhysics.Mathematics.Vector2(-value.X / 2f, value.Y / 2f));
                vertices.Add(new FarseerGames.FarseerPhysics.Mathematics.Vector2(value.X / 2f, value.Y / 2f));
                vertices.Add(new FarseerGames.FarseerPhysics.Mathematics.Vector2(value.X / 2f, -value.Y / 2f));

                _geom.SetVertices(vertices);
                */
                var geom = FarseerGames.FarseerPhysics.Factories.GeomFactory.Instance.CreateRectangleGeom(_body, value.X, value.Y);
                _geom.SetVertices(geom.LocalVertices);
            }
        }
        

        Body _body;
        Geom _geom;
    }
}
