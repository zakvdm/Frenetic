using System;
using Lidgren.Network;

namespace Frenetic
{
    public class GameSessionController : IController
    {
        
        public GameSessionController(IGameSession gameSession, MessageQueue messageQueue, INetworkSession networkSession, IViewFactory viewFactory, Player.Factory playerFactory, IPlayer localPlayer, ICamera camera)
        {
            _gameSession = gameSession;
            _messageQueue = messageQueue;
            _networkSession = networkSession;
            _viewFactory = viewFactory;
            _playerFactory = playerFactory;
            _localPlayer = localPlayer;
            _camera = camera;
            _networkPlayerController = new NetworkPlayerController(_messageQueue);
            _gameSession.Controllers.Add(_networkPlayerController);
        }
        #region IController Members
        public void Process(long ticks)
        {
            // Handle GameSession level messages:
            if (_networkSession.IsServer)
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
                object data = _messageQueue.ReadMessage(MessageType.NewPlayer);
                if (data == null)
                    break;
                int newID = (int)data;
                IPlayer newPlayer = _playerFactory(newID);
                
                // TODO: Consider moving these sends into the GameSessionView?

                // send ack to new player:
                _networkSession.SendTo(new Message() { Type = MessageType.SuccessfulJoin, Data = newID }, NetChannel.ReliableInOrder1, _networkSession[newID]);
                
                // send existent players' info to new player:
                foreach (int currentID in _networkPlayerController.Players.Keys)
                {
                    _networkSession.SendTo(new Message() { Type = MessageType.NewPlayer, Data = currentID }, NetChannel.ReliableUnordered, _networkSession[newID]);
                }

                // tell existent players about new player:
                _networkSession.SendToAllExcept(new Message() { Type = MessageType.NewPlayer, Data = newID }, NetChannel.ReliableUnordered, _networkSession[newID]);

                _networkPlayerController.Players.Add(newID, newPlayer);
                _gameSession.Views.Add(new NetworkPlayerView(newPlayer, _networkSession));
            }
        }

        private void ReadMessagesAsClient()
        {
            while (true)
            {
                object data = _messageQueue.ReadMessage(MessageType.NewPlayer);
                if (data == null)
                    break;
                int ID = (int)data;
                IPlayer newPlayer = _playerFactory(ID);
                _networkPlayerController.Players.Add(ID, newPlayer);
                _gameSession.Views.Add(_viewFactory.MakePlayerView(newPlayer, _camera));
            }
            while (true)
            {
                object data = _messageQueue.ReadMessage(MessageType.SuccessfulJoin);
                if (data == null)
                    break;
                int ID = (int)data;
                _localPlayer.ID = ID;
                _gameSession.Views.Add(_viewFactory.MakePlayerView(_localPlayer, _camera));
            }

        }

        IGameSession _gameSession;
        MessageQueue _messageQueue;
        INetworkSession _networkSession;
        IViewFactory _viewFactory;
        Player.Factory _playerFactory;
        IPlayer _localPlayer;
        ICamera _camera;
        NetworkPlayerController _networkPlayerController;
    }
}
