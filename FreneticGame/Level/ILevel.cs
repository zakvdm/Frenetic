using System;
using System.Collections.Generic;

namespace Frenetic.Level
{
    public interface ILevel
    {
        void Load();
        bool Loaded { get; }
        List<LevelPiece> Pieces { get; }
    }
}
