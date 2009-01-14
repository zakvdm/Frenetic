using System;

namespace Frenetic
{
    public class NetworkPlayerView : IView
    {
        IPlayer _player;
        INetworkSession _networkSession;

        public NetworkPlayerView(IPlayer player, INetworkSession networkSession)
        {
            _player = player;
            _networkSession = networkSession;
        }

        public void Generate()
        {
            Message msg = new Message() { Type = MessageType.PlayerData, Data = _player };
            if (_networkSession.IsServer)
                _networkSession.SendToAll(msg, Lidgren.Network.NetChannel.Unreliable);
            else
                _networkSession.Send(msg, Lidgren.Network.NetChannel.Unreliable);
        }
    }
}
