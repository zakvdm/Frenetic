using System;
using System.Collections.Generic;
using System.Linq;
using Frenetic.UserInput;

namespace Frenetic
{
    public class CommandConsole : ICommandConsole
    {
        public CommandConsole(IMediator mediator, Log<string> commandLog)
        {
            _mediator = mediator;

            Active = false;
            Log = commandLog;
        }

        public void ProcessInput(string input)
        {
            if ((input.Length > 0) && input.StartsWith("/"))
            {
                string commandLine = input.Substring(1); // Remove the "/"
                string[] pieces = commandLine.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (pieces.Length > 1)
                {
                    _mediator.Set(pieces[0], String.Join(" ", pieces, 1, pieces.Length - 1));
                }
                else
                {
                    _mediator.Get(pieces[0]);
                }
                Log.AddMessage(commandLine);
            }
        }

        public Log<string> FindPossibleInputCompletions(string input)
        {
            // Early outs:
            if (input.Length == 0 || _mediator.AvailableProperties.Count == 0)
                return null;

            string searchString = input.ToLower();

            if (searchString.StartsWith("/"))
                searchString = searchString.Substring(1);
            
            return new Log<string>((from command in _mediator.AvailableProperties
                        where (command.ToString().Length >= searchString.Length)
                        where (command.ToString().Substring(0, searchString.Length).ToLower() == searchString)
                        select command).ToList<string>());
        }

        public string TryToCompleteInput(string input)
        {
            var possibleCommands = FindPossibleInputCompletions(input);

            if (possibleCommands == null)
                return input;

            if (possibleCommands.Count == 1)
            {
                input = possibleCommands[0].ToString() + " ";
            }
            else if (possibleCommands.Count > 1)
            {
                string possibleCompletion = possibleCommands[0].ToString();
                int index = input.Length;
                while (possibleCommands.TrueForAll(command => command.ToString()[index] == possibleCompletion[index]))
                {
                    index++;
                }
                input = possibleCompletion.Substring(0, index);
                
            }
            
            // Once we try to complete, we can safely prepend "/" since this is intended to be a command
            if (!input.StartsWith("/"))
                input = "/" + input;

            return input;
        }

        public bool Active { get; set; }
        public Log<string> Log { get; set; }

        IMediator _mediator;
    }
}
