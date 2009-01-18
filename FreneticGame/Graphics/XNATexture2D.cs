using System;
using Microsoft.Xna.Framework.Graphics;

namespace Frenetic.Graphics
{
    public class XNATexture : ITexture
    {
        public Texture2D Texture2D { get; private set; }

        public XNATexture(Texture2D texture)
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
