using System;

using Lidgren.Network;
using Frenetic.Network.Lidgren;
using System.Xml.Serialization;
using Frenetic.Player;
using Frenetic.Weapons;
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
        ServerSnap,
        ClientSnap,
        Player,
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

   
    [XmlInclude(typeof(Frenetic.Player.Player))]
    [XmlInclude(typeof(PlayerState))]
    [XmlInclude(typeof(NetworkPlayerSettings))]
    [XmlInclude(typeof(LocalPlayerSettings))]
    [XmlInclude(typeof(ChatMessage))]
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
