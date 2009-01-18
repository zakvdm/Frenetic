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

        Body _body;
        Geom _geom;
    }
}
