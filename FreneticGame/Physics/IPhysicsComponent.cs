using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Frenetic.Player;

namespace Frenetic.Physics
{
    public interface IPhysicsComponent
    {
        bool Enabled { get; set; }
        bool IsStatic { get; set; }
        Vector2 Position { get; set; }
        Vector2 LinearVelocity { get; set; }
        Vector2 Size { get; set; }

        int CollisionGroup { get; set; }

        void ApplyImpulse(Vector2 impulse);
        void ApplyForce(Vector2 force);

        void OnWasShot(IPlayer shootingPlayer);

        event Action CollidedWithWorld;
        event Action<IPlayer> WasShot;
    }
}
