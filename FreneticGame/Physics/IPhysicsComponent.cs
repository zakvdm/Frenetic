using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Frenetic.Physics
{
    public interface IPhysicsComponent
    {
        Vector2 Position { get; set; }
    }
}
