using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Frenetic.UserInput
{
    public class GameInput : IGameInput
    {
        public GameInput(IKeyMapping keyMapping, IKeyboard keyboard, IMouse mouse)
        {
            this.KeyMapping = keyMapping;
            this.Keyboard = keyboard;
            this.Mouse = mouse;
        }
        #region IGameInput Members

        public bool IsGameKeyDown(GameKey gamekey)
        {
            var mapping = this.KeyMapping[gamekey];
            foreach (var keyboardKey in mapping.Keyboard)
            {
                if (this.Keyboard.IsKeyDown(keyboardKey))
                    return true;
            }
            foreach (var mouseKey in mapping.Mouse)
            {
                if (this.Mouse.IsKeyDown(mouseKey))
                    return true;
            }
            return false;
        }

        #endregion
        IKeyMapping KeyMapping { get; set; }
        IKeyboard Keyboard { get; set; }
        IMouse Mouse { get; set; }
    }
}
