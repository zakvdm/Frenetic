using System;

using Lidgren.Network;
using Frenetic.Network.Lidgren;
using System.Xml.Serialization;

namespace Frenetic.Network
{
    public interface INetworkSession : IDisposable
    {
        void Shutdown(string reason);
        Message ReadMessage();
    }
    
    public enum MessageType
    {
        ServerSnap,
        ClientSnap,
        PlayerData,
        Event,
        SuccessfulJoin,
        NewPlayer,
        ChatLog
    }

    
    [XmlInclude(typeof(Player))]
    [XmlInclude(typeof(ChatMessage))]
    public class Message
    {
        public int ClientID { get; set; }
        public MessageType Type { get; set; }
        public object Data { get; set; }
    }
}
