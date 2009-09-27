using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Frenetic.Graphics;

namespace Frenetic.Gameplay.Level
{
    public class LevelView : IView
    {
        public LevelView(ILevel level, ISpriteBatch spriteBatch, ITexture texture, ICamera camera)
        {
            _level = level;
            _spriteBatch = spriteBatch;
            _texture = texture;
            _camera = camera;
        }

        #region IView Members

        public void Generate(float elapsedSeconds)
        {
            _spriteBatch.Begin(_camera.TranslationMatrix);
            foreach (LevelPiece piece in _level.Pieces)
            {
                _spriteBatch.Draw(_texture, new Vector2(piece.Position.X - (piece.Size.X / 2), piece.Position.Y - (piece.Size.Y / 2)),
                    null, piece.Color, 0f, new Vector2(0, 0), piece.Size, SpriteEffects.None, 0f);
            }
            _spriteBatch.End();
        }

        #endregion

        ILevel _level;
        ISpriteBatch _spriteBatch;
        ITexture _texture;
        ICamera _camera;
    }
}
