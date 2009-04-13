using System;
using Frenetic.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;


namespace Frenetic
{
    public class ConsoleView : IView
    {
        const float ALPHA = 0.8f;
        static Vector2 TEXT_OFFSET = new Vector2(20f, 20f);


        public ConsoleView(ICommandConsole commandConsole, IMessageConsole messageConsole, ConsoleController consoleController, Rectangle inputWindow, Rectangle commandWindow, Rectangle messageWindow, ISpriteBatch spriteBatch, ITexture texture, IFont font)
        {
            _commandConsole = commandConsole;
            _messageConsole = messageConsole;
            _consoleController = consoleController;
            _inputWindow = inputWindow;
            _commandWindow = commandWindow;
            _messageWindow = messageWindow;
            _spriteBatch = spriteBatch;
            _texture = texture;
            _font = font;

            CursorText = "Frenetic 0.1 > ";
            MiddleGap = 30;
            EdgeGap = 10;
        }

        #region IView Members

        public void Generate()
        {
            if (_commandConsole.Active)
            {
                _spriteBatch.Begin();

                // CURRENT INPUT:
                DrawWindow(_inputWindow, Color.Black);
                _spriteBatch.DrawText(_font, CursorText + _consoleController.CurrentInput, new Vector2(_inputWindow.Left + TEXT_OFFSET.X, _inputWindow.Bottom - TEXT_OFFSET.Y), Color.Yellow);

                // COMMAND WINDOW:
                DrawWindow(_commandWindow, Color.Black);
                DrawLog(_commandConsole.Log, _commandWindow, Color.White);
                
                // POSSIBLE COMMAND WINDOW:
                var possibleCommands = _commandConsole.FindPossibleInputCompletions(_consoleController.CurrentInput);
                if ((possibleCommands != null) && possibleCommands.Count > 0 && possibleCommands.Count < 10)
                {
                    Rectangle possibleCommandWindow = new Rectangle(_commandWindow.Right + EdgeGap, (int)(_commandWindow.Bottom - 2 * TEXT_OFFSET.Y - (possibleCommands.Count * _font.LineSpacing)), _messageWindow.Width, (int)(2 * TEXT_OFFSET.Y + (possibleCommands.Count * _font.LineSpacing)));

                    DrawWindow(possibleCommandWindow, Color.White);
                    DrawLog(possibleCommands, possibleCommandWindow, Color.Black);
                }

                _spriteBatch.End();
            }

            if (_messageConsole.Active)
            {
                _spriteBatch.Begin();
                // MESSAGE WINDOW:
                if (_messageConsole.Log != null)
                {
                    if (_messageConsole.Log.Count > 0)
                    {
                        DrawWindow(_messageWindow, Color.DarkGray);
                        DrawLog(_messageConsole.Log, _messageWindow, Color.Green);
                    }
                }
                _spriteBatch.End();
            }
        }

        #endregion

        private void DrawWindow(Rectangle window, Color color)
        {
            _spriteBatch.Draw(_texture, window, new Color(color, ALPHA), 0f);
        }


        private void DrawLog<T>(Log<T> log, Rectangle window, Color color)
        {
            if (_font == null)
                return;

            if (log == null)
                return;

            if (log.Count == 0)
                return;

            Vector2 currentTextPosition = new Vector2(window.Left + TEXT_OFFSET.X, window.Bottom - TEXT_OFFSET.Y - _font.LineSpacing);
            foreach (T line in log) // Read the log contents from newest message to oldest
            {
                _spriteBatch.DrawText(_font, line.ToString(), currentTextPosition, color);
                currentTextPosition.Y -= _font.LineSpacing;
            }
        }

        public string CursorText { get; set; }

        public int MiddleGap { get; private set; }
        public int EdgeGap { get; private set; }

        Rectangle _inputWindow, _commandWindow, _messageWindow;
        ICommandConsole _commandConsole;
        IMessageConsole _messageConsole;
        ConsoleController _consoleController;
        ISpriteBatch _spriteBatch;
        ITexture _texture;
        IFont _font;
    }
}
