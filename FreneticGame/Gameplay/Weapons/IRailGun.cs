﻿using System;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Frenetic.Player;
using Frenetic.Physics;

namespace Frenetic.Weapons
{
    public interface IRailGun
    {
        int Damage { get; }
        void Shoot(Vector2 origin, Vector2 direction);
        Shots Shots { get; }

        event Action<IPhysicsComponent> DamagedAPlayer;
    }
}
