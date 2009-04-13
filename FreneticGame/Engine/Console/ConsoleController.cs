using System;
using Microsoft.Xna.Framework.Input;
using Frenetic.UserInput;

namespace Frenetic
{
    public class ConsoleController : IController
    {
        public ConsoleController(ICommandConsole commandConsole, IMessageConsole messageConsole, IKeyboard keyboard)
        {
            _commandConsole = commandConsole;
            _messageConsole = messageConsole;
            _keyboard = keyboard;

            CurrentInput = "";
        }

        #region IController Members

        public void Process(float elapsedTime)
        {
            _keyboard.Unlock();

            if (_keyboard.IsKeyDown(Keys.OemTilde) && !_keyboard.WasKeyDown(Keys.OemTilde))
            {
                _commandConsole.Active = !_commandConsole.Active;
                _messageConsole.Active = !_messageConsole.Active;
            }

            if (!_commandConsole.Active && !_messageConsole.Active)
            {
                _keyboard.SaveState();  // Save state so that we can't toggle off and immediately back on...
                return;
            }

            // READ TEXT INPUT:
            foreach (Keys key in XnaKeyboard.AlphaNumericKeys)  // TODO: Is there a way to make this more general? (no XNAKeyboard reference?)
            {
                if (_keyboard.IsKeyDown(key) && !_keyboard.WasKeyDown(key))
                {
                    // Check if it's upper or lower case
                    if (_keyboard.IsKeyDown(Keys.LeftShift) || _keyboard.IsKeyDown(Keys.RightShift))
                        CurrentInput += key.GetStringValue();
                    else
                        CurrentInput += key.GetStringValue().ToLower();
                    
                }
            }

            // OTHER KEYS:
            if (_keyboard.IsKeyDown(Keys.Space) && !_keyboard.WasKeyDown(Keys.Space))
                CurrentInput += " ";

            if (_keyboard.IsKeyDown(Keys.Back) && !_keyboard.WasKeyDown(Keys.Back) && (CurrentInput.Length > 0))
                CurrentInput = CurrentInput.Remove(CurrentInput.Length - 1);

            if (_keyboard.IsKeyDown(Keys.Tab) && !_keyboard.WasKeyDown(Keys.Tab))
                CurrentInput = _commandConsole.TryToCompleteInput(CurrentInput);

            if (_keyboard.IsKeyDown(Keys.Enter) && !_keyboard.WasKeyDown(Keys.Enter))   // ENTER
            {
                if (CurrentInput.StartsWith("/"))
                {
                    _commandConsole.ProcessInput(CurrentInput);
                }
                else
                {
                    // NOTE: We don't set ClientName or ServerSnap on the ChatMessage because this will have been set by the server when we get the message back...
                    _messageConsole.ProcessInput(CurrentInput);
                }
                CurrentInput = "";
            }



            // CLEANUP:
            _keyboard.SaveState();
            _keyboard.Lock();
        }

        #endregion

        public string CurrentInput { get; set; }

        ICommandConsole _commandConsole;
        IMessageConsole _messageConsole;
        IKeyboard _keyboard;

    }
}
