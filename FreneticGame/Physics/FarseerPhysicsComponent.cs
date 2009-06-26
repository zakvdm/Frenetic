using System;
using FarseerGames.FarseerPhysics.Dynamics;
using FarseerGames.FarseerPhysics.Collisions;
using Microsoft.Xna.Framework;

namespace Frenetic.Physics
{
    public class FarseerPhysicsComponent : IPhysicsComponent
    {
        public delegate IPhysicsComponent Factory(Vector2 size);

        public FarseerPhysicsComponent(Body body, Geom geom)
        {
            _body = body;
            _geom = geom;

            _geom.FrictionCoefficient = 1.1f;

            _geom.OnCollision += new CollisionEventHandler((geom1, geom2, contactList) =>
                {
                    CollidedWithWorld();
                    return true;
                }); 
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
                return _body.Position;
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
                throw new NotImplementedException();
                //_geom.SetVertices(Vertices.CreateSimpleRectangle(value.X, value.Y));
                //_geom.SetBody(_body);
            }
        }
        public Vector2 LinearVelocity
        {
            get
            {
                return new Vector2(_body.LinearVelocity.X, _body.LinearVelocity.Y);
            }
        }

        public void ApplyImpulse(Vector2 impulse)
        {
            _body.ApplyImpulse(impulse);
        }

        public void ApplyForce(Vector2 force)
        {
            _body.ApplyForce(force);
        }

        public event CollidedWithWorldDelegate CollidedWithWorld = delegate { };
        

        Body _body;
        Geom _geom;
    }
}
