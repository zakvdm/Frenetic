using System;
using FarseerGames.FarseerPhysics;
using Microsoft.Xna.Framework;

namespace Frenetic.Physics
{
    public class FarseerPhysicsSimulator : IPhysicsSimulator
    {
        public FarseerPhysicsSimulator(PhysicsSimulator physicsSimulator)
        {
            _physicsSimulator = physicsSimulator;
        }

        public float Gravity
        {
            get
            {
                return _physicsSimulator.Gravity.Y;
            }
            set
            {
                _physicsSimulator.Gravity = new Vector2(0f, value);
            }
        }

        PhysicsSimulator _physicsSimulator;
    }
}
