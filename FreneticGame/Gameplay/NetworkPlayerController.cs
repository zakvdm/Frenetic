﻿using System;
using System.Xml.Serialization;
using System.IO;
using System.Collections.Generic;

namespace Frenetic
{
    public class NetworkPlayerController : IController
    {
        MessageQueue _messageQueue;
        public Dictionary<int, IPlayer> Players { get; private set; }
        public NetworkPlayerController(MessageQueue messageQueue)
        {
            Players = new Dictionary<int, IPlayer>();
            _messageQueue = messageQueue;
        }

        public void Process(long ticks)
        {
            while (true)
            {
                Player player = (Player)_messageQueue.ReadMessage(MessageType.PlayerData);
                
                if (player == null)
                    return;

                if (!Players.ContainsKey(player.ID))
                    continue;

                Players[player.ID].Position = player.Position;
            }
        }
    }
}
