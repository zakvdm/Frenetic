using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace Frenetic.Network
{
    public class IncomingMessageQueue : IIncomingMessageQueue
    {
        public IncomingMessageQueue(INetworkSession networkSession, IClientStateTracker clientStateTracker)
        {
            _networkSession = networkSession;
            _clientStateTracker = clientStateTracker;
            InitializeQueues();
        }

        public bool HasAvailable(ItemType type)
        {
            EnqueueAllWaitingItemsFromNetworkSession();

            if (_data[type].Count == 0)
                return false;

            return true;
        }

        public Item ReadItem(ItemType type)
        {
            if (!HasAvailable(type))
            {
                return null;
            }

            return _data[type].Dequeue();
        }

        private void InitializeQueues()
        {
            // Make a queue for every possible ItemType
            foreach (ItemType type in Enum.GetValues(typeof(ItemType)))
            {
                _data.Add(type, new Queue<Item>());
            }
        }

        private void EnqueueAllWaitingItemsFromNetworkSession()
        {
            // Read all incoming messages and sort through Items by ItemType
            Message msg;
            do
            {
                msg = _networkSession.ReadNextMessage();
                if (msg != null)
                {
                    foreach (Item item in msg.Items)
                    {
                        _data[item.Type].Enqueue(item);
                    }
                }
            }
            while (msg != null);

            RemoveInvalidItemsFromTheFrontOfEachQueue();
        }

        void RemoveInvalidItemsFromTheFrontOfEachQueue()
        {
            // An Item is invalid if we find no corresponding client in the ClientStateTracker (probably because the client who sent this Item has since disconnected...)
            foreach (ItemType type in Enum.GetValues(typeof(ItemType)))
            {
                if (RequiresAValidClient(type))
                {
                    while (!ItemClientIsValid(type))
                    {
                        _data[type].Dequeue();
                    }
                }
            }
        }

        bool RequiresAValidClient(ItemType type)
        {
            return ((_data[type].Count > 0) && (_data[type].Peek().ClientID != 0));
        }
        bool ItemClientIsValid(ItemType type)
        {
            return ((_data[type].Count == 0)
                || (_clientStateTracker.FindNetworkClient(_data[type].Peek().ClientID) != null)
                || ((_clientStateTracker.LocalClient != null) && (_clientStateTracker.LocalClient.ID == _data[type].Peek().ClientID)));
        }

        Dictionary<ItemType, Queue<Item>> _data = new Dictionary<ItemType,Queue<Item>>();
        INetworkSession _networkSession;
        IClientStateTracker _clientStateTracker;
    }
}
