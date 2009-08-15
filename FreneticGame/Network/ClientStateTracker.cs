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
            networkSession.ClientDisconnected += HandleClientDisconnect;

            NetworkClients = new List<Client>();
        }

        public Client FindNetworkClient(int clientID)
        {
            return NetworkClients.Find(client => client.ID == clientID);
        }

        void HandleNewClientJoined(object sender, ClientStatusChangeEventArgs newClientInfo)
        {
            if (newClientInfo.IsLocalClient)
            {
                LocalClient = _clientFactory.GetLocalClient();
                LocalClient.ID = newClientInfo.ID;
            }
            else
            {
                Client newClient = _clientFactory.MakeNewClient(newClientInfo.ID);
                NetworkClients.Add(newClient);
            }
        }

        void HandleClientDisconnect(object sender, ClientStatusChangeEventArgs disconnectingClientInfo)
        {
            Client disconnectingClient = FindNetworkClient(disconnectingClientInfo.ID);

            NetworkClients.Remove(disconnectingClient);

            // We call this to allow for any other cleanup that needs to be done per client (such as removing PlayerViews, etc.)
            _clientFactory.DeleteClient(disconnectingClient);
        }

        public Client LocalClient { get; private set; }
        public List<Client> NetworkClients { get; private set; }
        ISnapCounter _snapCounter;
        INetworkSession _networkSession;
        IClientFactory _clientFactory;
    }
}
