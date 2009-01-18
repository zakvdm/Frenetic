using System;
using System.Collections.Generic;
namespace Frenetic.Level
{
    public interface ILevelLoader
    {
        void LoadEmptyLevel(List<LevelPiece> levelPieces, int width, int height);
    }
}
