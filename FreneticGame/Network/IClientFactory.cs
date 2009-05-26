using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Frenetic.Network
{
    public interface IClientFactory
    {
        Client MakeNewClient(int ID);
        LocalClient GetLocalClient();
    }
}
