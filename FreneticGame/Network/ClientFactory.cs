using System;
using Frenetic.Player;

namespace Frenetic.Network
{
    public class ServerSideClientFactory : IClientFactory
    {
        public ServerSideClientFactory(Client.Factory clientFactory, IGameSession gameSession)
        {
            _clientFactory = clientFactory;
            _gameSession = gameSession;
        }

        #region IClientFactory Members

        public virtual Client MakeNewClient(int ID)
        {
            Client client = _clientFactory();
            client.ID = ID;

            return client;
        }

        public virtual LocalClient GetLocalClient()
        {
            throw new NotImplementedException("Server should not be trying to create local clients!");
        }

        #endregion

        Client.Factory _clientFactory;
        protected IGameSession _gameSession;
    }

    public class ClientSideClientFactory : ServerSideClientFactory
    {
        public ClientSideClientFactory(Client.Factory clientFactory, IGameSession gameSession, PlayerView.Factory playerViewFactory, LocalClient localClient)
            : base(clientFactory, gameSession)
        {
            _playerViewFactory = playerViewFactory;
            _localClient = localClient;
        }

        public override Client MakeNewClient(int ID)
        {
            Client client = base.MakeNewClient(ID);

            _gameSession.Views.Add(_playerViewFactory(client.Player, client.PlayerSettings));

            return client;
        }

        public override LocalClient GetLocalClient()
        {
            if (_localPlayerView == null)
            {
                _localPlayerView = _playerViewFactory(_localClient.Player, _localClient.PlayerSettings);
                _gameSession.Views.Add(_localPlayerView);
            }

            return _localClient;
        }

        PlayerView.Factory _playerViewFactory;
        LocalClient _localClient;
        
        PlayerView _localPlayerView;
    }
}
