using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Frenetic.UserInput;
using Frenetic.Gameplay.Level;
using Frenetic.Gameplay.Weapons;

namespace Frenetic.Player
{
    public class KeyboardPlayerController : IPlayerController
    {
        public static float ShootTimer = 0.5f;
        public static float JumpTimer = 0.5f;
        
        public KeyboardPlayerController(IPlayer player, IGameInput gameInput, ICrosshair crosshair, IPlayerRespawner playerRespawner)
        {
            this.Player = player;
            this.GameInput = gameInput;
            this.Crosshair = crosshair;
            this.PlayerRespawner = playerRespawner;

            LastShootTime = float.MinValue;
            LastJumpTime = float.MinValue;
        }

        public void RemoveDeadProjectiles()
        {
            this.Player.Weapons.RemoveDeadProjectiles();
        }
       
        #region IController Members

        public void Process(float elapsedTime)
        {
            TotalElapsedTime += elapsedTime;

            if (this.GameInput.IsGameKeyDown(GameKey.Jump) && CanJump(TotalElapsedTime))
            {
                var jumped = Player.Jump();
                if (jumped)
                {
                    LastJumpTime = TotalElapsedTime;
                }
            }

            if (this.GameInput.IsGameKeyDown(GameKey.MoveLeft))
            {
                Player.MoveLeft();
            }

            if (this.GameInput.IsGameKeyDown(GameKey.MoveRight))
            {
                Player.MoveRight();
            }

            if (this.GameInput.IsGameKeyDown(GameKey.Shoot))
            {
                if (CanShoot(TotalElapsedTime))
                {
                    if (this.Player.Status == PlayerStatus.Alive)
                    {
                        this.Player.PendingShot = Crosshair.WorldPosition;
                        //this.Player.Shoot(Crosshair.WorldPosition);
                    }
                    else
                    {
                        this.PlayerRespawner.RespawnPlayer(this.Player);
                    }
                    LastShootTime = TotalElapsedTime;
                }
            }

            ProcessGameKeys();

            Player.Update();
        }
        #endregion

        public IPlayer Player { get; private set; }

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

        private void ProcessGameKeys()
        {
            if (this.GameInput.IsGameKeyDown(GameKey.RocketLauncher))
            {
                this.Player.Weapons.ChangeWeapon(WeaponType.RocketLauncher);
            }
            if (this.GameInput.IsGameKeyDown(GameKey.RailGun))
            {
                this.Player.Weapons.ChangeWeapon(WeaponType.RailGun);
            }
        }

        private float TotalElapsedTime { get; set; }

        private float LastShootTime { get; set; }
        private float LastJumpTime { get; set; }

        IGameInput GameInput { get; set; }
        ICrosshair Crosshair { get; set; }
        IPlayerRespawner PlayerRespawner { get; set; }
    }
}
