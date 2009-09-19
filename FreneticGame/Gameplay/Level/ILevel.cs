using System;
using System.Collections.Generic;

namespace Frenetic.Gameplay.Level
{
    public interface ILevel
    {
        void Load();
        bool Loaded { get; }
        List<LevelPiece> Pieces { get; }
    }
}
