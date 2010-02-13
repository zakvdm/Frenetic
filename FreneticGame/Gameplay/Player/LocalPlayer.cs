using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Frenetic.Physics;
using Frenetic.Gameplay.Weapons;
using Frenetic.Engine;

namespace Frenetic.Player
{
    public class LocalPlayer : BasePlayer
    {
        public LocalPlayer(IPlayerSettings playerSettings, IPhysicsComponent physicsComponent, IBoundaryCollider boundaryCollider, IWeapons weapons, ITimer timer)
            : base(playerSettings, physicsComponent, boundaryCollider, weapons, timer)
        { }
    }
}
