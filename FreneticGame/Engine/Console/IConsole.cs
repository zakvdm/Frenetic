using System;

namespace Frenetic
{
    public interface IConsole
    {
        MessageLog Log { get; set; }

        bool Active { get; set; }
        void ProcessInput(string input);
    }
}
