using System;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Frenetic.Player;
using Frenetic.Physics;

namespace Frenetic.Gameplay.Weapons
{
    public interface IWeapon
    {
        int Damage { get; }
        void Shoot(Vector2 origin, Vector2 direction);

        event Action<IWeapon, IPhysicsComponent> DamagedAPlayer;
    }

    public interface IProjectileWeapon : IWeapon
    {
        List<Rocket> Rockets { get; }

        void RemoveDeadProjectiles();
    }
}
