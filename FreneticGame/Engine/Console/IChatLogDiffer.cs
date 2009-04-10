using System;
using Frenetic.Network;

namespace Frenetic
{
    public interface IChatLogDiffer
    {
        MessageLog Diff(Client client);
    }
}
