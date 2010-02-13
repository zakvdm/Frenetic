using System;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Frenetic.Physics;
using Frenetic.Player;

namespace Frenetic.Gameplay.Weapons
{
    public class RailGun : IWeapon
    {
        public static int Offset = 50;

        public RailGun(IRayCaster rayCaster)
        {
            _rayCaster = rayCaster;
            Slugs = new Shots();

            this.Damage = 50;
        }

        public int Damage { get; set; }
        public Shots Slugs { get; private set; }

        public void Shoot(Vector2 origin, Vector2 direction)
        {
            Vector2 endPoint;

            Vector2 offsetOrigin = origin + (Vector2.Normalize(direction - origin) * RailGun.Offset);
            List<IPhysicsComponent> hitObjects = _rayCaster.ShootRay(offsetOrigin, direction, out endPoint);

            this.Slugs.Add(new Shot(origin, endPoint));

            foreach (var physicsComponent in hitObjects)
            {
                this.DamagedAPlayer(this, physicsComponent);
            }
        }

        public event Action<IWeapon, IPhysicsComponent> DamagedAPlayer = delegate { };

        IRayCaster _rayCaster;
    }
}
