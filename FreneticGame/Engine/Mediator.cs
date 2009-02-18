using System;
using System.Linq;
using System.Collections.Generic;

namespace Frenetic
{
    public class Mediator
    {
        public Mediator()
        {
            Commands = new Dictionary<string, Func<string, string>>();
        }

        public List<string> AvailableCommands
        {
            get
            {
                return Commands.Keys.ToList();
            }
        }

        public string Get(string propertyName)
        {
            if (!Commands.ContainsKey(propertyName))
                return null;

            return Commands[propertyName](null);
        }
        public string Do(string commandName, string argument)
        {
            if (!Commands.ContainsKey(commandName))
                return null;

            return Commands[commandName](argument);
        }

        public Dictionary<string, Func<string, string>> Commands { get; private set; }
    }
}
