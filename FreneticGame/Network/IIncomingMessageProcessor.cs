using System;
using System.Text;

namespace Frenetic
{
    public interface IIncomingMessageProcessor
    {
        void Process(string message);
    }
}
