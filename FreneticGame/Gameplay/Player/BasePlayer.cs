using System;
using Microsoft.Xna.Framework;

using Frenetic.Physics;
using FarseerGames.FarseerPhysics.Dynamics;
using Frenetic.Weapons;
using Frenetic.Engine;
using Frenetic.Gameplay;

namespace Frenetic.Player
{
    public class BasePlayer : IPlayer
    {
        public static int StartHealth = 100;
        public static Vector2 JumpImpulse = new Vector2(0, -250000);
        public static Vector2 MoveForce = new Vector2(-500000f, 0);
        public static float MaxSpeed = 1000;
        public static int CollisionGroup = 1;

        public BasePlayer(IPlayerSettings playerSettings, IPhysicsComponent physicsComponent, IBoundaryCollider boundaryCollider, IRailGun weapon, ITimer timer)
        {
            this.PlayerSettings = playerSettings;
            this.PlayerScore = new PlayerScore();

            if (physicsComponent == null)
                this.PhysicsComponent = new DummyPhysicsComponent();
            else
                this.PhysicsComponent = physicsComponent;

            this.PhysicsComponent.CollidedWithWorld += () => InContactWithLevel = true;
            this.PhysicsComponent.WasShot += Damage;

            this.Weapon = weapon;
            this.Timer = timer;
            this.BoundaryCollider = boundaryCollider;

            this.Health = BasePlayer.StartHealth;
            this.Status = PlayerStatus.Alive;
            this.Position = new Vector2(400, 100);

            this.PhysicsComponent.CollisionGroup = BasePlayer.CollisionGroup;
        }
        
        public IPlayerSettings PlayerSettings { get; protected set; }

        public int Health { get; set; }
        public PlayerStatus Status { get; set; }
        public Vector2 Position
        {
            get
            { return this.PhysicsComponent.Position; }
            set
            { this.PhysicsComponent.Position = value; }
        }

        public IRailGun CurrentWeapon
        {
            get { return this.Weapon; }
        }

        public PlayerScore PlayerScore { get; set; }

        public bool InContactWithLevel { get; set; }

        public virtual void Update()
        {
            this.Position = this.BoundaryCollider.MoveWithinBoundary(Position);
            InContactWithLevel = false;
        }

        public bool Jump()
        {
            if (InContactWithLevel)
            {
                InContactWithLevel = false;
                this.PhysicsComponent.ApplyImpulse(JumpImpulse);
                return true;
            }

            return false;
        }

        public void MoveLeft()
        {
            if (this.PhysicsComponent.LinearVelocity.X > -MaxSpeed)
            {
                this.PhysicsComponent.ApplyForce(MoveForce);
            }
        }

        public void MoveRight()
        {
            if (this.PhysicsComponent.LinearVelocity.X < MaxSpeed)
            {
                this.PhysicsComponent.ApplyForce(MoveForce * -1);
            }
        }

        public void Shoot(Vector2 targetPosition)
        {
            var hitObjects = this.Weapon.Shoot(this.Position, targetPosition);

            foreach (var physicsComponent in hitObjects)
            {
                // Notify everything that we hit
                physicsComponent.OnWasShot(this, this.Weapon.Damage);
            }
        }

        private void Damage(IPlayer shootingPlayer, int damage)
        {
            this.Health -= damage;

            if (this.Health <= 0)
            {
                this.Status = PlayerStatus.Dead;
                this.OnDied();  // TODO: Do i really need this event? (currently used by nothing?)

                // UPDATE SCORES:
                this.PlayerScore.Deaths += 1;
                shootingPlayer.PlayerScore.Kills += 1;
            }
        }

        public virtual void UpdatePositionFromNetwork(Vector2 newestPosition, float deliveryTime)
        {
        }

        protected void OnDied()
        {
            this.Died();
        }
        public event Action Died = delegate { };

        public PlayerStatus? PendingStatus { get; set; }
        public Vector2? PendingShot { get; set; }

        protected IPhysicsComponent PhysicsComponent;

        protected IRailGun Weapon;
        protected ITimer Timer;
        IBoundaryCollider BoundaryCollider;
    }
}
