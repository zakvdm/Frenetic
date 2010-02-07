using System;
using Microsoft.Xna.Framework;

namespace Frenetic.Gameplay.Weapons
{
    public interface IWeaponView
    {
        void Draw(Matrix translationMatrix);
    }
}
