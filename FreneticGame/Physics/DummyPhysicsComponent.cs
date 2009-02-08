﻿using System;
using Microsoft.Xna.Framework;

namespace Frenetic.Physics
{
    public class DummyPhysicsComponent : IPhysicsComponent
    {
        public DummyPhysicsComponent()
        {
            Position = Microsoft.Xna.Framework.Vector2.Zero;
        }
        #region IPhysicsComponent Members

        public bool IsStatic { get; set; }
        public Vector2 Position { get; set; }
        public Vector2 LinearVelocity { get; private set; }
        public Vector2 Size { get; set; }

        public void ApplyImpulse(Vector2 impulse)
        {
        }

        public void ApplyForce(Vector2 force)
        {
        }

        public event CollidedWithWorldDelegate CollidedWithWorld = delegate { };

        #endregion
    }
}
