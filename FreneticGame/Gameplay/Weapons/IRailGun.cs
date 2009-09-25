using System;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Frenetic.Player;
using Frenetic.Physics;

namespace Frenetic.Weapons
{
    public interface IRailGun
    {
        int Damage { get; }
        List<IPhysicsComponent> Shoot(Vector2 origin, Vector2 direction);
        Shots Shots { get; }
    }
}
