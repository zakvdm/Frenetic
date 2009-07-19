using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Frenetic.Graphics;
using Microsoft.Xna.Framework.Graphics;

namespace Frenetic.Engine.Overlay
{
    public interface IOverlayView
    {
        bool Visible { get; set; }

        Rectangle Window { get; set; }
        void Draw(ISpriteBatch spritebatch);

        Color BackgroundColor { get; set; }
    }
}
