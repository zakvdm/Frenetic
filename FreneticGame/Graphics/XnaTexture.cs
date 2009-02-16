using System;
using Microsoft.Xna.Framework.Graphics;

namespace Frenetic.Graphics
{
    public class XnaTexture : ITexture
    {
        public Texture2D Texture2D { get; private set; }

        public XnaTexture(Texture2D texture)
        {
            Texture2D = texture;
        }

        public int Width
        {
            get
            {
                return Texture2D.Width;
            }
        }
        public int Height
        {
            get
            {
                return Texture2D.Height;
            }
        }
    }
}
