using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Frenetic.Physics
{
    public interface IPhysicsComponent
    {
        bool IsStatic { get; set; }
        Vector2 Position { get; set; }
        Vector2 Size { get; set; }
    }
}
