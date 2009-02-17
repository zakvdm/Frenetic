using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Frenetic
{
    public class KeyboardPlayerController : IController
    {
        public IPlayer Player { get; private set; }
        IKeyboard Keyboard { get; set; }
        IMouse Mouse { get; set; }
        ICrosshair Crosshair { get; set; }

        public KeyboardPlayerController(IPlayer player, IKeyboard keyboard, IMouse mouse, ICrosshair crosshair)
        {
            Player = player;
            Keyboard = keyboard;
            Mouse = mouse;
            Crosshair = crosshair;

            LastShootTime = long.MinValue;
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

            if (Mouse.LeftButtonIsDown() && CanShoot(TotalTicksElapsed))
            {
                LastShootTime = TotalTicksElapsed;
                Player.Shoot(Crosshair.WorldPosition);
            }

            Player.Update();
        }

        internal bool CanShoot(long time)
        {
            var diff = time - LastShootTime;
            return diff > ShootTimer || diff < 0;
        }

        #endregion

        private long TotalTicksElapsed { get; set; }
        private long LastShootTime { get; set; }

        static long ShootTimer = 2000000;
    }
}
