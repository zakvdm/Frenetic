using System;
using Lidgren.Network;

namespace Frenetic
{
    public class GameSessionFactory : IGameSessionFactory
    {
        INetworkSessionFactory _networkSessionFactory;
        INetworkSession _networkSession;
        MessageQueue _messageQueue;
        IGameSession _gameSession;
        GameSessionController _gameSessionController;

        public GameSessionFactory(INetworkSessionFactory networkSessionFactory)
        {
            _networkSessionFactory = networkSessionFactory;
        }
        #region IGameSessionFactory Members

        public IController MakeServerGameSession()
        {
            _networkSession = _networkSessionFactory.MakeServerNetworkSession();
            _messageQueue = new MessageQueue(_networkSession);
            _gameSession = new GameSession();
            _gameSessionController = new GameSessionController(_gameSession, _messageQueue, _networkSession);

            /*
            NetworkWorldView nwv = new NetworkWorldView();

            _gameSession.Views.Add(nwv);
            */

            return _gameSessionController;
        }

        public IController MakeClientGameSession()
        {
            _networkSession = _networkSessionFactory.MakeClientNetworkSession();
            _messageQueue = new MessageQueue(_networkSession);
            _gameSession = new GameSession();
            _gameSessionController = new GameSessionController(_gameSession, _messageQueue, _networkSession);

            return _gameSessionController;
        }

        #endregion
    }
}
