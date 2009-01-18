using System;
using System.Collections.Generic;

namespace Frenetic.Level
{
    public class Level
    {
        public Level(ILevelLoader levelLoader)
        {
            Pieces = new List<LevelPiece>();

            _levelLoader = levelLoader;
            Loaded = false;
        }

        public void Load()
        {
            _levelLoader.LoadEmptyLevel(Pieces, 800, 600);
            Loaded = true;
        }
        
        public List<LevelPiece> Pieces { get; private set; }
        public bool Loaded { get; private set; }

        ILevelLoader _levelLoader;
    }
}
