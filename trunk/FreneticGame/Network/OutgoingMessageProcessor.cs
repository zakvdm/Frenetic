using System;
using System.Text;

namespace Frenetic
{
    public class OutgoingMessageProcessor : IOutgoingMessageProcessor
    {
        INetworkSession _networkSession;
        public OutgoingMessageProcessor(INetworkSession networkSession)
        {
            _networkSession = networkSession;
        }
        public string Process(string message)
        {
            return "hello";
        }
    }
}
