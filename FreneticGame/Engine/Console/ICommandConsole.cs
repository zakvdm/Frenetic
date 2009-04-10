using System;
using System.Collections.Generic;

namespace Frenetic
{
    public interface ICommandConsole : IConsole
    {
        MessageLog FindPossibleInputCompletions(string input);
        string TryToCompleteInput(string input);
    }
}
