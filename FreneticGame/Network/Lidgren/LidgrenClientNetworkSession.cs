using System;
using Lidgren.Network;
using log4net;

namespace Frenetic.Network.Lidgren
{
    public class LidgrenClientNetworkSession : IClientNetworkSession
    {
        public LidgrenClientNetworkSession(INetClient netClient, IMessageSerializer messageSerializer, ILog logger)
        {
            _netClient = netClient;
            _messageSerializer = messageSerializer;
            _logger = logger;

            ClientDisconnected += (obj, args) => _logger.Info("ZAK HERE: Client disconnected");
        }

        public event EventHandler<ClientStatusChangeEventArgs> ClientJoined;
        public event EventHandler<ClientStatusChangeEventArgs> ClientDisconnected;

        public void Dispose()
        {
            Shutdown("Cleaning up connection.");
        }

        #region IClientNetworkSession Members

        public void Join(string IP, int port)
        {
            _netClient.Start();
            _netClient.Connect(IP, port);
        }
        public void Join(int port)
        {
            _netClient.Start();
            _netClient.DiscoverLocalServers(port);
        }

        public void Send(Message msg, NetChannel channel)
        {
            if (_netClient.Status != NetConnectionStatus.Connected)
                throw new System.InvalidOperationException("Client not connected to server");

            byte[] data = _messageSerializer.Serialize(msg);
            NetBuffer buffer = _netClient.CreateBuffer(data.Length);
            buffer.Write(data);

            _netClient.SendMessage(buffer, channel);

            _logger.Debug("Sent Message with " + msg.Items.Count + " items " + " and total length " + data.Length + " bytes to the server.");
        }

        #endregion

        #region INetworkSession Members

        public void Shutdown(string reason)
        {
            _logger.Info("Ending client network session...");
            _netClient.Shutdown(reason);
        }

        public Message ReadNextMessage()
        {
            NetBuffer inBuffer = _netClient.CreateBuffer();
            NetMessageType type;

            bool messageExists = false;
            messageExists = _netClient.ReadMessage(inBuffer, out type);
            if (messageExists)
            {
                switch (type)
                {
                    case NetMessageType.ServerDiscovered:
                        // just connect to any server found!
                        _logger.Info("Found a server, connecting...");
                        // make hail
                        NetBuffer buf = _netClient.CreateBuffer();
                        buf.Write("Hail from " + Environment.MachineName);
                        _netClient.Connect(inBuffer.ReadIPEndPoint(), buf.ToArray());
                        break;
                    case NetMessageType.DebugMessage:
                        _logger.Info(inBuffer.ReadString());
                        break;
                    case NetMessageType.VerboseDebugMessage:
                        _logger.Debug(inBuffer.ReadString());
                        break;
                    case NetMessageType.Data:
                        Message msg = _messageSerializer.Deserialize(inBuffer.ReadBytes(inBuffer.LengthBytes));
                
                        if (HandleMessageFromServer(msg))
                            break;  // Message is handled in this class

                        return msg; // Message needs to be handled externally
                }
            }
            return null;
        }
        
        #endregion

        bool HandleMessageFromServer(Message incomingMessage)
        {
            var itemsToHandle = incomingMessage.Items.FindAll(NetworkSessionHandledItems);

            foreach (Item item in itemsToHandle)
            {
                switch (item.Type)
                {
                    case ItemType.SuccessfulJoin:
                        ClientJoined(this, new ClientStatusChangeEventArgs((int)item.Data, true));
                        break;
                    case ItemType.NewClient:
                        ClientJoined(this, new ClientStatusChangeEventArgs((int)item.Data, false));
                        break;
                    case ItemType.DisconnectingClient:
                        ClientDisconnected(this, new ClientStatusChangeEventArgs((int)item.Data, false));
                        break;
                }
            }

            incomingMessage.Items.RemoveAll(NetworkSessionHandledItems);

            return (incomingMessage.Items.Count == 0); // If the count is 0, then we handled all Items successfully here (so return true)
        }

        bool NetworkSessionHandledItems(Item item)
        {
            switch (item.Type)
            {
                case ItemType.SuccessfulJoin:
                case ItemType.NewClient:
                case ItemType.DisconnectingClient:
                    return true;
                default:
                    return false;
            }
        }


        INetClient _netClient;
        IMessageSerializer _messageSerializer;
        ILog _logger;
    }
}
