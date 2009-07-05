using System;

using Frenetic.Player;

namespace Frenetic.Network
{
    public class ServerSideClientFactory : IClientFactory
    {
        public ServerSideClientFactory(Client.Factory clientFactory, IGameSession gameSession, PlayerUpdater playerUpdater)
        {
            _clientFactory = clientFactory;
            _gameSession = gameSession;
            _playerUpdater = playerUpdater;
        }

        #region IClientFactory Members

        public virtual Client MakeNewClient(int ID)
        {
            Client client = _clientFactory();
            client.ID = ID;

            if (_playerUpdater != null)
            {
                _playerUpdater.AddPlayer(client.Player);
            }

            return client;
        }

        public virtual LocalClient GetLocalClient()
        {
            throw new NotImplementedException("Server should not be trying to create local clients!");
        }

        public virtual void DeleteClient(Client client)
        {
            if (_playerUpdater != null)
            {
                _playerUpdater.RemovePlayer(client.Player);
            }
        }

        #endregion

        Client.Factory _clientFactory;
        protected IGameSession _gameSession;

        PlayerUpdater _playerUpdater;
    }

    public class ClientSideClientFactory : ServerSideClientFactory
    {
        public ClientSideClientFactory(Client.Factory clientFactory, IGameSession gameSession, PlayerView playerView, LocalClient localClient)
            : base(clientFactory, gameSession, null)
        {
            _playerView = playerView;
            _localClient = localClient;
        }

        public override Client MakeNewClient(int ID)
        {
            Client client = base.MakeNewClient(ID);

            _playerView.AddPlayer(client.Player, client.PlayerSettings);

            return client;
        }

        public override LocalClient GetLocalClient()
        {
            if (!_playerView.Players.Contains(_localClient.Player))
            {
                _playerView.AddPlayer(_localClient.Player, _localClient.PlayerSettings);
            }

            return _localClient;
        }

        public override void DeleteClient(Client client)
        {
            base.DeleteClient(client);

            _playerView.RemovePlayer(client.Player);
        }

        LocalClient _localClient;
        PlayerView _playerView;
    }
}
