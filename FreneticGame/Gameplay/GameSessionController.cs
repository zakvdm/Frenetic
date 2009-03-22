using System;
using Lidgren.Network;
using Frenetic.Network;
using Frenetic.Network.Lidgren;

namespace Frenetic
{
    public class GameSessionController : IController
    {
        public GameSessionController(IGameSession gameSession, IIncomingMessageQueue incomingMessageQueue, IOutgoingMessageQueue outgoingMessageQueue, Player.Factory playerFactory, IPlayer localPlayer)
            : this(gameSession, incomingMessageQueue, outgoingMessageQueue, playerFactory, null, localPlayer, null, true)
        {
        }

        public GameSessionController(IGameSession gameSession, IIncomingMessageQueue incomingMessageQueue, IOutgoingMessageQueue outgoingMessageQueue, Player.Factory playerFactory, PlayerView.Factory playerViewFactory, IPlayer localPlayer, ICamera camera, bool isServer)
        {
            _gameSession = gameSession;
            _incomingMessageQueue = incomingMessageQueue;
            _outgoingMessageQueue = outgoingMessageQueue;
            _playerFactory = playerFactory;
            _playerViewFactory = playerViewFactory;
            _localPlayer = localPlayer;
            _camera = camera;
            _isServer = isServer;

            _networkPlayerController = new NetworkPlayerController(_incomingMessageQueue);
            _gameSession.Controllers.Add(_networkPlayerController);
        }
        #region IController Members
        public void Process(long ticks)
        {
            // Handle GameSession level messages:
            if (_isServer)
                ReadMessagesAsServer();
            else
                ReadMessagesAsClient();

            // Update all gamesession controllers:
            foreach (IController controller in _gameSession.Controllers)
            {
                controller.Process(ticks);
            }
        }
        #endregion

        private void ReadMessagesAsServer()
        {
            while (true)
            {
                object data = _incomingMessageQueue.ReadMessage(MessageType.NewPlayer);
                if (data == null)
                    break;
                int newID = (int)data;
                IPlayer newPlayer = _playerFactory(newID);
                
                // TODO: Consider moving these sends into the GameSessionView?

                // send ack to new player:
                _outgoingMessageQueue.WriteFor(new Message() { Type = MessageType.SuccessfulJoin, Data = newID }, NetChannel.ReliableInOrder1, newID);
                
                // send existent players' info to new player:
                foreach (int currentID in _networkPlayerController.Players.Keys)
                {
                    _outgoingMessageQueue.WriteFor(new Message() { Type = MessageType.NewPlayer, Data = currentID }, NetChannel.ReliableUnordered, newID);
                }

                // tell existent players about new player:
                _outgoingMessageQueue.WriteForAllExcept(new Message() { Type = MessageType.NewPlayer, Data = newID }, NetChannel.ReliableUnordered, newID);

                _networkPlayerController.Players.Add(newID, newPlayer);
                _gameSession.Views.Add(new NetworkPlayerView(newPlayer, _outgoingMessageQueue));
            }
        }

        private void ReadMessagesAsClient()
        {
            while (true)
            {
                object data = _incomingMessageQueue.ReadMessage(MessageType.NewPlayer);
                if (data == null)
                    break;
                int ID = (int)data;
                IPlayer newPlayer = _playerFactory(ID);
                _networkPlayerController.Players.Add(ID, newPlayer);
                _gameSession.Views.Add(_playerViewFactory(newPlayer));
            }
            while (true)
            {
                object data = _incomingMessageQueue.ReadMessage(MessageType.SuccessfulJoin);
                if (data == null)
                    break;
                int ID = (int)data;
                _localPlayer.ID = ID;
                //_gameSession.Views.Add(_playerViewFactory(_localPlayer));
            }

        }

        IGameSession _gameSession;
        IIncomingMessageQueue _incomingMessageQueue;
        IOutgoingMessageQueue _outgoingMessageQueue;
        bool _isServer = false;
        Player.Factory _playerFactory;
        PlayerView.Factory _playerViewFactory;
        IPlayer _localPlayer;
        ICamera _camera;
        NetworkPlayerController _networkPlayerController;
    }
}
