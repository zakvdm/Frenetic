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
            if (!_netClient.Connected)
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
                        return null;
                    case NetMessageType.DebugMessage:
                        Console.WriteLine(inBuffer.ReadString());
                        return null;
                    case NetMessageType.VerboseDebugMessage:
                        return null;
                    case NetMessageType.Data:
                        return _messageSerializer.Deserialize(inBuffer.ReadBytes(inBuffer.LengthBytes));
                }
            }
            return null;
        }
        
        #endregion

        INetClient _netClient;
        IMessageSerializer _messageSerializer;
    }
}
