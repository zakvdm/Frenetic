using System;
using Microsoft.Xna.Framework;

namespace Frenetic.Physics
{
    public interface IBoundaryCollider
    {
        Vector2 MoveWithinBoundary(Vector2 position);
    }
}
