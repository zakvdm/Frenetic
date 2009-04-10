using System;
using System.Xml.Serialization;
using System.IO;
using System.Collections.Generic;
using Frenetic.Network;

namespace Frenetic
{
    // Used by both the server and client to update network players
    public class NetworkPlayerController : IController
    {
        public NetworkPlayerController(IIncomingMessageQueue incomingMessageQueue)
        {
            Players = new Dictionary<int, IPlayer>();
            _incomingMessageQueue = incomingMessageQueue;
        }

        public void Process(float elapsedTime)
        {
            while (true)
            {
                Player player = (Player)_incomingMessageQueue.ReadMessage(MessageType.PlayerData);
                
                if (player == null)
                    return;

                if (!Players.ContainsKey(player.ID))
                    continue;

                // We can't just do a direct assignment (Player[player.ID] = player) here because we need the service objects (physics components, etc.) to remain in tact
                //      Unfortunately, this means that every time another property gets added to Player that needs to be network synced, it needs to be assigned here.
                //      It may very well be that there is a better solution here... after all, we really shouldn't be sending unnecessary info over the network...
                Players[player.ID].Position = player.Position;
                Players[player.ID].Settings = player.Settings;
            }
        }

        IIncomingMessageQueue _incomingMessageQueue;
        public Dictionary<int, IPlayer> Players { get; private set; }
    }
}
