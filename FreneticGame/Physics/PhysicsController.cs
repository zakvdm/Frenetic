using System;

using Frenetic;
using FarseerGames.FarseerPhysics;

namespace Frenetic.Physics
{
    public class PhysicsController : IController
    {
        public PhysicsController(IPhysicsSimulator physicsSimulator)
        {
            _physicsSimulator = physicsSimulator;
        }

        #region IController Members

        public void Process(float elapsedTime)
        {
            _physicsSimulator.Update(elapsedTime);
        }

        #endregion

        IPhysicsSimulator _physicsSimulator;
    }
}