using System;

namespace Frenetic
{
    public interface IGameSessionFactory
    {
        IController MakeServerGameSession();
        IController MakeClientGameSession();
    }
}
