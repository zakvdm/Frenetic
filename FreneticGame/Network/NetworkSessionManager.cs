using System;


namespace Frenetic.Network
{
    public class NetworkSessionManager : IDisposable
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

        #region IDisposable Members

        public void Dispose()
        {
            if (_clientNetworkSession != null)
                _clientNetworkSession.Shutdown("Killing session...");
            if (_serverNetworkSession != null)
                _serverNetworkSession.Shutdown("Killing session...");
            GC.SuppressFinalize(this);
        }

        ~NetworkSessionManager()
        {
            Console.WriteLine("WARNING: Dispose not called on NetworkSessionManager!");
        }

        #endregion
    }
}
