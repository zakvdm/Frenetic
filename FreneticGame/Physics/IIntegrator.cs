using System;
using Microsoft.Xna.Framework;

namespace Frenetic.Physics
{
    public interface IIntegrator
    {
        Vector2 Integrate(Vector2 position);
    }
}
