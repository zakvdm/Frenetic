using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Frenetic.UserInput;
using Frenetic.Gameplay.Level;

namespace Frenetic.Player
{
    public class KeyboardPlayerController : IController
    {
        public static float ShootTimer = 1;
        public static float JumpTimer = 0.5f;
        
        public KeyboardPlayerController(IPlayer player, IKeyboard keyboard, IMouse mouse, ICrosshair crosshair, IPlayerRespawner playerRespawner)
        {
            this.Player = player;
            this.Keyboard = keyboard;
            this.Mouse = mouse;
            this.Crosshair = crosshair;
            this.PlayerRespawner = playerRespawner;

            LastShootTime = float.MinValue;
            LastJumpTime = float.MinValue;
        }
       
        #region IController Members

        public void Process(float elapsedTime)
        {
            TotalElapsedTime += elapsedTime;

            if ((Keyboard.IsKeyDown(Keys.Space) || Keyboard.IsKeyDown(Keys.W)) && CanJump(TotalElapsedTime))
            {
                var jumped = Player.Jump();
                if (jumped)
                {
                    LastJumpTime = TotalElapsedTime;
                }
            }

            if (Keyboard.IsKeyDown(Keys.Left) || Keyboard.IsKeyDown(Keys.A))
            {
                Player.MoveLeft();
            }

            if (Keyboard.IsKeyDown(Keys.Right) || Keyboard.IsKeyDown(Keys.D))
            {
                Player.MoveRight();
            }

            if (Mouse.LeftButtonIsDown())
            {
                if (CanShoot(TotalElapsedTime))
                {
                    if (this.Player.Status == PlayerStatus.Alive)
                    {
                        //this.Player.PendingShot = Crosshair.WorldPosition;
                        this.Player.Shoot(Crosshair.WorldPosition);
                    }
                    else
                    {
                        this.PlayerRespawner.RespawnPlayer(this.Player);
                    }
                    LastShootTime = TotalElapsedTime;
                }
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

        public IPlayer Player { get; private set; }
        IKeyboard Keyboard { get; set; }
        IMouse Mouse { get; set; }
        ICrosshair Crosshair { get; set; }
        IPlayerRespawner PlayerRespawner { get; set; }

        private float TotalElapsedTime { get; set; }

        private float LastShootTime { get; set; }
        private float LastJumpTime { get; set; }
    }
}
