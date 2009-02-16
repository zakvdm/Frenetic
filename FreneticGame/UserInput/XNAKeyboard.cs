using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;

namespace Frenetic.UserInput
{
    public class XNAKeyboard : IKeyboard
    {
        public bool IsKeyDown(Keys key)
        {
            if (Locked)
                return false;

            return Microsoft.Xna.Framework.Input.Keyboard.GetState().IsKeyDown(key);
        }
        
        public bool WasKeyDown(Keys key)
        {
            if (_previousState == null)
                return false;

            return _previousState.IsKeyDown(key);
        }

        public void SaveState()
        {
            _previousState = Microsoft.Xna.Framework.Input.Keyboard.GetState();
        }
        public void Lock()
        {
            Locked = true;
        }
        public void Unlock()
        {
            Locked = false;
        }
        public bool Locked { get; private set; }

        public static AlphaNumericKeys AlphaNumericKeys = new AlphaNumericKeys();
        
        KeyboardState _previousState;

    }
}
