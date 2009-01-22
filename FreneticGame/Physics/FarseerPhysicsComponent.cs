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
                _body.Position = value;
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
                var geom = FarseerGames.FarseerPhysics.Factories.GeomFactory.Instance.CreateRectangleGeom(_body, value.X, value.Y);
                _geom.SetVertices(geom.LocalVertices);
                _body.Position = _body.Position;
                _geom.ComputeCollisionGrid();
            }
        }
        

        Body _body;
        Geom _geom;
    }
}
