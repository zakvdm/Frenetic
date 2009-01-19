using System;

namespace Frenetic.Level
{
    public class LevelController : IController
    {
        public LevelController(Level level)
        {
            _level = level;
        }
        #region IController Members

        public void Process(long ticks)
        {
            if (!_level.Loaded)
                _level.Load();
        }

        #endregion

        Level _level;
    }
}
