using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;



namespace Frenetic.Graphics
{
    class XNASpriteBatch : ISpriteBatch
    {
        public XNASpriteBatch(SpriteBatch spriteBatch)
        {
            _spriteBatch = spriteBatch;
        }

        public void Begin()
        {
            _spriteBatch.Begin();
        }
        public void Begin(Matrix translationMatrix)
        {
            _spriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Deferred, SaveStateMode.None, translationMatrix);
        }

        public void End()
        {
            _spriteBatch.End();
        }

        public void Draw(ITexture texture, Vector2 position, Nullable<Rectangle> sourceRectangle, Color color,
            float rotation, Vector2 origin, Vector2 scale, SpriteEffects effects, float layerDepth)
        {
            _spriteBatch.Draw(texture.Texture2D, position, sourceRectangle, color, rotation, origin, scale, effects, layerDepth);
        }

        SpriteBatch _spriteBatch;
    }
}
