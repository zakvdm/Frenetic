using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Frenetic.Graphics
{
    public interface IBubbleTextDrawer
    {
        void AddText(string text, Vector2 position, Color color, float fadeOutTime);
        void DrawText(ISpriteBatch spriteBatch, float elapsedSeconds);
    }
}
