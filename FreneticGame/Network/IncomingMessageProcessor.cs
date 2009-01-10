using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Frenetic
{
    public class IncomingMessageProcessor : IIncomingMessageProcessor
    {
        #region IIncomingMessageProcessor Members

        public void Process(string message)
        {
            Console.WriteLine(message);
        }

        #endregion
    }
}
