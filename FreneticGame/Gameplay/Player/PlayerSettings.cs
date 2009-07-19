using System;
using Microsoft.Xna.Framework.Graphics;
using Frenetic.Graphics;

namespace Frenetic.Player
{
    public class NetworkPlayerSettings : IPlayerSettings
    {
        public NetworkPlayerSettings()
        {
            Name = "NetworkPlayer";
            Texture = PlayerTexture.Ball;

            Random rnd = new Random();
            Color = new Color((float)rnd.NextDouble(), (float)rnd.NextDouble(), (float)rnd.NextDouble());
        }

        public string Name { get; set; }
        public Color Color { get; set; }
        public PlayerTexture Texture { get; set; }
    }

    public class LocalPlayerSettings : IPlayerSettings
    {
        public LocalPlayerSettings()
        {
            Name = "Player";
            Texture = PlayerTexture.Ball;

            Color = Color.Black;
        }

        [Tweakable]
        public string Name { get; set; }
        [Tweakable]
        public Color Color { get; set; }

        public PlayerTexture Texture { get; set; }
    }
}
