using System;
using Microsoft.Xna.Framework;

using Frenetic.Physics;
using FarseerGames.FarseerPhysics.Dynamics;
using Frenetic.Weapons;
using Frenetic.Engine;
using Frenetic.Gameplay;

namespace Frenetic.Player
{
    [Serializable()]
    public class Player : IPlayer
    {
        public Player(IPlayerSettings playerSettings, IPhysicsComponent physicsComponent, IBoundaryCollider boundaryCollider, IRailGun weapon, ITimer timer)
        {
            this.PlayerSettings = playerSettings;
            this.PlayerScore = new PlayerScore();

            if (physicsComponent == null)
                _physicsComponent = new DummyPhysicsComponent();
            else
                _physicsComponent = physicsComponent;

            _physicsComponent.CollidedWithWorld += () => InContactWithLevel = true;
            _physicsComponent.OnShot += () =>
                {
                    IsAlive = false;
                    OnDeath();  // TODO: Do i really need this event? (currently used by nothing?)
                    this.PlayerScore.Deaths += 1;
                    timer.AddActionTimer(2f, () => this.IsAlive = true);
                    return;
                };

            _boundaryCollider = boundaryCollider;
            _weapon = weapon;
            _timer = timer;

            IsAlive = true;
            Position = new Vector2(400, 100);
        }
        public Player() 
        {
            _physicsComponent = new DummyPhysicsComponent();
        } // For XmlSerializer

        public IPlayerSettings PlayerSettings { get; private set; }

        public bool IsAlive { get; set; }
        public Vector2 Position
        {
            get
            {
                return _physicsComponent.Position;
            }
            set
            {
                _physicsComponent.Position = value;
            }
        }

        public IRailGun CurrentWeapon
        {
            get { return _weapon; }
        }

        public PlayerScore PlayerScore { get; private set; }

        public void Update()
        {
            Position = _boundaryCollider.MoveWithinBoundary(Position);
            InContactWithLevel = false;
        }

        public bool Jump()
        {
            if (InContactWithLevel)
            {
                InContactWithLevel = false;
                _physicsComponent.ApplyImpulse(JumpImpulse);
                return true;
            }

            return false;
        }

        public void MoveLeft()
        {
            if (_physicsComponent.LinearVelocity.X > -MaxSpeed)
            {
                _physicsComponent.ApplyForce(MoveForce);
            }
        }

        public void MoveRight()
        {
            if (_physicsComponent.LinearVelocity.X < MaxSpeed)
            {
                _physicsComponent.ApplyForce(MoveForce * -1);
            }
        }

        public void Shoot(Vector2 targetPosition)
        {
            _weapon.Shoot(this.Position, targetPosition);
        }

        public event Action OnDeath = delegate { };

        public Vector2? PendingShot { get; set; }

        IPhysicsComponent _physicsComponent;
        IBoundaryCollider _boundaryCollider;

        IRailGun _weapon;
        ITimer _timer;

        public static Vector2 JumpImpulse = new Vector2(0, -250);
        public static Vector2 MoveForce = new Vector2(-0.8f, 0);
        static float MaxSpeed = 10;

        internal bool InContactWithLevel { get; set; }
    }
}
