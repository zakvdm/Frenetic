using System;
using System.Collections.Generic;

namespace Frenetic.Network
{
    public class ClientStateTracker : IClientStateTracker
    {
        public ClientStateTracker(ISnapCounter snapCounter)
        {
            _snapCounter = snapCounter;

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
            CurrentClients.Add(new Client() { ID = ID, LastServerSnap = _snapCounter.CurrentSnap });
        }

        public List<Client> CurrentClients { get; private set; }
        ISnapCounter _snapCounter;
    }
}
