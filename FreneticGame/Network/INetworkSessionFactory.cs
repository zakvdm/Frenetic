using System;

namespace Frenetic
{
    public interface INetworkSessionFactory
    {
        INetworkSession MakeServerNetworkSession();
        INetworkSession MakeClientNetworkSession();
    }
}
