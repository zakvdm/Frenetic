using System;
using Microsoft.Xna.Framework.Input;
using Frenetic.UserInput;
using Frenetic.Engine.Overlay;

namespace Frenetic.Engine.Overlay
{
    public class ConsoleController : IController
    {
        public ConsoleController(InputLine inputLine, ICommandConsole commandConsole, IMessageConsole messageConsole, 
            IOverlayView inputView, IOverlayView commandConsoleView, IOverlayView messageConsoleView, IOverlayView possibleCommandsView, IKeyboard keyboard)
        {
            this.InputLine = inputLine;
            _commandConsole = commandConsole;
            _messageConsole = messageConsole;
            _inputView = inputView;
            _commandConsoleView = commandConsoleView;
            _messageConsoleView = messageConsoleView;
            _possibleCommandsView = possibleCommandsView;
            _keyboard = keyboard;

            this.InputLine.CurrentInput = "";
        }

        #region IController Members

        public void Process(float elapsedSeconds)
        {
            _keyboard.Unlock();

            if (_keyboard.IsKeyDown(Keys.OemTilde) && !_keyboard.WasKeyDown(Keys.OemTilde))
            {
                bool newState = !_commandConsole.Active;
                _commandConsole.Active = newState;
                _messageConsole.Active = newState;
                _inputView.Visible = newState;
                _commandConsoleView.Visible = newState;
                _messageConsoleView.Visible = newState;
                _possibleCommandsView.Visible = newState;
            }

            if (!_commandConsole.Active && !_messageConsole.Active)
            {
                return;
            }

            // READ TEXT INPUT:
            foreach (Keys key in XnaKeyboard.AlphaNumericKeys)  // TODO: Is there a way to make this more general? (no XNAKeyboard reference?)
            {
                if (_keyboard.IsKeyDown(key) && !_keyboard.WasKeyDown(key))
                {
                    // Check if it's upper or lower case
                    if (_keyboard.IsKeyDown(Keys.LeftShift) || _keyboard.IsKeyDown(Keys.RightShift))
                        this.InputLine.CurrentInput += key.GetStringValue();
                    else
                        this.InputLine.CurrentInput += key.GetStringValue().ToLower();
                    
                }
            }

            // OTHER KEYS:
            if (_keyboard.IsKeyDown(Keys.Space) && !_keyboard.WasKeyDown(Keys.Space))
                this.InputLine.CurrentInput += " ";

            if (_keyboard.IsKeyDown(Keys.Back) && !_keyboard.WasKeyDown(Keys.Back) && (this.InputLine.CurrentInput.Length > 0))
                this.InputLine.CurrentInput = this.InputLine.CurrentInput.Remove(this.InputLine.CurrentInput.Length - 1);

            if (_keyboard.IsKeyDown(Keys.Tab) && !_keyboard.WasKeyDown(Keys.Tab))
                this.InputLine.CurrentInput = _commandConsole.TryToCompleteInput(this.InputLine.CurrentInput);

            if (_keyboard.IsKeyDown(Keys.Enter) && !_keyboard.WasKeyDown(Keys.Enter))   // ENTER
            {
                if (this.InputLine.CurrentInput.StartsWith("/"))
                {
                    _commandConsole.ProcessInput(this.InputLine.CurrentInput);
                }
                else
                {
                    // NOTE: We don't set ClientName or ServerSnap on the ChatMessage because this will have been set by the server when we get the message back...
                    _messageConsole.ProcessInput(this.InputLine.CurrentInput);
                }
                this.InputLine.CurrentInput = "";
            }

            // CLEANUP:
            _keyboard.Lock();
        }

        #endregion

        InputLine InputLine;
        ICommandConsole _commandConsole;
        IMessageConsole _messageConsole;
        IOverlayView _inputView;
        IOverlayView _commandConsoleView;
        IOverlayView _messageConsoleView;
        IOverlayView _possibleCommandsView;
        IKeyboard _keyboard;

    }
}
