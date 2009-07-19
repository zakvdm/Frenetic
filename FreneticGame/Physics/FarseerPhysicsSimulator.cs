using System;
using FarseerGames.FarseerPhysics;
using Microsoft.Xna.Framework;
using FarseerGames.FarseerPhysics.Collisions;

namespace Frenetic.Physics
{
    public class FarseerPhysicsSimulator : IPhysicsSimulator
    {
        public FarseerPhysicsSimulator(PhysicsSimulator physicsSimulator)
        {
            PhysicsSimulator = physicsSimulator;
            PhysicsSimulator.NarrowPhaseCollider = NarrowPhaseCollider.SAT;
            PhysicsSimulator.BiasFactor = 0.5f;
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
