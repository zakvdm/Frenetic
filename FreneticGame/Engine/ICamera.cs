using System;
using Microsoft.Xna.Framework;
namespace Frenetic
{
    public interface ICamera
    {
        Vector2 ConvertToWorldCoordinates(Microsoft.Xna.Framework.Vector2 screenPosition);
        Vector2 Position { get; }
        Matrix TranslationMatrix { get; }
    }
}
