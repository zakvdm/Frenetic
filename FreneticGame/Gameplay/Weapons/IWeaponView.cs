using System;
using Microsoft.Xna.Framework;
using Frenetic.Player;

namespace Frenetic.Gameplay.Weapons
{
    public interface IWeaponView
    {
        void DrawWeapon(IWeapons weapon);
        void DrawEffects(Matrix translationMatrix);
    }
}
