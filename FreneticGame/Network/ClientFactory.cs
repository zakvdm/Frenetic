using System;

using Frenetic.Player;
using System.Collections.Generic;

namespace Frenetic.Network
{
    public class ServerSideClientFactory : IClientFactory
    {
        public ServerSideClientFactory(Client.Factory clientFactory, IPlayerList playerList)
        {
            _playerList = playerList;
            _clientFactory = clientFactory;

            _playerList.Players.Clear();
        }

        #region IClientFactory Members

        public virtual Client MakeNewClient(int ID)
        {
            Client client = _clientFactory();
            client.ID = ID;

            _playerList.Add(client.Player);

            return client;
        }

        public virtual LocalClient GetLocalClient()
        {
            throw new NotImplementedException("Server should not be trying to create local clients!");
        }

        public virtual void DeleteClient(Client client)
        {
            _playerList.Players.Remove(client.Player);
        }

        #endregion

        protected IPlayerList _playerList;
        Client.Factory _clientFactory;
    }

    public class ClientSideClientFactory : ServerSideClientFactory
    {
        public ClientSideClientFactory(Client.Factory clientFactory, IPlayerList playerList, LocalClient localClient)
            : base(clientFactory, playerList)
        {
            _localClient = localClient;
        }

        public override Client MakeNewClient(int ID)
        {
            Client client = base.MakeNewClient(ID);

            return client;
        }

        public override LocalClient GetLocalClient()
        {
            if (!_playerList.Players.Contains(_localClient.Player))
            {
                _playerList.Add(_localClient.Player);
            }

            return _localClient;
        }

        LocalClient _localClient;
    }
}
