using System;

using Frenetic;
using FarseerGames.FarseerPhysics;

namespace Frenetic.Physics
{
    public class FarseerPhysicsController : IController
    {
        public FarseerPhysicsController(PhysicsSimulator physicsSimulator)
        {
            _physicsSimulator = physicsSimulator;
        }

        #region IController Members

        public void Process(float ticks)
        {
            _physicsSimulator.Update(ticks);
        }

        #endregion

        PhysicsSimulator _physicsSimulator;
    }
}