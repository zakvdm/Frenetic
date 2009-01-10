using System;
using System.Collections.Generic;
using System.Text;
using System.Net;

using Lidgren.Network;

namespace Frenetic.Network.Lidgren
{
    public class LidgrenNetworkSession : INetworkSession
    {
        public LidgrenNetworkSession(INetServer networkServer)
        {
            _networkServer = networkServer;
        }
        public LidgrenNetworkSession(INetClient networkClient)
        {
            _networkClient = networkClient;
        }

        //public event EventHandler<GamerJoinedEventArgs> GamerJoined;

        public bool IsServer
        {
            get
            {
                if (_networkServer != null)
                    return true;
                return false;
            }
        }

        Dictionary<int, INetConnection> _connections = new Dictionary<int, INetConnection>();
        public INetConnection this[int playerID]
        {
            get 
            {
                return _connections[playerID];
            }
        }

        public void Create()
        {
            if (!IsServer)
                throw new System.InvalidOperationException("Client can't start session");

            if (_networkServer.IsListening)
                throw new System.InvalidOperationException("Session already created");
            
            _networkServer.Start();
        }
        public void Join(string IP, int port)
        {
            if (IsServer)
                throw new System.InvalidOperationException("Server can't join session");

            _networkClient.Start();
            _networkClient.Connect(IP, port);
        }
        public void Join(int port)
        {
            if (IsServer)
                throw new System.InvalidOperationException("Server can't join session");

            _networkClient.Start();
            _networkClient.DiscoverLocalServers(port);
        }

        public void Send(Message msg, NetChannel channel)
        {
            if (!Connected)
                throw new System.InvalidOperationException("Client not connected to server");

            byte[] data = _serializer.Serialize(msg);
            NetBuffer buffer = _networkClient.CreateBuffer(data.Length);
            buffer.Write(data);

            _networkClient.SendMessage(buffer, channel);
        }
        public void Send(Message msg, NetChannel channel, INetConnection connection)
        {
            // TODO: unit test... Should check (connection != null) instead of !Connected???
            if (!Connected)
                throw new System.InvalidOperationException("Client not connected to server");

            byte[] data = _serializer.Serialize(msg);
            NetBuffer buffer = _networkServer.CreateBuffer(data.Length);
            buffer.Write(data);

            _networkServer.SendMessage(buffer, channel, connection);
        }
        public void SendToAll(Message msg, NetChannel channel)
        {
            if (!IsServer)
                throw new System.InvalidOperationException("Client can't send to all");

            byte[] data = _serializer.Serialize(msg);
            NetBuffer buffer = _networkServer.CreateBuffer(data.Length);
            buffer.Write(data);

            _networkServer.SendToAll(buffer, channel);
        }
        public void SendToAll(Message msg, NetChannel channel, INetConnection excludedConnection)
        {
            if (!IsServer)
                throw new System.InvalidOperationException("Client can't send to all");

            byte[] data = _serializer.Serialize(msg);
            NetBuffer buffer = _networkServer.CreateBuffer(data.Length);
            buffer.Write(data);

            _networkServer.SendToAll(buffer, channel, excludedConnection);
        }
        
        public Message ReadMessage()
        {
            if (IsServer)
                return ServerReceiveMessage();
            else
                return ClientReceiveMessage();
        }
        private Message ServerReceiveMessage()
        {
            NetBuffer inBuffer = _networkServer.CreateBuffer();
            NetMessageType type;
            INetConnection sender;
            bool messageExists = false;
            messageExists = _networkServer.ReadMessage(inBuffer, out type, out sender);
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
                        return _serializer.Deserialize(inBuffer.ReadBytes(inBuffer.LengthBytes));
                }
            }
            return null;
        }
        private Message ClientReceiveMessage()
        {
            NetBuffer inBuffer = _networkClient.CreateBuffer();
            NetMessageType type;

            bool messageExists = false;
            messageExists = _networkClient.ReadMessage(inBuffer, out type);
            if (messageExists)
            {
                switch (type)
                {
                    case NetMessageType.ServerDiscovered:
                        // just connect to any server found!

                        // make hail
                        NetBuffer buf = _networkClient.CreateBuffer();
                        buf.Write("Hail from " + Environment.MachineName);
                        _networkClient.Connect(inBuffer.ReadIPEndPoint(), buf.ToArray());
                        return null;
                    case NetMessageType.DebugMessage:
                        Console.WriteLine(inBuffer.ReadString());
                        return null;
                    case NetMessageType.VerboseDebugMessage:
                        return null;
                    case NetMessageType.Data:
                        return _serializer.Deserialize(inBuffer.ReadBytes(inBuffer.LengthBytes));
                }
            }
            return null;
        }

        public void Shutdown(string reason)
        {
            if (IsServer)
                _networkServer.Shutdown(reason);
            else
                _networkClient.Shutdown(reason);

        }

        private INetServer _networkServer = null;
        private INetClient _networkClient = null;

        //private Dictionary<int, Gamer> _gamers = new Dictionary<int, Gamer>();
        private IMessageSerializer _serializer = new XmlMessageSerializer();

        private bool Connected
        {
            get
            {
                if (IsServer)
                    return _networkServer.Connected;
                else
                    return _networkClient.Connected;
            }
        }
    }

    
}
