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
        public Body Body { get; private set; }

        public void Update()
        {
            Position = _boundaryCollider.MoveWithinBoundary(Position);
        }

        IPhysicsComponent _physicsComponent;
        IBoundaryCollider _boundaryCollider;
    }
}
