using System;

namespace Frenetic
{
    public class SnapCounter : IController, ISnapCounter
    {
        public SnapCounter()
        {
            CurrentSnap = 1;    // Start from 1
        }

        public int SnapsPerSecond = 20;

        #region IController Members

        public void Process(float elapsedTime)
        {
            _elapsedTime += elapsedTime;

            float msPerSnap = 1000 / SnapsPerSecond;

            if (_elapsedTime  > msPerSnap)
            {
                _elapsedTime -= msPerSnap;
                CurrentSnap++;
            }

        }

        #endregion

        public int CurrentSnap { get; set; }
        float _elapsedTime = 0f;
    }
}
