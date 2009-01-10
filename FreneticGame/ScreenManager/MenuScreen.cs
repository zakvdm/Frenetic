using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Frenetic
{
    abstract public class MenuScreen : GameScreen
    {
        #region Fields
        List<string> menuEntries = new List<string>();
        int selectedEntry = 0;

        Viewport _viewport;
        SpriteBatch _spriteBatch;
        SpriteFont _spriteFont;
        #endregion

        #region Properties


        /// <summary>
        /// Gets the list of menu entry strings, so derived classes can add
        /// or change the menu contents.
        /// </summary>
        protected IList<string> MenuEntries
        {
            get { return menuEntries; }
        }

        #endregion

        #region Initialization
        public MenuScreen(Viewport viewport, SpriteBatch spriteBatch, SpriteFont font)
        {
            _viewport = viewport;
            _spriteBatch = spriteBatch;
            _spriteFont = font;

            TransitionOnTime = TimeSpan.FromSeconds(1.0);
            TransitionOffTime = TimeSpan.FromSeconds(1.0);
        }

        #endregion


        #region Handle Input


        /// <summary>
        /// Responds to user input, changing the selected entry and accepting
        /// or cancelling the menu.
        /// </summary>
        public override void HandleInput(IInputState input)
        {
            // Move to the previous menu entry?
            if (input.MenuUp)
            {
                selectedEntry--;

                if (selectedEntry < 0)
                    selectedEntry = menuEntries.Count - 1;

                //AudioManager.PlayCue("menuMove");
            }

            // Move to the next menu entry?
            if (input.MenuDown)
            {
                selectedEntry++;

                if (selectedEntry >= menuEntries.Count)
                    selectedEntry = 0;

                //AudioManager.PlayCue("menuMove");
            }

            // Accept or cancel the menu?
            if (input.MenuSelect)
            {
                //AudioManager.PlayCue("menuSelect");
                OnSelectEntry(selectedEntry);
            }
            else if (input.MenuCancel)
            {
                OnCancel();
            }
        }


        /// <summary>
        /// Notifies derived classes that a menu entry has been chosen.
        /// </summary>
        protected abstract void OnSelectEntry(int entryIndex);


        /// <summary>
        /// Notifies derived classes that the menu has been cancelled.
        /// </summary>
        protected abstract void OnCancel();


        #endregion

        #region Draw


        /// <summary>
        /// Draws the menu.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            Vector2 viewportSize = new Vector2(_viewport.Width, _viewport.Height);

            Vector2 position = new Vector2(0f, viewportSize.Y * 0.65f);

            // Make the menu slide into place during transitions, using a
            // power curve to make things look more interesting (this makes
            // the movement slow down as it nears the end).
            float transitionOffset = (float)Math.Pow(TransitionPosition, 2);

            if (ScreenState == ScreenState.TransitionOn)
                position.Y += transitionOffset * 256;
            else
                position.Y += transitionOffset * 512;

            // Draw each menu entry in turn.
            _spriteBatch.Begin();

            for (int i = 0; i < menuEntries.Count; i++)
            {
                Color color = Color.White;
                float scale = 1.0f;

                if (IsActive && (i == selectedEntry))
                {
                    // The selected entry is yellow, and has an animating size.
                    double time = gameTime.TotalGameTime.TotalSeconds;
                    float pulsate = (float)Math.Sin(time * 6f) + 1f;

                    color = Color.Orange;
                    scale += pulsate * 0.05f;
                }

                // Modify the alpha to fade text out during transitions.
                color = new Color(color.R, color.G, color.B, TransitionAlpha);

                // Draw text, centered on the middle of each line.
                Vector2 origin = new Vector2(0, _spriteFont.LineSpacing / 2);
                Vector2 size = _spriteFont.MeasureString(menuEntries[i]);
                position.X = viewportSize.X / 2f - size.X / 2f * scale;
                _spriteBatch.DrawString(_spriteFont, menuEntries[i],
                                                     position, color, 0, origin, scale,
                                                     SpriteEffects.None, 0);

                position.Y += _spriteFont.LineSpacing;
            }

            _spriteBatch.End();
        }


        #endregion
    }
}
