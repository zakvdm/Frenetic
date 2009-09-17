using System;
using Microsoft.Xna.Framework;
using Frenetic.Player;

namespace Frenetic.Physics
{
    public class DummyPhysicsComponent : IPhysicsComponent
    {
        public DummyPhysicsComponent()
        {
            Position = Microsoft.Xna.Framework.Vector2.Zero;
        }
        #region IPhysicsComponent Members

        public bool Enabled { get; set; }
        public bool IsStatic { get; set; }
        public Vector2 Position { get; set; }
        public Vector2 LinearVelocity { get; set; }
        public Vector2 Size { get; set; }

        public int CollisionGroup { get; set; }

        public void ApplyImpulse(Vector2 impulse)
        {
        }

        public void ApplyForce(Vector2 force)
        {
        }

        public void OnWasShot(IPlayer shootingPlayer) { }

        public event Action CollidedWithWorld = delegate { };
        public event Action<IPlayer> WasShot = delegate { };

        #endregion
    }
}
