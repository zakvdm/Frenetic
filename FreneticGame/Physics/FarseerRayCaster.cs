using System;
using System.Linq;
using Microsoft.Xna.Framework;
using FarseerGames.FarseerPhysics.Collisions;
using System.Collections.Generic;
namespace Frenetic.Physics
{
    public class FarseerRayCaster : IRayCaster
    {
        public FarseerRayCaster(IPhysicsSimulator physicsSimulator)
        {
            _physicsSimulator = physicsSimulator;
        }
        #region IRayCaster Members

        
        public List<IPhysicsComponent> ShootRay(Vector2 origin, Vector2 direction, out Vector2 foundEndPoint)
        {
            List<Vector2> intersectionPoints = new List<Vector2>();
            List<Geom> intersectionGeoms = CollisionHelper.LineSegmentAllGeomsIntersect(ref origin, ref direction, _physicsSimulator.PhysicsSimulator, false, ref intersectionPoints);

            List<IPhysicsComponent> hitPhysicsComponents = new List<IPhysicsComponent>();

            foreach (Geom geom in intersectionGeoms)
            {
                hitPhysicsComponents.Add(geom.Tag as IPhysicsComponent);
            }

            foundEndPoint = direction;

            return hitPhysicsComponents;
        }

        #endregion

        IPhysicsSimulator _physicsSimulator;
    }
}
