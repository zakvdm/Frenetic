using System;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Frenetic.Physics
{
    public interface IRayCaster
    {
        /// <summary>
        /// Projects the Ray out.
        /// </summary>
        /// <param name="origin">Start of Ray</param>
        /// <param name="direction">Direction of Ray (does NOT have to be a unit vector)</param>
        /// <param name="foundEndPoint">Will contain the end point of the ray</param>
        /// <returns>A List of all the hit IPhysicsComponents</returns>
        List<IPhysicsComponent> ShootRay(Vector2 origin, Vector2 direction, out Vector2 foundEndPoint);
    }
}
