using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Frenetic.Graphics;

namespace Frenetic.Engine.Overlay
{
    public class ConsoleOverlaySetView : OverlaySetView
    {
        public ConsoleOverlaySetView(IOverlayView inputView, IOverlayView commandConsoleView, IOverlayView messageConsoleView, IOverlayView possibleCommandsView, ISpriteBatch spriteBatch, ITexture texture, IFont font)
            : base(spriteBatch, texture, font)
        {
            _overlays.Add(inputView);
            _overlays.Add(commandConsoleView);
            _overlays.Add(messageConsoleView);
            _overlays.Add(possibleCommandsView);
        }
    }
}
