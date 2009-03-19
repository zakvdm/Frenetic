using System;
using Microsoft.Xna.Framework;

using Frenetic.Physics;
using FarseerGames.FarseerPhysics.Dynamics;

namespace Frenetic
{
    public class Player : IPlayer
    {
        public delegate IPlayer Factory(int ID);

        public Player(int ID, IPhysicsComponent physicsComponent, IBoundaryCollider boundaryCollider)
        {
            if (physicsComponent == null)
                _physicsComponent = new DummyPhysicsComponent();
            else
                _physicsComponent = physicsComponent;

            _physicsComponent.CollidedWithWorld += () => InContactWithLevel = true;

            this.ID = ID;
            _boundaryCollider = boundaryCollider;

            Position = new Vector2(400, 100);
        }
        public Player() 
        {
            _physicsComponent = new DummyPhysicsComponent();
        } // For XmlSerializer

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
            Console.WriteLine("Shot fired at {0}", targetPosition);
        }

        IPhysicsComponent _physicsComponent;
        IBoundaryCollider _boundaryCollider;

        public static Vector2 JumpImpulse = new Vector2(0, -300000);
        public static Vector2 MoveForce = new Vector2(-2000, 0);
        static float MaxSpeed = 30;
        static long JumpTimer = 8000000;

        public int ID { get; set; }
        
        internal bool InContactWithLevel { get; set; }
    }
}
