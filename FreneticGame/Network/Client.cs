using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Frenetic
{
    public class Client
    {
        private INetworkClient _networkClient;
        public Client(INetworkClient networkClient)
        {
            _networkClient = networkClient;
        }

        public void Connect(string IP, int port)
        {
            _networkClient.Start();
            _networkClient.Connect(IP, port);
        }
    }
}
