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
        public BasePlayer(IPlayerSettings playerSettings, IPhysicsComponent physicsComponent, IBoundaryCollider boundaryCollider, IRailGun weapon, ITimer timer)
        {
            this.PlayerSettings = playerSettings;
            this.PlayerScore = new PlayerScore();

            if (physicsComponent == null)
                this.PhysicsComponent = new DummyPhysicsComponent();
            else
                this.PhysicsComponent = physicsComponent;

            this.PhysicsComponent.CollidedWithWorld += () => InContactWithLevel = true;
            this.PhysicsComponent.Shot += () =>
                {
                    IsAlive = false;
                    this.OnDied();  // TODO: Do i really need this event? (currently used by nothing?)
                    this.PlayerScore.Deaths += 1;
                    timer.AddActionTimer(2f, () => this.IsAlive = true);
                    return;
                };

            this.Weapon = weapon;
            this.Timer = timer;
            this.BoundaryCollider = boundaryCollider;

            IsAlive = true;
            Position = new Vector2(400, 100);
        }
        public BasePlayer() 
        {
            // TODO: REMOVE
            this.PhysicsComponent = new DummyPhysicsComponent();
        } // For XmlSerializer

        
        public IPlayerSettings PlayerSettings { get; protected set; }

        public bool IsAlive { get; set; }
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
            this.Weapon.Shoot(this.Position, targetPosition);
        }

        public virtual void UpdatePositionFromNetwork(Vector2 newestPosition, float deliveryTime)
        {
        }

        protected void OnDied()
        {
            this.Died();
        }
        public event Action Died = delegate { };

        public Vector2? PendingShot { get; set; }

        protected IPhysicsComponent PhysicsComponent;

        protected IRailGun Weapon;
        protected ITimer Timer;
        IBoundaryCollider BoundaryCollider;

        public static Vector2 JumpImpulse = new Vector2(0, -250000);
        public static Vector2 MoveForce = new Vector2(-500000f, 0);
        public static float MaxSpeed = 1000;

        internal bool InContactWithLevel { get; set; }
    }
}
