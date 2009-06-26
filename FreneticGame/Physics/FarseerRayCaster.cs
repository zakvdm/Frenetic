using System;
using Microsoft.Xna.Framework;
namespace Frenetic.Physics
{
    public class FarseerRayCaster : IRayCaster
    {
        #region IRayCaster Members

        public Vector2 ShootRay(Vector2 origin, Vector2 direction)
        {
            return direction;
        }

        #endregion
    }
}
