using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Frenetic.Physics;
using Microsoft.Xna.Framework;
using Frenetic.Player;

namespace Frenetic.Gameplay.Weapons
{
    public enum WeaponType
    {
        RocketLauncher,
        RailGun
    }
    public interface IWeapons
    {
        IWeapon this[WeaponType type] { get; }

        void Shoot(Vector2 from, Vector2 towards);
        void ChangeWeapon(WeaponType weaponType);

        void RemoveDeadProjectiles();

        event Action<int, IPhysicsComponent> DamagedAPlayer;

        Shots Shots { get; }
    }
}
