using System;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Frenetic.Physics;

namespace Frenetic.Weapons
{
    public class RailGun : IRailGun
    {
        public RailGun(IRayCaster rayCaster)
        {
            _rayCaster = rayCaster;
            Shots = new List<Shot>();
        }

        public List<Shot> Shots { get; private set; }

        public void Shoot(Vector2 origin, Vector2 direction)
        {
            Vector2 endPoint = _rayCaster.ShootRay(origin, direction);
            this.Shots.Add(new Shot(origin, endPoint));
        }

        IRayCaster _rayCaster;
    }
}
