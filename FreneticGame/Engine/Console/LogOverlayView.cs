using System;
using Microsoft.Xna.Framework;
using Frenetic.Graphics;
using Microsoft.Xna.Framework.Graphics;

namespace Frenetic.Engine.Overlay
{
    public class LogOverlayView<T> : IOverlayView
    {
        public LogOverlayView(IConsole<T> console, Rectangle logWindow, IFont font, Color textColor)
        {
            _console = console;
            this.Window = logWindow;
            _font = font;
            _textColor = textColor;

            this.BackgroundColor = OverlaySetView.BACKGROUND_COLOR;
        }
        #region IHudView Members

        /// <summary>
        /// NOTE: This will only return true if it has been set to true AND there are items in the Log (might be a bit confusing, but I'm trying it anyway!)
        /// </summary>
        public virtual bool Visible 
        {
            get
            {
                return (_internal_visibility_flag && _console.Log.Count > 0);
            }
            set
            {
                _internal_visibility_flag = value;
            }
        }
        public virtual Rectangle Window { get; set; }
        public Color BackgroundColor { get; set; }

        public virtual void Draw(ISpriteBatch spritebatch)
        {
            DrawLog<T>(_console.Log, this.Window, spritebatch);
        }

        #endregion

        protected void DrawLog<LogType>(Log<LogType> log, Rectangle window, ISpriteBatch spritebatch)
        {
            if (_font == null)
                return;

            if (log == null)
                return;

            if (log.Count == 0)
                return;

            Vector2 currentTextPosition = new Vector2(window.Left + OverlaySetView.TEXT_OFFSET.X, window.Bottom - OverlaySetView.TEXT_OFFSET.Y - _font.LineSpacing);
            foreach (LogType line in log) // Read the log contents from newest message to oldest
            {
                spritebatch.DrawText(_font, line.ToString(), currentTextPosition, _textColor);
                currentTextPosition.Y -= _font.LineSpacing;
            }
        }

        IConsole<T> _console;
        protected IFont _font;
        protected Color _textColor;

        protected bool _internal_visibility_flag = false;
    }

    public class PossibleCommandsLogHudView : LogOverlayView<string>
    {
        public PossibleCommandsLogHudView(InputLine inputLine, ICommandConsole commandConsole, Rectangle templateWindow, IFont font, Color textColor)
            : base(null, Rectangle.Empty, font, textColor)
        {
            _inputLine = inputLine;
            _commandConsole = commandConsole;
            _templateWindow = templateWindow;

            base.BackgroundColor = Color.Black;
        }

        public override void Draw(ISpriteBatch spritebatch)
        {
            var possibleCommands = FindPossibleCommands();
            DrawLog<string>(possibleCommands, this.Window, spritebatch);
        }

        /// <summary>
        /// NOTE: The Possible Commands window is only visible when there are more than 0 possible completions...
        /// </summary>
        public override bool Visible
        {
            get
            {
                var possibleCommands = FindPossibleCommands();
                if ((possibleCommands != null) && possibleCommands.Count > 0)
                    return _internal_visibility_flag;

                return false;
            }
            set
            {
                _internal_visibility_flag = value;
            }
        }
        /// <summary>
        /// Returns a Window sized to hold the possible completions (based on the dimensions of the Window passed into the constructor)
        /// </summary>
        public override Rectangle Window
        {
            get
            {
                var possibleCommands = FindPossibleCommands();
                return new Rectangle(_templateWindow.Left, (int)(_templateWindow.Bottom - 2 * OverlaySetView.TEXT_OFFSET.Y - (possibleCommands.Count * _font.LineSpacing)), _templateWindow.Width, (int)(2 * OverlaySetView.TEXT_OFFSET.Y + (possibleCommands.Count * _font.LineSpacing)));
            }
        }

        Log<string> FindPossibleCommands()
        {
            return _commandConsole.FindPossibleInputCompletions(_inputLine.CurrentInput);
        }

        ICommandConsole _commandConsole;
        InputLine _inputLine;
        Rectangle _templateWindow;
    }
}
