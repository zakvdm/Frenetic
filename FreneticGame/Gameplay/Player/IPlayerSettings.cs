using System;
using Microsoft.Xna.Framework.Graphics;
using Frenetic.Graphics;
using Frenetic.Engine;

namespace Frenetic.Player
{
    public interface IPlayerSettings : IDiffable<IPlayerSettings>
    {
        string Name { get; set; }
        Color Color { get; set; }
        PlayerTexture Texture { get; set; }
    }
}
