using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Frenetic.Gameplay.Weapons
{
    public class WeaponList : IWeapons, IEnumerable<IWeapon>
    {
        public WeaponList(List<IWeapon> weaponList)
        {
            weapons = weaponList;

            foreach (var weapon in weaponList)
            {
                weapon.DamagedAPlayer += (weap, physicsComp) => physicsComp.OnWasShot(null, weap.Damage);
            }
        }
        #region IWeapons Members

        public IWeapon this[WeaponType type]
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public event Action<IWeapon, Frenetic.Physics.IPhysicsComponent> DamagedAPlayer;

        public void Shoot(Microsoft.Xna.Framework.Vector2 from, Microsoft.Xna.Framework.Vector2 towards)
        {
            throw new NotImplementedException();
        }

        public void ChangeWeapon(WeaponType weaponType)
        {
            throw new NotImplementedException();
        }

        public void RemoveDeadProjectiles()
        {
            throw new NotImplementedException();
        }

        public Shots Shots
        {
            get { throw new NotImplementedException(); }
        }

        #endregion

        #region IEnumerable<IWeapon> Members

        public IEnumerator<IWeapon> GetEnumerator()
        {
            foreach (var weapon in this.weapons)
                yield return weapon;
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        #endregion

        List<IWeapon> weapons;
    }
}
