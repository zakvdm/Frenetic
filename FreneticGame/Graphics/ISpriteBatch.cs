using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Frenetic.Graphics
{
    public interface ISpriteBatch
    {
        void Begin();
        void Begin(Matrix translationMatrix);
        void End();
        void Draw(ITexture texture, Vector2 position, Rectangle? sourceRectangle, Color color, 
            float rotation, Vector2 origin, Vector2 scale, SpriteEffects effects, float layerDepth);
    }
}
