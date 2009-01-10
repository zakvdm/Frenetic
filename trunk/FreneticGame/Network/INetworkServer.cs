using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Frenetic
{
    public interface INetworkServer
    {
        bool IsListening { get; }
        void Start();
        void Shutdown(string reason);
    }
}
