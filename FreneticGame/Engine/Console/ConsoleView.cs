using System;
using Frenetic.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;


namespace Frenetic.Engine.Overlay
{
    public class OverlaySetView : IView
    {
        public static Color BACKGROUND_COLOR = new Color(Color.DarkGray, 0.8f);
        public static Vector2 TEXT_OFFSET = new Vector2(20f, 20f);
        public static int EDGE_GAP = 10;


        public OverlaySetView(ISpriteBatch spriteBatch, ITexture texture, IFont font)
        {
            _spriteBatch = spriteBatch;
            _texture = texture;
            _font = font;
        }

        #region IView Members

        public void Generate(float elapsedSeconds)
        {
            foreach (IOverlayView overlay in _overlays)
            {
                if (overlay.Visible)
                {
                    _spriteBatch.Begin();

                    DrawWindow(overlay.Window, overlay.BackgroundColor);
                    overlay.Draw(_spriteBatch);

                    _spriteBatch.End();
                }
            }
        }

        #endregion

        private void DrawWindow(Rectangle window, Color color)
        {
            _spriteBatch.Draw(_texture, window, color, 0f);
        }

        protected List<IOverlayView> _overlays = new List<IOverlayView>();
        ISpriteBatch _spriteBatch;
        ITexture _texture;
        IFont _font;
    }
}
