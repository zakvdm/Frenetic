using System;
using System.Xml.Serialization;
using System.IO;
using System.Collections.Generic;
using Frenetic.Network;

namespace Frenetic
{
    public class NetworkPlayerController : IController
    {
        public NetworkPlayerController(IIncomingMessageQueue incomingMessageQueue)
        {
            Players = new Dictionary<int, IPlayer>();
            _incomingMessageQueue = incomingMessageQueue;
        }

        public void Process(long ticks)
        {
            while (true)
            {
                Player player = (Player)_incomingMessageQueue.ReadMessage(MessageType.PlayerData);
                
                if (player == null)
                    return;

                if (!Players.ContainsKey(player.ID))
                    continue;

                Players[player.ID].Position = player.Position;
            }
        }

        IIncomingMessageQueue _incomingMessageQueue;
        public Dictionary<int, IPlayer> Players { get; private set; }
    }
}
