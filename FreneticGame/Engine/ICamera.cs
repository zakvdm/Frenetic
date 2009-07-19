using System;
using Microsoft.Xna.Framework;
namespace Frenetic
{
    public interface ICamera
    {
        float ScreenWidth { get; }
        float ScreenHeight { get; }

        Vector2 ConvertToWorldCoordinates(Vector2 screenPosition);
        Vector2 Position { get; }

        /// <summary>
        /// The Matrix that converts world coordinates into screen coordinates
        /// </summary>
        Matrix TranslationMatrix { get; }
    }
}
