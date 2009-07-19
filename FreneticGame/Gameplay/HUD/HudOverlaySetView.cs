using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Frenetic.Engine.Overlay;
using Frenetic.Graphics;

namespace Frenetic.Gameplay.HUD
{
    public class HudOverlaySetView : OverlaySetView
    {
        public HudOverlaySetView(IOverlayView scoreView, ISpriteBatch spriteBatch, ITexture texture, IFont font)
            :base(spriteBatch, texture, font)
        {
            _overlays.Add(scoreView);
        }
    }
}
