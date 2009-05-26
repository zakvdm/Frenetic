using System;

using Lidgren.Network;
using Frenetic.Network.Lidgren;
using System.Xml.Serialization;
using Frenetic.Player;

namespace Frenetic.Network
{
    public interface INetworkSession : IDisposable
    {
        event EventHandler<ClientJoinedEventArgs> ClientJoined;

        void Shutdown(string reason);
        Message ReadMessage();
    }
    
    public enum MessageType
    {
        ServerSnap,
        ClientSnap,
        Player,
        PlayerSettings,
        Event,
        SuccessfulJoin,
        NewPlayer,
        ChatLog
    }

    public class ClientJoinedEventArgs : EventArgs
    {
        public ClientJoinedEventArgs(int ID, bool isLocalClient)
        {
            this.ID = ID;
            IsLocalClient = isLocalClient;
        }

        public int ID { get; private set; }
        public bool IsLocalClient { get; private set; }
    }

    
    [XmlInclude(typeof(Frenetic.Player.Player))]
    [XmlInclude(typeof(NetworkPlayerSettings))]
    [XmlInclude(typeof(LocalPlayerSettings))]
    [XmlInclude(typeof(ChatMessage))]
    public class Message
    {
        public int ClientID { get; set; }
        public MessageType Type { get; set; }
        public object Data { get; set; }
    }
}
