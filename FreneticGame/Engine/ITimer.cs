using System;

namespace Frenetic.Engine
{
    public interface ITimer : IController
    {
        void AddActionTimer(float durationOfTimer, Action action);
    }
}
