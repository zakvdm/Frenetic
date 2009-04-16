using System;
using System.Collections.Generic;

namespace Frenetic.Network
{
    public class ClientStateTracker : IClientStateTracker
    {
        public ClientStateTracker(ISnapCounter snapCounter, Client.Factory clientFactory)
        {
            _snapCounter = snapCounter;
            _clientFactory = clientFactory;

            CurrentClients = new List<Client>();
        }

        public Client this[int clientID]
        {
            get
            {
                return CurrentClients.Find(client => client.ID == clientID);
            }
        }

        public void AddNewClient(int ID)
        {
            Client newClient = _clientFactory();
            newClient.ID = ID;
            newClient.LastServerSnap = _snapCounter.CurrentSnap;
            CurrentClients.Add(newClient);
        }

        public List<Client> CurrentClients { get; private set; }
        ISnapCounter _snapCounter;
        Client.Factory _clientFactory;
    }
}
