using System;
using Frenetic.Network;

namespace Frenetic
{
    public class NetworkPlayerView : IView
    {
        
        public NetworkPlayerView(IPlayer player, IOutgoingMessageQueue outgoingMessageQueue)
        {
            _player = player;
            _outgoingMessageQueue = outgoingMessageQueue;
        }

        public void Generate()
        {
            if (_player.ID == 0)
                return;

            Message msg = new Message() { Type = MessageType.PlayerData, Data = _player };
            _outgoingMessageQueue.Write(msg, Lidgren.Network.NetChannel.Unreliable);
        }

        IPlayer _player;
        IOutgoingMessageQueue _outgoingMessageQueue;
    }
}
