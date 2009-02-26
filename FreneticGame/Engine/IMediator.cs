using System;
using System.Collections.Generic;
namespace Frenetic
{
    public interface IMediator
    {
        List<string> AvailableCommands { get; }
        string Do(string commandName, string argument);
        string Get(string propertyName);
        void Register(string name, Func<string, string> command);
    }
}
