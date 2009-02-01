using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Frenetic
{
    public class Camera : ICamera
    {
        public Camera(IPlayer player, Vector2 screenSize)
        {
            _player = player;
            _screenSizeOffset = screenSize / 2;
        }

        public Vector2 ConvertToWorldCoordinates(Vector2 screenPosition)
        {
            return Vector2.Transform(screenPosition, Matrix.Invert(TranslationMatrix));
        }

        public Vector2 Position
        {
            get
            {
                return _player.Position;
            }
        }

        public Matrix TranslationMatrix
        {
            get
            {
                return Matrix.CreateTranslation(-Position.X + _screenSizeOffset.X, -Position.Y + _screenSizeOffset.Y, 0.0f);
            }
        }

        IPlayer _player;
        Vector2 _screenSizeOffset;
    }
}