using System;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Frenetic.Player;
using Frenetic.Physics;

namespace Frenetic.Weapons
{
    public interface IWeapon
    {
        int Damage { get; }
        void Shoot(Vector2 origin, Vector2 direction);
        Shots Shots { get; }

        event Action<IPhysicsComponent> DamagedAPlayer;
    }

    public interface IProjectileWeapon : IWeapon
    {
        List<Rocket> Rockets { get; }

        void RemoveDeadProjectiles();
    }
}
