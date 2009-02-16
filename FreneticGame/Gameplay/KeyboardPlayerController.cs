using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Frenetic.UserInput;

namespace Frenetic
{
    public class KeyboardPlayerController : IController
    {
        public IPlayer Player { get; private set; }
        IKeyboard Keyboard { get; set; }

        public KeyboardPlayerController(IPlayer player, IKeyboard keyboard)
        {
            Player = player;
            Keyboard = keyboard;
        }
        #region IController Members
        
        public void Process(long ticks)
        {
            TotalTicksElapsed += ticks;

            if (Keyboard.IsKeyDown(Keys.Space))
            {
                Player.Jump(TotalTicksElapsed);
            }

            if (Keyboard.IsKeyDown(Keys.Left))
            {
                Player.MoveLeft();
            }

            if (Keyboard.IsKeyDown(Keys.Right))
            {
                Player.MoveRight();
            }

            Player.Update();
        }

        #endregion

        private long TotalTicksElapsed { get; set; }
    }
}
