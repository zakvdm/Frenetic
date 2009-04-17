using System;
using FarseerGames.FarseerPhysics;

namespace Frenetic.Physics
{
    public interface IPhysicsSimulator
    {
        PhysicsSimulator PhysicsSimulator { get; }

        void Update(float elapsedTime);
        float Gravity { get; set; }
    }
}
