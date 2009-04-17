using System;
using FarseerGames.FarseerPhysics;
using Microsoft.Xna.Framework;

namespace Frenetic.Physics
{
    public class FarseerPhysicsSimulator : IPhysicsSimulator
    {
        public FarseerPhysicsSimulator(PhysicsSimulator physicsSimulator)
        {
            PhysicsSimulator = physicsSimulator;
        }

        public PhysicsSimulator PhysicsSimulator { get; private set; }

        public void Update(float elapsedTime)
        {
            PhysicsSimulator.Update(elapsedTime);
        }

        public float Gravity
        {
            get
            {
                return PhysicsSimulator.Gravity.Y;
            }
            set
            {
                PhysicsSimulator.Gravity = new Vector2(0f, value);
            }
        }
    }
}
