using System;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Frenetic.Weapons
{
    public interface IRailGun
    {
        void Shoot(Vector2 origin, Vector2 direction);
        Shots Shots { get; }
    }
}
