using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;

namespace Frenetic.UserInput
{
    public enum MouseKeys
    {
        Left,
        Right,
        Middle
    }
    public interface IMouse
    {
        bool IsKeyDown(MouseKeys mousekey);
    }
}
