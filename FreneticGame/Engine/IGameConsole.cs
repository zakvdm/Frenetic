using System;
using System.Collections.Generic;

namespace Frenetic
{
    public interface IGameConsole
    {
        bool Active { get; set; }
        List<string> CommandLog { get; set; }
        List<Command> FindPossibleInputCompletions();
        void TryToCompleteCurrentInput();
        string CurrentInput { get; set; }
        List<string> MessageLog { get; set; }
        void ProcessInput();
    }
}
