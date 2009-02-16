using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;


namespace Frenetic.Graphics
{
    class XnaFont : IFont
    {
        public SpriteFont SpriteFont { get; private set; }

        public XnaFont(SpriteFont spriteFont)
        {
            SpriteFont = spriteFont;
        }

        public int LineSpacing
        {
            get
            {
                return SpriteFont.LineSpacing;
            }
            set
            {
                SpriteFont.LineSpacing = value;
            }
        }
    }
}
