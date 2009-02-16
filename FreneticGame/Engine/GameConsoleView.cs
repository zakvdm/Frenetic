using System;
using Frenetic.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;


namespace Frenetic
{
    public class GameConsoleView : IView
    {
        const float ALPHA = 0.8f;
        static Vector2 TEXT_OFFSET = new Vector2(20f, 20f);


        public GameConsoleView(IGameConsole console, Rectangle inputWindow, Rectangle commandWindow, Rectangle messageWindow, ISpriteBatch spriteBatch, ITexture texture, IFont font)
        {
            _console = console;
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
            if (_console.Active)
            {
                _spriteBatch.Begin();

                // CURRENT INPUT:
                DrawWindow(_inputWindow, Color.Black);
                _spriteBatch.DrawText(_font, CursorText + _console.CurrentInput, new Vector2(_inputWindow.Left + TEXT_OFFSET.X, _inputWindow.Bottom - TEXT_OFFSET.Y), Color.Yellow);

                // COMMAND WINDOW:
                DrawWindow(_commandWindow, Color.Black);
                DrawLog(_console.CommandLog, _commandWindow, Color.White);
                
                // MESSAGE WINDOW:
                if (_console.MessageLog != null)
                {
                    if (_console.MessageLog.Count > 0)
                    {
                        DrawWindow(_messageWindow, Color.DarkGray);
                        DrawLog(_console.MessageLog, _messageWindow, Color.Green);
                    }
                }

                // POSSIBLE COMMAND WINDOW:
                List<Command> possibleCommands = _console.FindPossibleInputCompletions();
                if ((possibleCommands != null) && possibleCommands.Count > 0 && possibleCommands.Count < 10)
                {
                    Rectangle possibleCommandWindow = new Rectangle(_commandWindow.Right + EdgeGap, (int)(_commandWindow.Bottom - 2 * TEXT_OFFSET.Y - (possibleCommands.Count * _font.LineSpacing)), _messageWindow.Width, (int)(2 * TEXT_OFFSET.Y + (possibleCommands.Count * _font.LineSpacing)));

                    DrawWindow(possibleCommandWindow, Color.White);
                    DrawLog(possibleCommands.ConvertAll<string>((input) => input.ToString()), possibleCommandWindow, Color.Black);
                }

                _spriteBatch.End();
            }
        }

        #endregion

        private void DrawWindow(Rectangle window, Color color)
        {
            _spriteBatch.Draw(_texture, window, new Color(color, ALPHA), 0f);
        }

        private void DrawLog(List<string> log, Rectangle window, Color color)
        {
            if (_font == null)
                return;

            if (log == null)
                return;

            if (log.Count == 0)
                return;

            Vector2 currentTextPosition = new Vector2(window.Left + TEXT_OFFSET.X, window.Bottom - TEXT_OFFSET.Y - (log.Count * _font.LineSpacing));
            foreach (string line in log)
            {
                _spriteBatch.DrawText(_font, line, currentTextPosition, color);
                currentTextPosition.Y += _font.LineSpacing;
            }
        }

        public string CursorText { get; set; }

        public int MiddleGap { get; private set; }
        public int EdgeGap { get; private set; }

        Rectangle _inputWindow, _commandWindow, _messageWindow;
        IGameConsole _console;
        ISpriteBatch _spriteBatch;
        ITexture _texture;
        IFont _font;
    }
}
