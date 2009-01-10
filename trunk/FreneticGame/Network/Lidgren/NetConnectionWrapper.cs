using System;
using System.Net;

using Lidgren.Network;

namespace Frenetic.Network.Lidgren
{
    class NetConnectionWrapper : INetConnection
    {
        public NetConnection NetConnection { get; private set; }
        public NetConnectionWrapper(NetConnection netConnection)
        {
            NetConnection = netConnection;
        }

        public IPEndPoint RemoteEndPoint
        {
            get
            {
                return NetConnection.RemoteEndpoint;
            }
        }
        public int ConnectionID
        {
            get
            {
                return NetConnection.RemoteEndpoint.GetHashCode();
            }
        }
        public NetConnectionStatus Status
        {
            get
            {
                return NetConnection.Status;
            }
        }

        #region INetConnection Members
        public void Approve()
        {
            NetConnection.Approve(System.Text.Encoding.ASCII.GetBytes("Hello from server"));
        }
        #endregion
    }
}
