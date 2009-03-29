using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Frenetic.Level
{
    public class DumbLevelLoader : ILevelLoader
    {
        private const int BOUNDARY = 50;
        public DumbLevelLoader(LevelPiece.Factory levelPieceFactory)
        {
            _levelPieceFactory = levelPieceFactory;
        }

        public void LoadEmptyLevel(List<LevelPiece> levelPieces, int width, int height)
        {
            float halfwidth = BOUNDARY / 2;
            Vector2 middle = new Vector2(width / 2, height / 2);
            levelPieces.Add(_levelPieceFactory(new Vector2(-halfwidth, middle.Y), new Vector2(BOUNDARY, height)));
            levelPieces.Add(_levelPieceFactory(new Vector2(middle.X, -halfwidth), new Vector2(width, BOUNDARY)));
            levelPieces.Add(_levelPieceFactory(new Vector2(width + halfwidth, middle.Y), new Vector2(BOUNDARY, height)));
            levelPieces.Add(_levelPieceFactory(new Vector2(middle.X, height + halfwidth), new Vector2(width, BOUNDARY)));

            // BOUNDARY:
            /*
            levelPieces.Add(_levelPieceFactory(new Vector2(BOUNDARY/2, height/2), new Vector2(BOUNDARY, height)));              // left
            levelPieces.Add(_levelPieceFactory(new Vector2(width/2, BOUNDARY/2), new Vector2(width, BOUNDARY)));                // top
            levelPieces.Add(_levelPieceFactory(new Vector2(width - (BOUNDARY/2), height/2), new Vector2(BOUNDARY, height)));    // right
            levelPieces.Add(_levelPieceFactory(new Vector2(width/2, height - (BOUNDARY/2)), new Vector2(width, BOUNDARY)));     // bottom
            */

            // PIECES:
            levelPieces.Add(_levelPieceFactory(new Vector2(200, 400), new Vector2(150, 50)));
            levelPieces.Add(_levelPieceFactory(new Vector2(700, 300), new Vector2(50, 350)));
            levelPieces.Add(_levelPieceFactory(new Vector2(400, 200), new Vector2(300, 30)));
        }

        LevelPiece.Factory _levelPieceFactory;
    }
}
