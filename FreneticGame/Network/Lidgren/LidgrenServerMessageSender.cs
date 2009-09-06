using Lidgren.Network;
using Frenetic.Engine;
using log4net;

namespace Frenetic.Network.Lidgren
{
    public class LidgrenServerMessageSender : IServerMessageSender
    {
        public LidgrenServerMessageSender(INetServer netServer, ILoggerFactory loggerFactory)
        {
            _netServer = netServer;
            this.Logger = loggerFactory.GetLogger(this.GetType());
        }

        #region IServerMessageSender Members

        public void SendTo(Message msg, global::Lidgren.Network.NetChannel channel, INetConnection destinationConnection)
        {
            var buffer = _netServer.CreateBuffer();
            buffer.Write(msg);

            _netServer.SendMessage(buffer, channel, destinationConnection);
        }

        public void SendToAll(Message msg, global::Lidgren.Network.NetChannel channel)
        {
            var buffer = _netServer.CreateBuffer();
            buffer.Write(msg);

            _netServer.SendToAll(buffer, channel);

            this.Logger.Debug("Sent " + buffer.Data.Length + " bytes to " + _netServer.ConnectionCount + " Clients.");
        }

        public void SendToAllExcept(Message msg, global::Lidgren.Network.NetChannel channel, INetConnection excludedConnection)
        {
            var buffer = _netServer.CreateBuffer();
            buffer.Write(msg);

            _netServer.SendToAll(buffer, channel, excludedConnection);
        }

        #endregion

        INetServer _netServer;

        ILog Logger;
    }
}
