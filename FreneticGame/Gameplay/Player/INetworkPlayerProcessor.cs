using System;

namespace Frenetic.Player
{
    public interface INetworkPlayerProcessor
    {
        void UpdatePlayerFromNetworkMessage(Frenetic.Network.Message netMsg);
        void UpdatePlayerSettingsFromNetworkMessage(Frenetic.Network.Message netMsg);
    }
}
