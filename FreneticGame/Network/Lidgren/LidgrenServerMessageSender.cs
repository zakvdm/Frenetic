using Lidgren.Network;
using Frenetic.Engine;
using log4net;

namespace Frenetic.Network.Lidgren
{
    public class LidgrenServerMessageSender : IServerMessageSender
    {
        public LidgrenServerMessageSender(INetServer netServer, IMessageSerializer messageSerializer, ILoggerFactory loggerFactory)
        {
            _netServer = netServer;
            _messageSerializer = messageSerializer;
            this.Logger = loggerFactory.GetLogger(this.GetType());
        }

        #region IServerMessageSender Members

        public void SendTo(Message msg, global::Lidgren.Network.NetChannel channel, INetConnection destinationConnection)
        {
            byte[] data = _messageSerializer.Serialize(msg);
            NetBuffer buffer = _netServer.CreateBuffer(data.Length);
            buffer.Write(data);

            _netServer.SendMessage(buffer, channel, destinationConnection);
        }

        public void SendToAll(Message msg, global::Lidgren.Network.NetChannel channel)
        {
            byte[] data = _messageSerializer.Serialize(msg);
            NetBuffer buffer = _netServer.CreateBuffer(data.Length);
            buffer.Write(data);

            _netServer.SendToAll(buffer, channel);

            this.Logger.Debug("Sent " + data.Length + " bytes to " + _netServer.ConnectionCount + " Clients.");
        }

        public void SendToAllExcept(Message msg, global::Lidgren.Network.NetChannel channel, INetConnection excludedConnection)
        {
            byte[] data = _messageSerializer.Serialize(msg);
            NetBuffer buffer = _netServer.CreateBuffer(data.Length);
            buffer.Write(data);

            _netServer.SendToAll(buffer, channel, excludedConnection);
        }

        #endregion

        INetServer _netServer;
        IMessageSerializer _messageSerializer;

        ILog Logger;
    }
}
