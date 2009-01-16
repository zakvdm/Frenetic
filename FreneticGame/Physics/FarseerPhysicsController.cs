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

        public void Process()
        {
            _physicsSimulator.Update(.001f);    // TODO: Make time based
        }

        #endregion

        PhysicsSimulator _physicsSimulator;
    }
}