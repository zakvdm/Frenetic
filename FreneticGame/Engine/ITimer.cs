using System;

namespace Frenetic.Engine
{
    public interface ITimer
    {
        void AddActionTimer(float durationOfTimer, Action action);

        // STOPWATCH:
        void StartStopWatch();
        float StopWatchReading { get; }
    }
}
