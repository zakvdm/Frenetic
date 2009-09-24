using System;

namespace Frenetic
{
    public interface IGameSessionFactory : IDisposable
    {
        GameSessionControllerAndView MakeServerGameSession();
        GameSessionControllerAndView MakeClientGameSession(string address, int port);
    }
}
