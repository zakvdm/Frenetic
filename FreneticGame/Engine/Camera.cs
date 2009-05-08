using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Frenetic.Player;

namespace Frenetic
{
    public class Camera : ICamera
    {
        public Camera(IPlayer player, Vector2 screenSize)
        {
            _player = player;
            _screenSize = screenSize;
            _screenSizeOffset = screenSize / 2;
        }

        public float ScreenWidth
        {
            get
            {
                return _screenSize.X;
            }
        }
        public float ScreenHeight
        {
            get
            {
                return _screenSize.Y;
            }
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
        Vector2 _screenSize;
        Vector2 _screenSizeOffset;
    }
}