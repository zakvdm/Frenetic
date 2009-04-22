using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using FarseerGames.FarseerPhysics.Collisions;
using FarseerGames.FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework.Input;
using FarseerGames.FarseerPhysics;
using FarseerGames.FarseerPhysics.Factories;

namespace Frenetic.Physics
{
    public class DumbRayCasterTestController : IController
    {
        public DumbRayCasterTestController(DumbRayCaster rayCaster)
        {
            _dumbRayCaster = rayCaster;
        }
        DumbRayCaster _dumbRayCaster;
        #region IController Members

        public void Process(float elapsedTime)
        {
            KeyboardState kbState = Microsoft.Xna.Framework.Input.Keyboard.GetState();

            if (kbState.IsKeyDown(Keys.T))
            {
                StressTest(1000);
            }

            if (!kbState.IsKeyDown(Keys.F))
            {
                return;
            }

            _dumbRayCaster.DetectCollision(new Vector2(150, 420), new Vector2(1, -0.1f), true);
        }

        private void StressTest(int numberOfCasts)
        {
            int raysCast = numberOfCasts;
            Vector2 origin = new Vector2();
            Vector2 direction = new Vector2();

            Random rand = new Random();

            System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();

            stopwatch.Start();
            while (numberOfCasts > 0)
            {
                numberOfCasts--;

                origin.X = rand.Next(800);
                origin.Y = rand.Next(600);
                direction.X = rand.Next(800);
                direction.Y = rand.Next(600);

                _dumbRayCaster.DetectCollision(origin, direction, false);
            }
            stopwatch.Stop();

            Console.WriteLine(raysCast.ToString() + " rays were cast... It took: " + stopwatch.ElapsedMilliseconds.ToString() + "milliseconds!");
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

            _scale = 400;
            int halfwidth = 5;
            int incr = 20;

            Vertices vertices = new Vertices();
            vertices.Add(new Vector2(0, halfwidth));
            vertices.Add(new Vector2(0, -halfwidth));
            for (int i = 0; i < 2 * _scale; i += incr)
            {
                vertices.Add(new Vector2(i, -halfwidth));
            }
            vertices.Add(new Vector2(2 * _scale, -halfwidth));
            vertices.Add(new Vector2(2 * _scale, halfwidth));
            for (int i = 2 * _scale; i > 0; i -= incr)
            {
                vertices.Add(new Vector2(i, halfwidth));
            }

            _rayGeom = GeomFactory.Instance.CreatePolygonGeom(_simulator, _body, vertices, new Vector2(_scale, 0), 0, 50);
            _rayGeom.IsSensor = true;
        }

        public void DetectCollision(Vector2 origin, Vector2 direction)
        {
            DetectCollision(origin, direction, false);
        }

        public void DetectCollision(Vector2 origin, Vector2 direction, bool trace)
        {
            PlaceRay(origin, direction);

            List<Geom> hitGeoms = FindHitGeoms(trace);
            
            if (hitGeoms.Count == 0)
                return;

            Geom closestGeom = FindClosestGeom(hitGeoms, origin, direction, trace);

            Vector2 collisionPoint = FindCollisionPoint(closestGeom, origin, direction, trace);
        }

        private void PlaceRay(Vector2 origin, Vector2 direction)
        {
            _body.Position = origin;
            direction.Normalize();
            double angle = Math.Asin(direction.Y);
            _body.Rotation = (float)angle;
        }

        private List<Geom> FindHitGeoms(bool trace)
        {
            List<Geom> hitGeoms = new List<Geom>();
            int count = 0;
            foreach (Geom geom in _simulator.GeomList)
            {
                if (_rayGeom == geom)
                    continue;

                if (_rayGeom.Collide(geom))
                {
                    // ignore collisions with "back" part of _geom: check if moving in direction of ray moves us
                    // closer or further from ray origin... (it should be further)
                    // NOTE: Currently this will return some false positives if the object is just behind the ray origin.
                    //Vector2 tmp = geom.Position + direction;
                    //if (Vector2.Distance(origin, tmp) < Vector2.Distance(origin, geom.Position))
                    //    continue;

                    hitGeoms.Add(geom);
                    count++;
                }
            }
            if (trace)
                Console.WriteLine("Number of collisions with Ray: " + count.ToString());

            return hitGeoms;
        }

        private Geom FindClosestGeom(List<Geom> hitGeoms, Vector2 origin, Vector2 direction, bool trace)
        {
            float bestDistance = 999999f;
            float dist;
            Geom closestGeom = null;
            foreach (Geom geom in hitGeoms)
            {
                if (geom == _rayGeom)
                    Console.WriteLine("OOPS!");

                Vector2 pos2 = geom.Position;
                dist = Vector2.Distance(origin, pos2);
                if (dist < bestDistance)
                {
                    if (dist < 0)
                        Console.WriteLine("OOPS2!");

                    closestGeom = geom;
                    bestDistance = dist;
                }
            }
            if (trace)
                Console.WriteLine("Inter-geom distance is: " + bestDistance.ToString());

            return closestGeom;
        }

        private Vector2 FindCollisionPoint(Geom geom, Vector2 origin, Vector2 direction, bool trace)
        {
            // Find point on collision ray perpendicular to geom centre:
            Vector2 rayNormal = new Vector2(direction.Y, -direction.X);
            rayNormal.Normalize();

            Vector2 offset = geom.Position - origin;
            float t = ((offset.X * direction.X) + (offset.Y * direction.Y)) / (direction.X + direction.Y);

            Vector2 perpendicularIntersectPoint = origin + (t * direction);

            // Now walk back along the ray until we're outside the geom
            Vector2 pos = perpendicularIntersectPoint;
            while (geom.Collide(pos))
            {
                // NOTE: this is very inefficient (maybe start here if i ever need to speed this up)
                pos = pos - direction;
            }
            if (trace)
                Console.WriteLine("Collision position is: " + pos.ToString());

            return pos;
        }

        PhysicsSimulator _simulator;
        Geom _rayGeom;
        Body _body;

        int _scale = 0;

    }
}
