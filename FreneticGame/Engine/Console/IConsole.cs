using System;

namespace Frenetic
{
    public interface IConsole<T>
    {
        Log<T> Log { get; set; }

        bool Active { get; set; }
        void ProcessInput(string input);
    }
}
