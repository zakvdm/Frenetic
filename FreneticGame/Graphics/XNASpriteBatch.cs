using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Frenetic.Graphics
{
    class XnaSpriteBatch : ISpriteBatch
    {
        public XnaSpriteBatch(SpriteBatch spriteBatch)
        {
            _spriteBatch = spriteBatch;
        }

        public void Begin()
        {
            _spriteBatch.Begin();
        }
        public void Begin(Matrix translationMatrix)
        {
            _spriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Immediate, SaveStateMode.None, translationMatrix);
        }

        public void End()
        {
            _spriteBatch.End();
        }

        public void Draw(ITexture texture, Rectangle destinationRectangle, Color color, float layerDepth)
        {
            _spriteBatch.Draw(texture.Texture2D, destinationRectangle, null, color, 0f, new Vector2(0, 0), SpriteEffects.None, layerDepth);
        }
        public void Draw(ITexture texture, Vector2 position, Nullable<Rectangle> sourceRectangle, Color color,
            float rotation, Vector2 origin, Vector2 scale, SpriteEffects effects, float layerDepth)
        {
            _spriteBatch.Draw(texture.Texture2D, position, sourceRectangle, color, rotation, origin, scale, effects, layerDepth);
        }

        public void DrawText(IFont font, string text, Vector2 position, Color color)
        {
            _spriteBatch.DrawString(font.SpriteFont, text, position, color);
        }

        SpriteBatch _spriteBatch;
    }
}
