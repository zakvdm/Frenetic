﻿using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

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
        
        Random rand = new Random();
        int count = 0;
        public void Process(long ticks)
        {
            /*
            if (count > 300)
            {
                Player.Position = new Vector2(rand.Next(800), rand.Next(600));
                count = 0;
            }
            count++;
            */

            if (Keyboard.IsKeyDown(Keys.Space))
            {
                Player.Jump();
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
    }
}
