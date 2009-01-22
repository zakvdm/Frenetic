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

            _physicsComponent.CanJump += () => CanJump = true;

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

        public int ID { get; set; }
        public bool CanJump { get; set; }
        private DateTime LastJumpTime { get; set; }

        public void Update()
        {
            Position = _boundaryCollider.MoveWithinBoundary(Position);
        }

        public void Jump()
        {
            if ((DateTime.Now - LastJumpTime).TotalMilliseconds < JumpTimer)
            {
                CanJump = false;
            }

            if (CanJump)
            {
                LastJumpTime = DateTime.Now;
                _physicsComponent.ApplyImpulse(JumpImpulse);
            }
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

        IPhysicsComponent _physicsComponent;
        IBoundaryCollider _boundaryCollider;

        public static Vector2 JumpImpulse = new Vector2(0, -500000);
        public static Vector2 MoveForce = new Vector2(-2000, 0);
        static float MaxSpeed = 50;
        static double JumpTimer = 200;
    }
}
