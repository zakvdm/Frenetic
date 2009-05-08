using System;
using Microsoft.Xna.Framework.Graphics;
using Frenetic.Graphics;

namespace Frenetic.Player
{
    public interface IPlayerSettings
    {
        string Name { get; set; }
        Color Color { get; set; }
        PlayerTexture Texture { get; set; }
    }
}
