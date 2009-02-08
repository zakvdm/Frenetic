using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;

namespace Frenetic
{
    public interface IKeyboard
    {
        bool IsKeyDown(Keys key);
    }
}