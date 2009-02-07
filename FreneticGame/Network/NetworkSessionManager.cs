using System;


namespace Frenetic.Network
{
    public class NetworkSessionManager
    {
        public NetworkSessionManager(IClientNetworkSession clientNetworkSession, IServerNetworkSession serverNetworkSession)
        {
            _serverNetworkSession = serverNetworkSession;
            _clientNetworkSession = clientNetworkSession;
        }

        public void Start(int port)
        {
            _serverNetworkSession.Create(port);
        }
        public void Join(int port)
        {
            _clientNetworkSession.Join(port);
        }

        IServerNetworkSession _serverNetworkSession;
        IClientNetworkSession _clientNetworkSession;
    }
}
