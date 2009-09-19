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
        // TODO: Fix the LocalPlayerFarseerPhysicsComponent Autofac fiasco
        public LocalPlayer(IPlayerSettings playerSettings, LocalPlayerFarseerPhysicsComponent physicsComponent, IBoundaryCollider boundaryCollider, IRailGun weapon, ITimer timer)
            : base(playerSettings, physicsComponent, boundaryCollider, weapon, timer)
        { }
    }
}
