using System;
using Microsoft.Xna.Framework;

namespace Frenetic.Physics
{
    public class WorldBoundaryCollider : IBoundaryCollider
    {
        public WorldBoundaryCollider(PhysicsValues physicsValues)
        {
            _physicsValues = physicsValues;
        }

        public Vector2 MoveWithinBoundary(Vector2 position)
        {
            if (position.X > _physicsValues.Width)
                position.X = _physicsValues.Width;

            if (position.Y > _physicsValues.Height)
                position.Y = _physicsValues.Height;

            if (position.X < 0)
                position.X = 0;

            if (position.Y < 0)
                position.Y = 0;

            return position;
        }

        PhysicsValues _physicsValues;
    }
}
