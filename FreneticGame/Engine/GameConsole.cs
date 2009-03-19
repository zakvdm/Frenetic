using System;
using System.Collections.Generic;
using System.Linq;

namespace Frenetic
{
    public class GameConsole : IGameConsole
    {
        public GameConsole(IMediator mediator)
        {
            _mediator = mediator;

            Active = false;
            CommandLog = new List<string>();
            MessageLog = new List<string>();
            CurrentInput = "";
        }

        public void ProcessInput()
        {
            if ((CurrentInput.Length > 0) && CurrentInput.StartsWith("/"))
            {
                string commandLine = CurrentInput.Substring(1); // Remove the "/"
                string[] pieces = commandLine.Split(new char[]{' '}, StringSplitOptions.RemoveEmptyEntries);
                if (pieces.Length > 1)
                {
                    _mediator.Do(pieces[0], String.Join(" ", pieces, 1, pieces.Length - 1));
                }
                else
                {
                    _mediator.Get(pieces[0]);
                }
                CommandLog.Add(commandLine);
            }
            else
                MessageLog.Add(CurrentInput);

            CurrentInput = "";
        }

        public List<string> FindPossibleInputCompletions()
        {
            // Early outs:
            if (CurrentInput.Length == 0 || _mediator.AvailableCommands.Count == 0)
                return null;

            string searchString = CurrentInput.ToLower();

            if (searchString.StartsWith("/"))
                searchString = searchString.Substring(1);
            
            return (from command in _mediator.AvailableCommands
                        where (command.ToString().Length >= searchString.Length)
                        where (command.ToString().Substring(0, searchString.Length).ToLower() == searchString)
                        select command).ToList<string>();
        }

        public void TryToCompleteCurrentInput()
        {
            List<string> possibleCommands = FindPossibleInputCompletions();

            if (possibleCommands == null)
                return;

            if (possibleCommands.Count == 1)
            {
                CurrentInput = possibleCommands[0].ToString() + " ";
            }
            else if (possibleCommands.Count > 1)
            {
                string possibleCompletion = possibleCommands[0].ToString();
                int index = CurrentInput.Length;
                while (possibleCommands.TrueForAll(command => command.ToString()[index] == possibleCompletion[index]))
                {
                    index++;
                }
                CurrentInput = possibleCompletion.Substring(0, index);
                
            }
            
            // Once we try to complete, we can safely prepend "/" since this is intended to be a command
            if (!CurrentInput.StartsWith("/"))
                CurrentInput = "/" + CurrentInput;
        }

        public string CurrentInput { get; set; }
        public bool Active { get; set; }
        public List<string> CommandLog { get; set; }
        public List<string> MessageLog { get; set; }

        IMediator _mediator;
    }
}
