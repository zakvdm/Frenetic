using System;

namespace Frenetic
{
    public interface INetworkPlayerProcessor
    {
        void UpdatePlayerFromNetworkMessage(Frenetic.Network.Message netMsg);
        void UpdatePlayerSettingsFromNetworkMessage(Frenetic.Network.Message netMsg);
    }
}
