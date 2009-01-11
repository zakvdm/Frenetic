using System;

namespace Frenetic
{
    public interface IGameSessionFactory
    {
        GameSessionControllerAndView MakeServerGameSession();
        GameSessionControllerAndView MakeClientGameSession();
    }
}
