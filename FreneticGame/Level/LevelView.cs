﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Frenetic.Graphics;

namespace Frenetic.Level
{
    public class LevelView : IView
    {
        public LevelView(Level level, ISpriteBatch spriteBatch, ITexture texture)
        {
            _level = level;
            _spriteBatch = spriteBatch;
            _texture = texture;
        }

        #region IView Members

        public void Generate()
        {
            _spriteBatch.Begin();
            foreach (LevelPiece piece in _level.Pieces)
            {
                _spriteBatch.Draw(_texture, piece.Position, null, piece.Color, 0f,
                    new Vector2(piece.Size.X / 2f, piece.Size.Y / 2f),
                    //piece.Size,
                    new Vector2(1, 1),
                    SpriteEffects.None, 10f);
            }
            _spriteBatch.End();
        }

        #endregion

        Level _level;
        ISpriteBatch _spriteBatch;
        ITexture _texture;
    }
}
