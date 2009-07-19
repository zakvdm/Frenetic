using System;

namespace Frenetic.Level
{
    public class LevelController : IController
    {
        public LevelController(ILevel level)
        {
            _level = level;
        }
        #region IController Members

        public void Process(float elapsedTime)
        {
            if (!_level.Loaded)
                _level.Load();
        }

        #endregion

        ILevel _level;
    }
}
