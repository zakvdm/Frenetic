using System;
using Microsoft.Xna.Framework;

namespace Frenetic.Physics
{
    public interface IRayCaster
    {
        Vector2 ShootRay(Vector2 origin, Vector2 direction);
    }
}
