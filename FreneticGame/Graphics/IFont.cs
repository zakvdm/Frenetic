using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Frenetic.Graphics
{
    public interface IFont
    {
        int LineSpacing { get; set; }
        SpriteFont SpriteFont { get; }
    }
}
