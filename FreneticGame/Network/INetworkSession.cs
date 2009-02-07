using System;

using Lidgren.Network;
using Frenetic.Network.Lidgren;
using System.Xml.Serialization;

namespace Frenetic.Network
{
    public interface INetworkSession
    {
        void Shutdown(string reason);
        Message ReadMessage();
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
}
