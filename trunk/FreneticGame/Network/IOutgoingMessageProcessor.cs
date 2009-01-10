using System;
using System.Text;

namespace Frenetic
{
    public interface IOutgoingMessageProcessor
    {
        string Process(string message);
    }
}
