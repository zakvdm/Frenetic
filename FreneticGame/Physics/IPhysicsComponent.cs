using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Frenetic.Physics
{
    public interface IPhysicsComponent
    {
        bool IsStatic { get; set; }
        Vector2 Position { get; set; }
        Vector2 LinearVelocity { get; }
        Vector2 Size { get; set; }

        void ApplyImpulse(Vector2 impulse);
        void ApplyForce(Vector2 force);
    }
}
