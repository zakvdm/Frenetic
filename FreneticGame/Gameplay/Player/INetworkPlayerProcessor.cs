using System;

namespace Frenetic.Player
{
    public interface INetworkPlayerProcessor
    {
        void UpdatePlayerFromNetworkItem(Frenetic.Network.Item item);
        void UpdatePlayerFromPlayerStateItem(Frenetic.Network.Item stateItem);
        void UpdatePlayerSettingsFromNetworkItem(Frenetic.Network.Item item);
    }
}
