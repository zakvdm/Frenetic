using System;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Frenetic.Physics;
using Frenetic.Player;

namespace Frenetic.Weapons
{
    public class RailGun : IWeapon
    {
        public static int Offset = 50;

        public RailGun(IRayCaster rayCaster)
        {
            _rayCaster = rayCaster;
            Shots = new Shots();

            this.Damage = 50;
        }

        public int Damage { get; set; }
        public Shots Shots { get; private set; }

        public void Shoot(Vector2 origin, Vector2 direction)
        {
            Vector2 endPoint;

            if (direction == Vector2.Zero)
                return;

            Vector2 offsetOrigin = origin + (Vector2.Normalize(direction - origin) * RailGun.Offset);
            List<IPhysicsComponent> hitObjects = _rayCaster.ShootRay(offsetOrigin, direction, out endPoint);

            this.Shots.Add(new Shot(origin, endPoint));

            foreach (var physicsComponent in hitObjects)
            {
                this.DamagedAPlayer(physicsComponent);
            }
        }

        public event Action<IPhysicsComponent> DamagedAPlayer = delegate { };

        IRayCaster _rayCaster;
    }
}
