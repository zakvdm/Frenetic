using System;
using Frenetic.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Frenetic
{
    public class CrosshairView : IView
    {
        public CrosshairView(ICrosshair crosshair, ISpriteBatch spriteBatch, ITexture texture)
        {
            _crosshair = crosshair;
            _spriteBatch = spriteBatch;
            _texture = texture;
        }
        #region IView Members

        public void Generate()
        {
            _spriteBatch.Begin();
            Rectangle destinationRectangle = new Rectangle(
                                                    (int)(_crosshair.ViewPosition.X - (_crosshair.Size / 2)), 
                                                    (int)(_crosshair.ViewPosition.Y - (_crosshair.Size / 2)),
                                                    _crosshair.Size, _crosshair.Size);
            _spriteBatch.Draw(_texture, destinationRectangle, Color.White, 1);
            _spriteBatch.End();
        }

        #endregion

        ICrosshair _crosshair;
        ISpriteBatch _spriteBatch;
        ITexture _texture;
    }
}
