using System;
using System.Linq;
using System.Collections.Generic;

namespace Frenetic
{
    public class Mediator : IMediator
    {
        public Mediator()
        {
            _commands = new Dictionary<string, Func<string, string>>();
        }

        public List<string> AvailableCommands
        {
            get
            {
                return _commands.Keys.ToList();
            }
        }
        public void Register(string name, Func<string, string> command)
        {
            _commands.Add(name, command);
        }
        
        public string Get(string propertyName)
        {
            var command = GetCommand(propertyName);

            if (command == null)
                return null;

            // null argument is used to indicate a Property get
            return command(null);
        }
        public string Do(string commandName, string argument)
        {
            var command = GetCommand(commandName);

            if (command == null)
                return null;

            return command(argument);
        }

        private Func<string, string> GetCommand(string commandName)
        {
            if (!_commands.ContainsKey(commandName))
                return null;

            return _commands[commandName];
        }

        private Dictionary<string, Func<string, string>> _commands;
    }
}
