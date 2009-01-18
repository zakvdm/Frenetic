using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Frenetic.Level
{
    public class DumbLevelLoader : ILevelLoader
    {
        private const int BOUNDARY = 20;
        public DumbLevelLoader(LevelPiece.Factory levelPieceFactory)
        {
            _levelPieceFactory = levelPieceFactory;
        }

        public void LoadEmptyLevel(List<LevelPiece> levelPieces, int width, int height)
        {
            levelPieces.Add(_levelPieceFactory(new Vector2(BOUNDARY/2, height/2), new Vector2(BOUNDARY, height)));
            levelPieces.Add(_levelPieceFactory(new Vector2(width/2, BOUNDARY/2), new Vector2(width, BOUNDARY)));
            levelPieces.Add(_levelPieceFactory(new Vector2(width - (BOUNDARY/2), height/2), new Vector2(BOUNDARY, height)));
            levelPieces.Add(_levelPieceFactory(new Vector2(width/2, height - (BOUNDARY/2)), new Vector2(width, BOUNDARY)));
        }

        LevelPiece.Factory _levelPieceFactory;
    }
}
