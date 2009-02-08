using System;
using Lidgren.Network;
using System.Collections.Generic;

namespace Frenetic.Network.Lidgren
{
    public class LidgrenServerNetworkSession : IServerNetworkSession
    {
        public LidgrenServerNetworkSession(INetServer netServer, IMessageSerializer messageSerializer)
        {
            _netServer = netServer;
            _messageSerializer = messageSerializer;
        }



        #region IServerNetworkSession Members

        public void Create(int port)
        {
            if (_netServer.IsListening)
                throw new System.InvalidOperationException("Session already created");

            _netServer.Port = port;
            _netServer.Start();
        }

        public void SendTo(Message msg, NetChannel channel, int destinationPlayerID)
        {
            if ((!ActiveConnections.ContainsKey(destinationPlayerID)) || (ActiveConnections[destinationPlayerID].Status != NetConnectionStatus.Connected))
                throw new System.InvalidOperationException("Not a valid player");

            byte[] data = _messageSerializer.Serialize(msg);
            NetBuffer buffer = _netServer.CreateBuffer(data.Length);
            buffer.Write(data);

            _netServer.SendMessage(buffer, channel, ActiveConnections[destinationPlayerID]);
        }
        public void SendToAll(Message msg, NetChannel channel)
        {
            byte[] data = _messageSerializer.Serialize(msg);
            NetBuffer buffer = _netServer.CreateBuffer(data.Length);
            buffer.Write(data);

            _netServer.SendToAll(buffer, channel);
        }
        public void SendToAllExcept(Message msg, NetChannel channel, int excludedPlayerID)
        {
            if (!ActiveConnections.ContainsKey(excludedPlayerID))
                throw new System.InvalidOperationException("Excluded player not connected to network session");

            byte[] data = _messageSerializer.Serialize(msg);
            NetBuffer buffer = _netServer.CreateBuffer(data.Length);
            buffer.Write(data);

            _netServer.SendToAll(buffer, channel, ActiveConnections[excludedPlayerID]);
        }

        #endregion

        #region INetworkSession Members

        public void Shutdown(string reason)
        {
            _netServer.Shutdown(reason);
        }

        public Message ReadMessage()
        {
            NetBuffer inBuffer = _netServer.CreateBuffer();
            NetMessageType type;
            INetConnection sender;
            bool messageExists = false;
            messageExists = _netServer.ReadMessage(inBuffer, out type, out sender);
            if (messageExists)
            {
                switch (type)
                {
                    case NetMessageType.ConnectionApproval:
                        sender.Approve();
                        break;
                    case NetMessageType.StatusChanged:
                        Console.WriteLine("Status for " + sender.ConnectionID.ToString() + " is: " + sender.Status);
                        if ((sender.Status == NetConnectionStatus.Connected) && (!_connections.ContainsKey(sender.ConnectionID)))
                        {
                            _connections.Add(sender.ConnectionID, sender);
                            return new Message() { Type = MessageType.NewPlayer, Data = sender.ConnectionID };
                        }
                        break;
                    case NetMessageType.DebugMessage:
                        Console.WriteLine(inBuffer.ReadString());
                        break;
                    case NetMessageType.Data:
                        return _messageSerializer.Deserialize(inBuffer.ReadBytes(inBuffer.LengthBytes));
                }
            }
            return null;
        }

        Dictionary<int, INetConnection> _connections = new Dictionary<int, INetConnection>();
        public Dictionary<int, INetConnection> ActiveConnections
        {
            get
            {
                return _connections;
            }
        }

        #endregion

        INetServer _netServer;
        IMessageSerializer _messageSerializer;
    }
}
