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

        public void Process(long ticks)
        {
            _physicsSimulator.Update(ticks * 0.000001f);    // TODO: Make time based
        }

        #endregion

        PhysicsSimulator _physicsSimulator;
    }
}