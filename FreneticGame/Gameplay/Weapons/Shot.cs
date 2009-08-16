using System;
using Microsoft.Xna.Framework;

namespace Frenetic.Weapons
{
    public struct Shot
    {
        public Shot(Vector2 startPoint, Vector2 endPoint) : this()
        {
            StartPoint = startPoint;
            EndPoint = endPoint;
        }

        public Vector2 StartPoint { get; set; }
        public Vector2 EndPoint { get; set; }
    }
}
