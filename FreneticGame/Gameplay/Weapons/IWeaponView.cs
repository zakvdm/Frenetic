using System;
using Microsoft.Xna.Framework;

namespace Frenetic.Weapons
{
    public interface IWeaponView
    {
        void Draw(Matrix translationMatrix);
    }
}
