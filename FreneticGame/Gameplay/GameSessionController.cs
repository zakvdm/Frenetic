using System;
using Lidgren.Network;
using Frenetic.Network;
using Frenetic.Network.Lidgren;

namespace Frenetic
{
    public class GameSessionController : IController
    {
        // Server constructor:
        public GameSessionController(IGameSession gameSession, IIncomingMessageQueue incomingMessageQueue, IOutgoingMessageQueue outgoingMessageQueue, IClientStateTracker clientStateTracker)
            : this(gameSession, incomingMessageQueue, outgoingMessageQueue, clientStateTracker, null, null, true)
        { }

        // Client constructor:
        public GameSessionController(IGameSession gameSession, IIncomingMessageQueue incomingMessageQueue, IOutgoingMessageQueue outgoingMessageQueue, IClientStateTracker clientStateTracker, PlayerView.Factory playerViewFactory, LocalClient localClient, bool isServer)
        {
            _gameSession = gameSession;
            _incomingMessageQueue = incomingMessageQueue;
            _outgoingMessageQueue = outgoingMessageQueue;
            _clientStateTracker = clientStateTracker;
            _playerViewFactory = playerViewFactory;
            _localClient = localClient;
            _isServer = isServer;
        }
        #region IController Members
        public void Process(float elapsedTime)
        {
            // Handle GameSession level messages:
            if (_isServer)
                ReadMessagesAsServer();
            else
                ReadMessagesAsClient();

            // Update all gamesession controllers:
            foreach (IController controller in _gameSession.Controllers)
            {
                controller.Process(elapsedTime);
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

                // TODO: Consider moving these sends into the GameSessionView?

                // send ack to new player:
                _outgoingMessageQueue.WriteFor(new Message() { Type = MessageType.SuccessfulJoin, Data = newID }, NetChannel.ReliableInOrder1, newID);
                
                // send existent players' info to new player:
                foreach (Client client in _clientStateTracker.CurrentClients)
                {
                    int currentID = client.ID;
                    _outgoingMessageQueue.WriteFor(new Message() { Type = MessageType.NewPlayer, Data = currentID }, NetChannel.ReliableUnordered, newID);
                }

                // tell existent players about new player:
                _outgoingMessageQueue.WriteForAllExcept(new Message() { Type = MessageType.NewPlayer, Data = newID }, NetChannel.ReliableUnordered, newID);

                _clientStateTracker.AddNewClient(newID);
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

                _clientStateTracker.AddNewClient(ID);
                _gameSession.Views.Add(_playerViewFactory(_clientStateTracker[ID].Player));
            }
            while (true)
            {
                object data = _incomingMessageQueue.ReadMessage(MessageType.SuccessfulJoin);
                if (data == null)
                    break;
                int ID = (int)data;
                _localClient.ID = ID;   // PlayerView & NetworkPlayerView already created when _localClient was initialized in GameSessionFactory...
            }
        }

        IGameSession _gameSession;
        IIncomingMessageQueue _incomingMessageQueue;
        IOutgoingMessageQueue _outgoingMessageQueue;
        IClientStateTracker _clientStateTracker;
        bool _isServer = false;
        PlayerView.Factory _playerViewFactory;
        LocalClient _localClient;
        NetworkPlayerProcessor _networkPlayerController;
    }
}
