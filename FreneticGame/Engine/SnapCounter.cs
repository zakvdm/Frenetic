using System;

namespace Frenetic
{
    public class SnapCounter : IController, ISnapCounter
    {
        public SnapCounter()
        {
            CurrentSnap = 1;    // Start from 1
        }

        public int SnapsPerSecond = 25;

        #region IController Members

        public void Process(float elapsedSeconds)
        {
            _totalElapsedSeconds += elapsedSeconds;

            float secondsPerSnap = 1f / SnapsPerSecond;

            if (_totalElapsedSeconds  > secondsPerSnap)
            {
                _totalElapsedSeconds -= secondsPerSnap;
                CurrentSnap++;
            }

        }

        #endregion

        public int CurrentSnap { get; set; }
        float _totalElapsedSeconds = 0f;
    }
}
