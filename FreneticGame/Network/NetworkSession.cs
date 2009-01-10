using System;
using System.Collections.Generic;
using System.Text;

namespace Frenetic
{
    public class NetworkSession
    {
        private INetworkServer _networkServer = null;
        private INetworkClient _networkClient = null;

        public bool IsServer
        {
            get
            {
                if (_networkServer != null)
                    return true;
                return false;
            }
        }

        public NetworkSession(INetworkServer networkServer)
        {
            _networkServer = networkServer;
        }
        public NetworkSession(INetworkClient networkClient)
        {
            _networkClient = networkClient;
        }

        public void Start()
        {
            if (!IsServer)
                throw new System.Exception("Client can't start session");

            _networkServer.Start();
        }
    }
}
