using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Frenetic.Gameplay.Weapons
{
    public enum WeaponType
    {
        RocketLauncher,
        RailGun
    }
    public interface IWeapons
    {
        void ChangeWeapon(WeaponType weaponType);
    }
}
