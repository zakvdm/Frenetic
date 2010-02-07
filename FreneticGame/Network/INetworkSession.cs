using System;

using Lidgren.Network;
using Frenetic.Network.Lidgren;
using Frenetic.Player;
using Frenetic.Gameplay.Weapons;
using System.Collections.Generic;

namespace Frenetic.Network
{
    public interface INetworkSession : IDisposable
    {
        event EventHandler<ClientStatusChangeEventArgs> ClientJoined;
        event EventHandler<ClientStatusChangeEventArgs> ClientDisconnected;

        void Send(Message msg, NetChannel channel);

        void Shutdown(string reason);
        Message ReadNextMessage();
    }
    
    public enum ItemType
    {
        Player,
        PlayerInput,
        PlayerSettings,
        Event,
        SuccessfulJoin,
        NewClient,
        DisconnectingClient,
        ChatLog
    }

    public class ClientStatusChangeEventArgs : EventArgs
    {
        public ClientStatusChangeEventArgs(int ID, bool isLocalClient)
        {
            this.ID = ID;
            IsLocalClient = isLocalClient;
        }

        public int ID { get; private set; }
        public bool IsLocalClient { get; private set; }
    }

   
    public class Message
    {
        public Message() { Items = new List<Item>(); }

        public List<Item> Items { get; set; }
    }

    public class Item
    {
        public int ClientID { get; set; }
        public ItemType Type { get; set; }
        public object Data { get; set; }
    }
}
