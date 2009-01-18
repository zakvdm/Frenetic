using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Frenetic.Level
{
    public class LevelPiece
    {
        public delegate LevelPiece Factory(Vector2 position, Vector2 size);

        public LevelPiece(Vector2 position, Vector2 size)
        {
            Position = position;
            Size = size;
            
            Random rnd = new Random();
            Color = new Color((float)rnd.NextDouble(), (float)rnd.NextDouble(), (float)rnd.NextDouble());
        }

        public Vector2 Position { get; private set; }
        public Vector2 Size { get; private set; }
        public Color Color { get; private set; }
    }
}
