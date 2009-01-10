using System;
using System.Net;

using Lidgren.Network;

namespace Frenetic.Network.Lidgren
{
    public class NetClientWrapper : INetClient
    {
        NetClient _netClient = null;

        public NetClientWrapper(NetClient netClient)
        {
            _netClient = netClient;

            _netClient.SetMessageTypeEnabled(NetMessageType.ConnectionRejected, true);
            _netClient.SetMessageTypeEnabled(NetMessageType.DebugMessage, true);
            _netClient.SetMessageTypeEnabled(NetMessageType.VerboseDebugMessage, true);
            _netClient.SetMessageTypeEnabled(NetMessageType.BadMessageReceived, true);
        }

        #region Properties
        public bool Connected
        {
            get
            {
                if (_netClient.ServerConnection == null)
                    return false;

                return (_netClient.ServerConnection.Status == NetConnectionStatus.Connected);
            }
        }
        #endregion

        #region INetClient Members

        public void Start()
        {
            _netClient.Start();
        }

        public void Connect(string IP, int port)
        {
            _netClient.Connect(IP, port, null);
        }

        public void Shutdown(string reason)
        {
            _netClient.Shutdown(reason);
        }

        public void SendMessage(NetBuffer data, NetChannel channel)
        {
            _netClient.SendMessage(data, channel);
        }

        public bool ReadMessage(NetBuffer intoBuffer, out NetMessageType type)
        {
            return _netClient.ReadMessage(intoBuffer, out type);
        }

        public NetBuffer CreateBuffer()
        {
            return _netClient.CreateBuffer();
        }

        public NetBuffer CreateBuffer(int initialCapacity)
        {
            return _netClient.CreateBuffer(initialCapacity);
        }

        #endregion

        public void DiscoverLocalServers(int port)
        {
            _netClient.DiscoverLocalServers(port);
        }

        public void Connect(IPEndPoint IPEndPoint, byte[] hail)
        {
            _netClient.Connect(IPEndPoint, hail);
        }
    }
}
