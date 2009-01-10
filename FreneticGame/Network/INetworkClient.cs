using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Frenetic
{
    public interface INetworkClient
    {
        void Start();
        void Connect(string IP, int port);
        void Shutdown(string reason);
    }
}
