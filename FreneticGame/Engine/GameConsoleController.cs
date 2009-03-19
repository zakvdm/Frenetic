using System;
using Microsoft.Xna.Framework.Input;
using Frenetic.UserInput;

namespace Frenetic
{
    class GameConsoleController : IController
    {
        public GameConsoleController(IGameConsole console, IKeyboard keyboard)
        {
            _console = console;
            _keyboard = keyboard;
        }

        #region IController Members

        public void Process(long ticks)
        {
            _keyboard.Unlock();

            if (_keyboard.IsKeyDown(Keys.OemTilde) && !_keyboard.WasKeyDown(Keys.OemTilde)  )
                _console.Active = !_console.Active;

            if (!_console.Active)
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
                        _console.CurrentInput += key.GetStringValue();
                    else
                        _console.CurrentInput += key.GetStringValue().ToLower();
                    
                }
            }

            // OTHER KEYS:
            if (_keyboard.IsKeyDown(Keys.Enter) && !_keyboard.WasKeyDown(Keys.Enter))
                _console.ProcessInput();

            if (_keyboard.IsKeyDown(Keys.Space) && !_keyboard.WasKeyDown(Keys.Space))
                _console.CurrentInput += " ";

            if (_keyboard.IsKeyDown(Keys.Back) && !_keyboard.WasKeyDown(Keys.Back) && (_console.CurrentInput.Length > 0))
                _console.CurrentInput = _console.CurrentInput.Remove(_console.CurrentInput.Length - 1);

            if (_keyboard.IsKeyDown(Keys.Tab) && !_keyboard.WasKeyDown(Keys.Tab))
                _console.TryToCompleteCurrentInput();

            // CLEANUP:
            _keyboard.SaveState();
            _keyboard.Lock();
        }

        #endregion

        IGameConsole _console;
        IKeyboard _keyboard;
    }
}
