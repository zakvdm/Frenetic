using System;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Frenetic.Physics;

namespace Frenetic.Weapons
{
    public class RailGun : IRailGun
    {
        public static int Offset = 50;

        public RailGun(IRayCaster rayCaster)
        {
            _rayCaster = rayCaster;
            Shots = new Shots();
        }

        public RailGun()
        {
            // ONLY FOR XML SERIALIZER!
        }

        public Shots Shots { get; private set; }

        public void Shoot(Vector2 origin, Vector2 direction)
        {
            Vector2 endPoint;

            if (direction == Vector2.Zero)
                return;

            Vector2 offsetOrigin = origin + (Vector2.Normalize(direction - origin) * RailGun.Offset);
            List<IPhysicsComponent> hitObjects = _rayCaster.ShootRay(offsetOrigin, direction, out endPoint);

            foreach (IPhysicsComponent physicsComponent in hitObjects)
            {
                physicsComponent.OnShot();
            }
            
            this.Shots.Add(new Shot(origin, endPoint));
        }


        IRayCaster _rayCaster;
    }
}
