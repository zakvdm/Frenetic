using System;
using System.Collections.Generic;

namespace Frenetic.Network
{
    public class ClientStateTracker : IClientStateTracker
    {
        public ClientStateTracker(ISnapCounter snapCounter, INetworkSession networkSession, IClientFactory clientFactory)
        {
            _snapCounter = snapCounter;
            _networkSession = networkSession;
            _clientFactory = clientFactory;

            networkSession.ClientJoined += HandleNewClientJoined;

            NetworkClients = new List<Client>();
        }

        public Client FindNetworkClient(int clientID)
        {
            return NetworkClients.Find(client => client.ID == clientID);
        }

        void HandleNewClientJoined(object sender, ClientJoinedEventArgs newClientInfo)
        {
            if (newClientInfo.IsLocalClient)
            {
                LocalClient = _clientFactory.GetLocalClient();
                LocalClient.ID = newClientInfo.ID;
            }
            else
            {
                Client newClient = _clientFactory.MakeNewClient(newClientInfo.ID);
                newClient.LastServerSnap = _snapCounter.CurrentSnap; // No point in sending info about what happened before they joined...
                NetworkClients.Add(newClient);
            }
        }

        public Client LocalClient { get; private set; }
        public List<Client> NetworkClients { get; private set; }
        ISnapCounter _snapCounter;
        INetworkSession _networkSession;
        IClientFactory _clientFactory;
    }
}
