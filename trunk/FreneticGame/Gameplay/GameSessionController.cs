using System;
using Lidgren.Network;

namespace Frenetic
{
    public class GameSessionController : IController
    {
        IGameSession _gameSession;
        MessageQueue _messageQueue;
        INetworkSession _networkSession;
        NetworkPlayerController _networkPlayerController;
        public GameSessionController(IGameSession gameSession, MessageQueue messageQueue, INetworkSession networkSession)
        {
            _gameSession = gameSession;
            _messageQueue = messageQueue;
            _networkSession = networkSession;
            _networkPlayerController = new NetworkPlayerController(_messageQueue);
            _gameSession.Controllers.Add(_networkPlayerController);
        }
        #region IController Members
        public void Process()
        {
            // Handle GameSession level messages:
            if (_networkSession.IsServer)
                ReadMessagesAsServer();
            else
                ReadMessagesAsClient();

            // Update all controllers and views:
            foreach (IController controller in _gameSession.Controllers)
            {
                controller.Process();
            }
            foreach (IView view in _gameSession.Views)
            {
                view.Generate();
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
                Player newPlayer = new Player(newID);
                
                // send ack to new player:
                _networkSession.Send(new Message() { Type = MessageType.SuccessfulJoin, Data = newID }, NetChannel.ReliableInOrder1, _networkSession[newID]);
                
                // send existent players' info to new player:
                foreach (int currentID in _networkPlayerController.Players.Keys)
                {
                    _networkSession.Send(new Message() { Type = MessageType.NewPlayer, Data = currentID }, NetChannel.ReliableUnordered, _networkSession[newID]);
                }

                // tell existent players about new player:
                _networkSession.SendToAll(new Message() { Type = MessageType.NewPlayer, Data = newID }, NetChannel.ReliableUnordered, _networkSession[newID]);

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
                Player newPlayer = new Player(ID);
                _networkPlayerController.Players.Add(ID, newPlayer);
                _gameSession.Views.Add(new PlayerView(newPlayer));
            }
            while (true)
            {
                object data = _messageQueue.ReadMessage(MessageType.SuccessfulJoin);
                if (data == null)
                    break;
                int ID = (int)data;
                Player localPlayer = new Player(ID);
                _gameSession.Controllers.Add(new KeyboardPlayerController(localPlayer));
                //_networkPlayerController.Players.Add(ID, localPlayer);
                _gameSession.Views.Add(new NetworkPlayerView(localPlayer, _networkSession));
                _gameSession.Views.Add(new PlayerView(localPlayer));
            }

        }
    }
}
