using System;
using System.Xml.Serialization;
using System.IO;
using System.Collections.Generic;
using Frenetic.Network;

namespace Frenetic
{
    // Used by both the server and client to update network players
    public class NetworkPlayerProcessor : INetworkPlayerProcessor
    {
        public NetworkPlayerProcessor(IClientStateTracker clientStateTracker)
        {
            _clientStateTracker = clientStateTracker;
        }

        public void UpdatePlayerFromNetworkMessage(Message netMsg)
        {
            if (!_clientStateTracker.CurrentClients.Exists(client => client.ID == netMsg.ClientID))
                return; // Probably this is the local player which hasn't been added to the client state tracker...

            IPlayer player = _clientStateTracker[netMsg.ClientID].Player;
                
            // We can't just do a direct assignment (Player[player.ID] = player) here because we need the service objects (physics components, etc.) to remain in tact
            //      Unfortunately, this means that every time another property gets added to Player that needs to be network synced, it needs to be assigned here.
            //      It may very well be that there is a better solution here... after all, we really shouldn't be sending unnecessary info over the network...
            player.Position = ((Player)netMsg.Data).Position;
        }

        public void UpdatePlayerSettingsFromNetworkMessage(Message netMsg)
        {
            PlayerSettings settings = _clientStateTracker[netMsg.ClientID].PlayerSettings;

            settings.Name = ((PlayerSettings)netMsg.Data).Name;
        }

        IClientStateTracker _clientStateTracker;
    }
}
