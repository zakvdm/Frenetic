using System;
using Microsoft.Xna.Framework;

namespace Frenetic.Physics
{
    public class WorldBoundaryCollider : IBoundaryCollider
    {
        public WorldBoundaryCollider(int width, int height)
        {
            _width = width;
            _height = height;
        }

        public Vector2 MoveWithinBoundary(Vector2 position)
        {
            if (position.X > _width)
                position.X = _width;

            if (position.Y > _height)
                position.Y = _height;

            if (position.X < 0)
                position.X = 0;

            if (position.Y < 0)
                position.Y = 0;

            return position;
        }

        int _width;
        int _height;
    }
}
