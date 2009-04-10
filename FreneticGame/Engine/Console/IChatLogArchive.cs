using System;
using Frenetic.Network;
namespace Frenetic
{
    public interface IChatLogArchive : IController
    {
        MessageLog this[Client client] { get; }
    }
}
