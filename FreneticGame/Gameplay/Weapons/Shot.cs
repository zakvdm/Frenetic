using System;
using Microsoft.Xna.Framework;

namespace Frenetic.Weapons
{
    public class Shot
    {
        public Shot(Vector2 startPoint, Vector2 endPoint)
        {
            this.StartPoint = startPoint;
            this.EndPoint = endPoint;
        }

        public Vector2 StartPoint { get; private set; }
        public Vector2 EndPoint { get; private set; }
    }
}
