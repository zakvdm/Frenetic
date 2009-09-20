using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Frenetic.Physics;
using Frenetic.Weapons;
using Frenetic.Engine;

namespace Frenetic.Player
{
    public class LocalPlayer : BasePlayer
    {
        public LocalPlayer(IPlayerSettings playerSettings, IPhysicsComponent physicsComponent, IBoundaryCollider boundaryCollider, IRailGun weapon, ITimer timer)
            : base(playerSettings, physicsComponent, boundaryCollider, weapon, timer)
        { }
    }
}
