using System;

namespace Frenetic.Graphics
{
    public interface ITextureBank<textureEnum>
    {
        ITexture this[textureEnum texture] { get; }
    }
}
