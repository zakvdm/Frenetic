#region File Description
//-----------------------------------------------------------------------------
// InputState.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Frenetic.UserInput;
#endregion

namespace Frenetic
{
    /// <summary>
    /// Helper for reading input from keyboard and gamepad. This public class tracks
    /// the current and previous state of both input devices, and implements query
    /// properties for high level input actions such as "move up through the menu"
    /// or "pause the game".
    /// </summary>
    /// <remarks>
    /// This public class is similar to one in the GameStateManagement sample.
    /// </remarks>
    public class MenuInputState
    {
        public MenuInputState(IKeyboard keyboard)
        {
            _keyboard = keyboard;
        }

        #region Fields

        public KeyboardState CurrentKeyboardState;
        public GamePadState CurrentGamePadState;

        public KeyboardState LastKeyboardState;
        public GamePadState LastGamePadState;

        #endregion

        /// <summary>
        /// Checks for a "menu up" input action (on either keyboard or gamepad).
        /// </summary>
        public bool MenuUp
        {
            get
            {
                return IsNewKeyPress(Keys.Up);
            }
        }


        /// <summary>
        /// Checks for a "menu down" input action (on either keyboard or gamepad).
        /// </summary>
        public bool MenuDown
        {
            get
            {
                return IsNewKeyPress(Keys.Down);
            }
        }


        /// <summary>
        /// Checks for a "menu select" input action (on either keyboard or gamepad).
        /// </summary>
        public bool MenuSelect
        {
            get
            {
                return IsNewKeyPress(Keys.Space) ||
                       IsNewKeyPress(Keys.Enter);
            }
        }


        /// <summary>
        /// Checks for a "menu cancel" input action (on either keyboard or gamepad).
        /// </summary>
        public bool MenuCancel
        {
            get
            {
                return IsNewKeyPress(Keys.Escape);
            }
        }


        /// <summary>
        /// Checks for a "pause the game" input action (on either keyboard or gamepad).
        /// </summary>
        public bool PauseGame
        {
            get
            {
                return IsNewKeyPress(Keys.Escape);
            }
        }

        bool IsNewKeyPress(Keys key)
        {
            return (_keyboard.IsKeyDown(key) && !_keyboard.WasKeyDown(key));
        }


        IKeyboard _keyboard;
    }
}
