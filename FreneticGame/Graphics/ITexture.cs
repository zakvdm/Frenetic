using System;
using Microsoft.Xna.Framework.Graphics;

namespace Frenetic.Graphics
{
    public interface ITexture
    {
        Texture2D Texture2D { get; }

        int Width { get; }
        int Height { get; }
    }
}
