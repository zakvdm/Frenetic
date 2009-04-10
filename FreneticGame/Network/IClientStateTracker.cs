using System;
using System.Collections.Generic;

namespace Frenetic.Network
{
    public interface IClientStateTracker
    {
        Client this[int clientID] { get; }
        List<Client> CurrentClients { get; }

        void AddNewClient(int ID);
    }
}
