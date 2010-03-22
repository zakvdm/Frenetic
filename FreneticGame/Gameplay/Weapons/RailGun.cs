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
            Slugs = new List<Slug>();

            this.Damage = 50;
        }

        public int Damage { get; set; }
        public List<Slug> Slugs { get; private set; }

        public void Shoot(Vector2 origin, Vector2 direction)
        {
            Vector2 endPoint;

            Vector2 offsetOrigin = origin + (Vector2.Normalize(direction - origin) * RailGun.Offset);
            List<IPhysicsComponent> hitObjects = _rayCaster.ShootRay(offsetOrigin, direction, out endPoint);

            this.Slugs.Add(new Slug(origin, endPoint));

            foreach (var physicsComponent in hitObjects)
            {
                this.HitAPhysicsComponent(this, physicsComponent);
            }
        }

        public void RemoveDeadProjectiles()
        {
            // A Slug only lives for one frame (once we draw it we're done with it)
            this.Slugs.Clear();
        }

        public event Action<IWeapon, IPhysicsComponent> HitAPhysicsComponent = delegate { };

        IRayCaster _rayCaster;
    }
}
