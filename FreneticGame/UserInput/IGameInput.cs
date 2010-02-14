using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;

namespace Frenetic.UserInput
{
    public class FreneticKeys
    {
        public List<Keys> Keyboard = new List<Keys>();
        public List<MouseKeys> Mouse = new List<MouseKeys>();
    }
    public enum GameKey
    {
        MoveLeft,
        MoveRight,
        Jump,
        Shoot,
        RocketLauncher,// = Keys.Q,
        RailGun// = Keys.E
    }

    public interface IGameInput
    {
        bool IsGameKeyDown(GameKey gamekey);
    }
}
