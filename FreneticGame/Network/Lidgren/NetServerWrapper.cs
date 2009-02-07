using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Lidgren.Network;

namespace Frenetic.Network.Lidgren
{
    public class NetServerWrapper : INetServer
    {
        NetServer _netServer = null;

        public NetServerWrapper(NetServer netServer)
        {
            _netServer = netServer;

            _netServer.SetMessageTypeEnabled(NetMessageType.ConnectionApproval, true);
        }

        #region INetServer Members
        public int Port
        {
            get { return _netServer.Configuration.Port; }
            set { _netServer.Configuration.Port = value; }
        }
        public bool IsListening
        {
            get { return _netServer.IsListening; }
        }
        public bool Connected
        {
            get { return (_netServer.Connections.Count > 0); }
        }

        public void Start()
        {
            _netServer.Start();
        }

        public bool ReadMessage(NetBuffer intoBuffer, out NetMessageType type, out INetConnection sender)
        {
            NetConnection senderInternal;
            bool messageExists = _netServer.ReadMessage(intoBuffer, out type, out senderInternal);
            sender = new NetConnectionWrapper(senderInternal);
            return messageExists;
        }
        public NetBuffer CreateBuffer()
        {
            return _netServer.CreateBuffer();
        }
        public NetBuffer CreateBuffer(int initialCapacity)
        {
            return _netServer.CreateBuffer(initialCapacity);
        }

        public void SendToAll(NetBuffer data, NetChannel channel)
        {
            _netServer.SendToAll(data, channel);
        }

        public void SendToAll(NetBuffer data, NetChannel channel, INetConnection exclude)
        {
            _netServer.SendToAll(data, channel, exclude.NetConnection);
        }

        public void SendMessage(NetBuffer data, NetChannel channel, INetConnection connection)
        {
            _netServer.SendMessage(data, connection.NetConnection, channel);
        }

        public void Shutdown(string reason)
        {
            _netServer.Shutdown(reason);
        }

        #endregion
    }
}
