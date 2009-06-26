﻿using System;
using Microsoft.Xna.Framework;

using Frenetic.Physics;
using FarseerGames.FarseerPhysics.Dynamics;
using Frenetic.Weapons;

namespace Frenetic.Player
{
    public class Player : IPlayer
    {
        public Player(IPhysicsComponent physicsComponent, IBoundaryCollider boundaryCollider)
        {
            if (physicsComponent == null)
                _physicsComponent = new DummyPhysicsComponent();
            else
                _physicsComponent = physicsComponent;

            _physicsComponent.CollidedWithWorld += () => InContactWithLevel = true;

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

        public IRailGun CurrentWeapon
        {
            get { return _weapon; }
        }

        public void AddWeapon(IRailGun railGun)
        {
            _weapon = railGun;
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
            _weapon.Shoot(this.Position, targetPosition);
        }

        IPhysicsComponent _physicsComponent;
        IBoundaryCollider _boundaryCollider;

        IRailGun _weapon;

        public static Vector2 JumpImpulse = new Vector2(0, -250);
        public static Vector2 MoveForce = new Vector2(-0.8f, 0);
        static float MaxSpeed = 10;

        internal bool InContactWithLevel { get; set; }
    }
}
