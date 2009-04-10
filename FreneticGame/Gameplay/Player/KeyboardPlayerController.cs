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
        IMouse Mouse { get; set; }
        ICrosshair Crosshair { get; set; }

        public KeyboardPlayerController(IPlayer player, IKeyboard keyboard, IMouse mouse, ICrosshair crosshair)
        {
            Player = player;
            Keyboard = keyboard;
            Mouse = mouse;
            Crosshair = crosshair;

            LastShootTime = float.MinValue;
            LastJumpTime = float.MinValue;
        }
        #region IController Members

        public void Process(float elapsedTime)
        {
            TotalElapsedTime += elapsedTime;

            if (Keyboard.IsKeyDown(Keys.Space) && CanJump(TotalElapsedTime))
            {
                var jumped = Player.Jump();
                if (jumped)
                {
                    LastJumpTime = TotalElapsedTime;
                }
            }

            if (Keyboard.IsKeyDown(Keys.Left))
            {
                Player.MoveLeft();
            }

            if (Keyboard.IsKeyDown(Keys.Right))
            {
                Player.MoveRight();
            }

            if (Mouse.LeftButtonIsDown() && CanShoot(TotalElapsedTime))
            {
                Player.Shoot(Crosshair.WorldPosition);
                LastShootTime = TotalElapsedTime;
            }

            Player.Update();
        }

        internal bool CanShoot(float time)
        {
            var diff = time - LastShootTime;
            return diff > ShootTimer || diff < 0;
        }

        internal bool CanJump(float time)
        {
            var diff = time - LastJumpTime;
            return diff > JumpTimer || diff < 0;
        }

        #endregion

        private float TotalElapsedTime { get; set; }

        private float LastShootTime { get; set; }
        private float LastJumpTime { get; set; }

        static float ShootTimer = 250;
        static float JumpTimer = 200;
    }
}
