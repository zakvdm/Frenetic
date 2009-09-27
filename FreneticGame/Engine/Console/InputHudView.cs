using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Frenetic.Graphics;
using Microsoft.Xna.Framework.Graphics;

namespace Frenetic.Engine.Overlay
{
    class InputOverlayView : IOverlayView
    {
        public InputOverlayView(InputLine inputLine, Rectangle inputWindow, IFont font)
        {
            _inputLine = inputLine;
            this.Window = inputWindow;
            _font = font;

            this.BackgroundColor = OverlaySetView.BACKGROUND_COLOR;
        }
        #region IHudView Members

        public bool Visible { get; set; }
        public Rectangle Window { get; set; }
        public Color BackgroundColor { get; set; }

        public void Draw(Frenetic.Graphics.ISpriteBatch spritebatch)
        {
            spritebatch.DrawText(_font, InputOverlayView.CursorText + _inputLine.CurrentInput, new Vector2(this.Window.Left + OverlaySetView.TEXT_OFFSET.X, this.Window.Bottom - OverlaySetView.TEXT_OFFSET.Y), Color.Yellow, 1);
        }

        #endregion

        InputLine _inputLine;
        IFont _font;
        
        internal const string CursorText = "Frenetic 0.2 > ";
    }
}
