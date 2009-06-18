using System;
using Lidgren.Network;

namespace Frenetic.Network.Lidgren
{
    public class LidgrenClientNetworkSession : IClientNetworkSession
    {
        public LidgrenClientNetworkSession(INetClient netClient, IMessageSerializer messageSerializer)
        {
            _netClient = netClient;
            _messageSerializer = messageSerializer;

            ClientDisconnected += (obj, args) => Console.WriteLine("ZAK HERE: Client disconnected!");
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

        public void SendToServer(Message msg, NetChannel channel)
        {
            if (_netClient.Status != NetConnectionStatus.Connected)
                throw new System.InvalidOperationException("Client not connected to server");

            byte[] data = _messageSerializer.Serialize(msg);
            NetBuffer buffer = _netClient.CreateBuffer(data.Length);
            buffer.Write(data);

            _netClient.SendMessage(buffer, channel);
        }

        #endregion

        #region INetworkSession Members

        public void Shutdown(string reason)
        {
            _netClient.Shutdown(reason);
        }

        public Message ReadMessage()
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

                        // make hail
                        NetBuffer buf = _netClient.CreateBuffer();
                        buf.Write("Hail from " + Environment.MachineName);
                        _netClient.Connect(inBuffer.ReadIPEndPoint(), buf.ToArray());
                        break;
                    case NetMessageType.DebugMessage:
                        Console.WriteLine(inBuffer.ReadString());
                        break;
                    case NetMessageType.VerboseDebugMessage:
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
            switch (incomingMessage.Type)
            {
                case MessageType.SuccessfulJoin:
                    ClientJoined(this, new ClientStatusChangeEventArgs((int)incomingMessage.Data, true));
                    return true;
                case MessageType.NewClient:
                    ClientJoined(this, new ClientStatusChangeEventArgs((int)incomingMessage.Data, false));
                    return true;
                case MessageType.DisconnectingClient:
                    ClientDisconnected(this, new ClientStatusChangeEventArgs((int)incomingMessage.Data, false));
                    return true;
                default:
                    return false;
            }
        }

        INetClient _netClient;
        IMessageSerializer _messageSerializer;
    }
}
