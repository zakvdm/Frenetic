#region File Description
//-----------------------------------------------------------------------------
// MessageBoxScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
#endregion

namespace Frenetic
{
    /// <summary>
    /// A popup message box screen, used to display "are you sure?"
    /// confirmation messages.
    /// </summary>
    /// <remarks>
    /// This public class is somewhat similar to one of the same name in the 
    /// GameStateManagement sample.
    /// </remarks>
    public class MessageBoxScreen : GameScreen
    {
        #region Constants
        const string usageText = "A button = Okay\n" +
                                 "B button = Cancel";
        #endregion

        #region Fields

        bool pauseMenu = false;
        string message;
        SpriteFont _smallFont;
        SpriteFont _font;
        Viewport _viewport;
        SpriteBatch _spriteBatch;
        Texture2D _blankTexture;

        #endregion

        #region Events

        public event EventHandler<EventArgs> Accepted;
        public event EventHandler<EventArgs> Cancelled;

        #endregion

        #region Initialization


        /// <summary>
        /// Constructor.
        /// </summary>
        public MessageBoxScreen(string message, bool pauseMenu, SpriteFont smallFont, SpriteFont font, Viewport viewport, SpriteBatch spriteBatch, Texture2D blankTexture)
        {
            this.message = message;
            _smallFont = smallFont;
            _font = font;
            _viewport = viewport;
            _spriteBatch = spriteBatch;
            _blankTexture = blankTexture;

            IsPopup = true;
            this.pauseMenu = pauseMenu;

            TransitionOnTime = TimeSpan.FromSeconds(0.25);
            TransitionOffTime = TimeSpan.FromSeconds(0.25);
        }

        #endregion

        #region Handle Input


        /// <summary>
        /// Responds to user input, accepting or cancelling the message box.
        /// </summary>
        public override void HandleInput(InputState input)
        {
            if (input.MenuSelect && (!pauseMenu ||
                (input.CurrentGamePadState.Buttons.A == ButtonState.Pressed)))
            {
                // Raise the accepted event, then exit the message box.
                if (Accepted != null)
                    Accepted(this, EventArgs.Empty);

                ExitScreen();
            }
            else if (input.MenuCancel || (input.MenuSelect && pauseMenu &&
                (input.CurrentGamePadState.Buttons.A == ButtonState.Released)))
            {
                // Raise the cancelled event, then exit the message box.
                if (Cancelled != null)
                    Cancelled(this, EventArgs.Empty);

                ExitScreen();
            }
        }


        #endregion

        #region Draw


        /// <summary>
        /// Draws the message box.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            // Darken down any other screens that were drawn beneath the popup.
            FadeBackBufferToBlack(TransitionAlpha * 2 / 3);

            // Center the message text in the viewport.
            Vector2 viewportSize = new Vector2(_viewport.Width, _viewport.Height);
            Vector2 textSize = _font.MeasureString(message);
            Vector2 textPosition = (viewportSize - textSize) / 2;
            Vector2 usageTextSize = _smallFont.MeasureString(usageText);
            Vector2 usageTextPosition = (viewportSize - usageTextSize) / 2;
            usageTextPosition.Y = textPosition.Y +
                _font.LineSpacing * 1.1f;

            // Fade the popup alpha during transitions.
            Color color = new Color(255, 255, 255, TransitionAlpha);

            // Draw the background rectangles
            Rectangle rect = new Rectangle(
                (int)(Math.Min(usageTextPosition.X, textPosition.X)),
                (int)(textPosition.Y),
                (int)(Math.Max(usageTextSize.X, textSize.X)),
                (int)(_font.LineSpacing * 1.1f + usageTextSize.Y)
                );
            rect.X -= (int)(0.1f * rect.Width);
            rect.Y -= (int)(0.1f * rect.Height);
            rect.Width += (int)(0.2f * rect.Width);
            rect.Height += (int)(0.2f * rect.Height);

            Rectangle rect2 = new Rectangle(rect.X - 1, rect.Y - 1,
                rect.Width + 2, rect.Height + 2);
            _spriteBatch.Begin();
            _spriteBatch.Draw(_blankTexture, rect2, new Color(128, 128, 128,
                (byte)(192.0f * (float)TransitionAlpha / 255.0f)));
            _spriteBatch.Draw(_blankTexture, rect, new Color(0, 0, 0,
                (byte)(232.0f * (float)TransitionAlpha / 255.0f)));
            _spriteBatch.End();

            // Draw the message box text.
            _spriteBatch.Begin();
            _spriteBatch.DrawString(_font, message,
                                                 textPosition, color);
            _spriteBatch.DrawString(_smallFont, usageText,
                                                 usageTextPosition, color);
            _spriteBatch.End();
        }

        /// <summary>
        /// Helper draws a translucent black fullscreen sprite, used for fading
        /// screens in and out, and for darkening the background behind popups.
        /// </summary>
        private void FadeBackBufferToBlack(int alpha)
        {
            _spriteBatch.Begin();

            _spriteBatch.Draw(_blankTexture,
                             new Rectangle(0, 0, _viewport.Width, _viewport.Height),
                             new Color(0, 0, 0, (byte)alpha));

            _spriteBatch.End();
        }


        #endregion
    }
}
