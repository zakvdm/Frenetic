using System;

namespace Frenetic
{
    public interface IMessageConsole : IConsole 
    {
        bool HasNewMessages { get; }

        string GetNewMessage();
    }
}
