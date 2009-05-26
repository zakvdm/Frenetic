using System;
using System.Collections.Generic;

namespace Frenetic.Network
{
    public interface IClientStateTracker
    {
        Client FindNetworkClient(int clientID);

        Client LocalClient { get; }
        List<Client> NetworkClients { get; }
    }
}
