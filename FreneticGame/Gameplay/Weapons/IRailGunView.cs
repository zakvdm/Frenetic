using System;
using Microsoft.Xna.Framework;

namespace Frenetic.Weapons
{
    public interface IRailGunView
    {
        void Draw(IRailGun railGun, Matrix translationMatrix);
    }
}
