using System;
using System.Collections.Generic;

namespace Frenetic
{
    public interface ICommandConsole : IConsole<string>
    {
        Log<string> FindPossibleInputCompletions(string input);
        string TryToCompleteInput(string input);
    }
}
