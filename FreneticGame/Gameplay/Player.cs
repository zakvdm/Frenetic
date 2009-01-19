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

        public void Update()
        {
            Position = _boundaryCollider.MoveWithinBoundary(Position);
        }

        public void Jump()
        {
            _physicsComponent.ApplyImpulse(JumpForce);
        }

        IPhysicsComponent _physicsComponent;
        IBoundaryCollider _boundaryCollider;

        static Vector2 JumpForce = new Vector2(0, -25000);
    }
}
