using System;
using System.Xml.Serialization;
using System.IO;
using System.Collections.Generic;
using Frenetic.Network;

namespace Frenetic.Player
{
    // Used by both the server and client to update network players
    public class NetworkPlayerProcessor : INetworkPlayerProcessor
    {
        public NetworkPlayerProcessor(IClientStateTracker clientStateTracker)
        {
            _clientStateTracker = clientStateTracker;
        }

        public void UpdatePlayerFromNetworkItem(Item item)
        {
            if (!IsValidClient(item.ClientID))
                return;

            IPlayer player = _clientStateTracker.FindNetworkClient(item.ClientID).Player;
                
            // We can't just do a direct assignment (Player[player.ID] = player) here because we need the service objects (physics components, etc.) to remain in tact
            //      Unfortunately, this means that every time another property gets added to Player that needs to be network synced, it needs to be assigned here.
            //      It may very well be that there is a better solution here... after all, we really shouldn't be sending unnecessary info over the network...
            var receivedPlayer = (IPlayer)item.Data;
            player.Position = receivedPlayer.Position;
            player.PendingShot = receivedPlayer.PendingShot;
        }
        public void UpdatePlayerFromPlayerStateItem(Item stateItem)
        {
            Client client;
            PlayerType playerType;

            if (_clientStateTracker.LocalClient.ID == stateItem.ClientID)
            {
                client = _clientStateTracker.LocalClient;
                playerType = PlayerType.Local;
            }
            else
            {
                if (!IsValidClient(stateItem.ClientID))
                    return;

                client = _clientStateTracker.FindNetworkClient(stateItem.ClientID);
                playerType = PlayerType.Network;
            }

            var playerState = (IPlayerState)stateItem.Data;
            playerState.RefreshPlayerValuesFromState(client.Player, playerType);
        }

        public void UpdatePlayerSettingsFromNetworkItem(Item item)
        {
            if (!IsValidClient(item.ClientID))
                return;

            IPlayerSettings settings = _clientStateTracker.FindNetworkClient(item.ClientID).Player.PlayerSettings;
            settings.Name = ((IPlayerSettings)item.Data).Name;
        }

        bool IsValidClient(int clientID)
        {
            // We don't care about Clients who aren't currently connected...
            return (_clientStateTracker.FindNetworkClient(clientID) != null);
        }

        IClientStateTracker _clientStateTracker;
    }
}
