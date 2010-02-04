using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Frenetic.Physics;

namespace Frenetic.Weapons
{
    public class Rocket
    {
        private static Vector2 Size = new Vector2(5, 5);
        public const float Speed = 450f;

        public delegate Rocket Factory(Vector2 position, Vector2 direction);

        public Rocket(Vector2 position, Vector2 direction, IPhysicsComponent physicsComponent)
        {
            this.PhysicsComponent = physicsComponent;

            this.IsAlive = true;

            this.PhysicsComponent.Size = Rocket.Size;

            this.PhysicsComponent.Position = position;
            this.PhysicsComponent.LinearVelocity = direction * Rocket.Speed;

            this.PhysicsComponent.CollidedWithWorld += () => this.IsAlive = false;
        }

        public bool IsAlive { get; set; }
        public Vector2 Position { get { return this.PhysicsComponent.Position; } }
        public Vector2 Velocity { get { return this.PhysicsComponent.LinearVelocity; } }

        public void Destroy()
        {
            this.PhysicsComponent.Enabled = false;
        }

        IPhysicsComponent PhysicsComponent;
    }
}
