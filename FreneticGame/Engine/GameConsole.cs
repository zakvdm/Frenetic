using System;
using System.Collections.Generic;
using System.Linq;

namespace Frenetic
{
    public class GameConsole : IGameConsole
    {
        public GameConsole(List<Command> commandList)
        {
            _commandList = commandList;

            Active = false;
            CommandLog = new List<string>();
            MessageLog = new List<string>();
            CurrentInput = "";
        }

        public void ProcessInput()
        {
            if ((CurrentInput.Length > 0) && CurrentInput.StartsWith("/"))
            {
                CommandLog.Add(CurrentInput.Substring(1)); // Remove the "/"
            }
            else
                MessageLog.Add(CurrentInput);

            CurrentInput = "";
        }

        public List<Command> FindPossibleInputCompletions()
        {
            // Early outs:
            if (CurrentInput.Length == 0 || _commandList == null || _commandList.Count == 0)
                return null;

            string searchString = CurrentInput.ToLower();

            if (searchString.StartsWith("/"))
                searchString = searchString.Substring(1);
            
            return (from command in _commandList
                        where (command.ToString().Length >= searchString.Length)
                        where (command.ToString().Substring(0, searchString.Length).ToLower() == searchString)
                        select command).ToList<Command>();
        }

        public void TryToCompleteCurrentInput()
        {
            List<Command> possibleCommands = FindPossibleInputCompletions();

            if (possibleCommands == null)
                return;

            if (possibleCommands.Count == 1)
            {
                CurrentInput = possibleCommands[0].ToString();
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

        List<Command> _commandList;
    }
}
