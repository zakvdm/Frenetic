using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Frenetic
{
    public class Command
    {
        public Command(string commandText)
        {
            _commandText = commandText;
        }

        public string ToString()
        {
            return _commandText;
        }

        string _commandText;
    }
}
