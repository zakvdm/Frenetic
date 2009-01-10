using System;

using Lidgren.Network;
using Frenetic.Network.Lidgren;
using System.Xml.Serialization;

namespace Frenetic
{
    public interface INetworkSession
    {
        bool IsServer { get; }

        void Create();
        void Join(string IP, int port);
        void Join(int port);
        void Shutdown(string reason);

        void Send(Message msg, NetChannel channel);
        void Send(Message msg, NetChannel channel, INetConnection connection);
        void SendToAll(Message msg, NetChannel channel);
        void SendToAll(Message msg, NetChannel channel, INetConnection excludedConnection);
        Message ReadMessage();

        INetConnection this[int playerID] { get; }
    }
    
    public enum MessageType
    {
        PlayerData,
        Event,
        SuccessfulJoin,
        NewPlayer
    }

    
    [XmlInclude(typeof(Player))]
    public class Message
    {
        public MessageType Type { get; set; }
        public object Data { get; set; }
    }

    public class GamerJoinedEventArgs : EventArgs
    {
        public GamerJoinedEventArgs(int playerID)
        {
            this.PlayerID = playerID;
        }

        public int PlayerID { get; private set; }
    }
}
