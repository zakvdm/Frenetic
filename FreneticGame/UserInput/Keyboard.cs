﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;

namespace Frenetic
{
    public class Keyboard : IKeyboard
    {
        public bool IsKeyDown(Keys key)
        {
            return false;
        }
    }
}
