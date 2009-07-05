using System;
using System.Linq;
using System.Collections.Generic;

namespace Frenetic.Engine
{
    public class Timer : ITimer
    {
        public void AddActionTimer(float durationOfTimer, Action action)
        {
            _timers.Add(new ActionTimer(_elapsedTime + durationOfTimer, action));

            _timers = _timers.OrderBy((timer) => timer.ExpirationTime).ToList();
        }

        #region IController Members

        public void Process(float elapsedSeconds)
        {
            _elapsedTime += elapsedSeconds;

            while (_timers.Count > 0 && _timers[0].ExpirationTime <= _elapsedTime)
            {
                _timers[0].Action();
                _timers.RemoveAt(0);
            }
        }

        #endregion

        float _elapsedTime = 0f;

        List<ActionTimer> _timers = new List<ActionTimer>();

        class ActionTimer
        {
            public ActionTimer(float expiration, Action action)
            {
                this.ExpirationTime = expiration;
                this.Action = action;
            }

            public float ExpirationTime { get; set; }
            public Action Action { get; set; }
        }
    }
}
