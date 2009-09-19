using System;

namespace Frenetic.Gameplay.Level
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
