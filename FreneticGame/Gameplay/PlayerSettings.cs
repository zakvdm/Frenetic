using System;
using Microsoft.Xna.Framework.Graphics;
using Frenetic.Graphics;

namespace Frenetic
{
    public class PlayerSettings
    {
        public PlayerSettings()
        {
            Texture = PlayerTextures.Ball;

            Random rnd = new Random();
            Color = new Color((float)rnd.NextDouble(), (float)rnd.NextDouble(), (float)rnd.NextDouble());
        }
        public string Name { get; set; }
        public Color Color { get; set; }
        public PlayerTextures Texture { get; set; }
    }
}
