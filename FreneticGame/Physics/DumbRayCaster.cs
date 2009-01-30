using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Frenetic.Level;
using Microsoft.Xna.Framework;
using FarseerGames.FarseerPhysics.Collisions;
using FarseerGames.FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework.Input;
using FarseerGames.FarseerPhysics;
using FarseerGames.FarseerPhysics.Factories;

namespace Frenetic.Physics
{
    public class TempController : IController
    {
        public TempController(DumbRayCaster rayCaster)
        {
            _dumbRayCaster = rayCaster;
        }
        DumbRayCaster _dumbRayCaster;
        #region IController Members

        public void Process(long ticks)
        {
            KeyboardState kbState = Keyboard.GetState();
            if (!kbState.IsKeyDown(Keys.F))
            {
                return;
            }

            _dumbRayCaster.DetectCollision(new Vector2(300, 320), new Vector2(1, 1));
        }

        #endregion
    }

    public class DumbRayCaster
    {
        public DumbRayCaster(PhysicsSimulator simulator)
        {
            _simulator = simulator;

            // NOTE: These should actually be injected, but this is okay for this hacky implementation.
            _body = BodyFactory.Instance.CreateRectangleBody(_simulator, 1000, 10, 1);

            int length = 800;
            int halfwidth = 5;
            int incr = 10;

            Vertices vertices = new Vertices();
            vertices.Add(new Vector2(0, halfwidth));
            vertices.Add(new Vector2(0, -halfwidth));
            for (int i = 0; i < length; i += incr)
            {
                vertices.Add(new Vector2(i, -halfwidth));
            }
            vertices.Add(new Vector2(length, -halfwidth));
            vertices.Add(new Vector2(length, halfwidth));
            for (int i = length; i > 0; i -= incr)
            {
                vertices.Add(new Vector2(i, halfwidth));
            }

            _geom = GeomFactory.Instance.CreatePolygonGeom(_simulator, _body, vertices, 0);
            _geom.IsSensor = true;
            _geom.ComputeCollisionGrid();
            
        }

        public void DetectCollision(Vector2 origin, Vector2 direction)
        {
            _body.Position = origin;
            direction.Normalize();
            double angle = Math.Asin(direction.Y);
            _body.Rotation = (float)angle;

            List<Geom> hitGeoms = new List<Geom>();
            int count = 0;
            foreach (Geom geom in _simulator.GeomList)
            {
                if (_geom == geom)
                    continue;

                if (_geom.Collide(geom))
                {
                    // ignore collisions with "back" part of _geom: check if moving in direction of ray moves us
                    // closer or further from ray origin... (it should be further)
                    // NOTE: Currently this will return some false positives if the object is just behind the ray origin.
                    Vector2 tmp = geom.Position + direction;
                    if (Vector2.Distance(origin, tmp) < Vector2.Distance(origin, geom.Position))
                        continue;

                    hitGeoms.Add(geom);
                    count++;
                }
            }

            if (hitGeoms.Count == 0)
                return;

            float bestDistance = 999999f;
            float dist;
            Geom closestGeom = null;
            foreach (Geom geom in hitGeoms)
            {
                if (geom == _geom)
                    Console.WriteLine("OOPS!");

                Vector2 pos1 = _geom.Position;
                Vector2 pos2 = geom.Position;
                dist = Vector2.Distance(pos1, pos2);
                if ((dist < bestDistance) && (dist > 0))
                {
                    closestGeom = geom;
                    bestDistance = dist;
                }
            }

            Console.WriteLine("Number of collisions with Ray: " + count.ToString());
            Console.WriteLine("Collision distance is: " + bestDistance.ToString());
            Console.WriteLine("Collision position is: " + closestGeom.Position.ToString());
        }

        PhysicsSimulator _simulator;
        Geom _geom;
        Body _body;

    }
}
