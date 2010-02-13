using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Frenetic.Gameplay.Weapons
{
    public class Slug
    {
        public Slug(Vector2 startPoint, Vector2 endPoint)
        {
            this.StartPoint = startPoint;
            this.EndPoint = endPoint;
        }

        public Vector2 StartPoint { get; private set; }
        public Vector2 EndPoint { get; private set; }

        public void Destroy()
        { }
    }
}
