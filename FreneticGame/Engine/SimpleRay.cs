using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Frenetic
{
    public class SimpleRay
    {
        private Vector2 direction;
        private Vector2 origin;
        private Vector2 end;

        public Vector2 Origin
        {
            get { return origin; }
            set { origin = value; }
        }
        public Vector2 Direction
        {
            get { return direction; }
            set
            {
                direction = value;
                direction.Normalize();
            }
        }
        public Vector2 End
        {
            get { return end; }
            set { end = value; }
        }

        public SimpleRay()
        {
            direction = new Vector2();
            origin = Vector2.Zero;
            end = Vector2.Zero;
        }

        public bool CollideWithGameObject(GameplayObject gameplayObject)
        {
            if (gameplayObject.GetType() == typeof(Tile))
            {
                if (((Tile)gameplayObject).Type == TileType.Solid)
                {
                    // TODO:  Check that we are actually "in" the relevant tile...
                    end = new Vector2(Origin.X, Origin.Y);
                    return true;
                }
                return false;
            }
            else// if (gameplayObject.GetType() == typeof(Player))
            {
                return CollideWithRectangle(gameplayObject);
            }
            
            //return false;
            
        }

        private bool CollideWithRectangle(GameplayObject gameplayObject)
        {
            #region N Code Explanation
            //this uses an interpretation of woo's method for ray-vs-AABB intersection
            //(i've only heard of woo's method third-hand so i have no idea if this is really the way he suggests)
            //
            // -determine closest two sides of AABB to ray origin (by looking at origin position relative to AABB center)
            // -calculate distance along ray (i.e ray parameter) from ray origin to the lines containing these sides
            // -the side with the furthest 'distance' is a candidate for intersection; test it with ray-vs-lineseg
            #endregion

            float xval, yval; // These store potential candidates for the sides to use as collision segments
            
            // STEP 1: Find the two edges of the rectangle closest to the ray origin
            if (origin.X < gameplayObject.Position.X)
            {
                xval = gameplayObject.Position.X - gameplayObject.HalfWidth;
            }
            else
            {
                xval = gameplayObject.Position.X + gameplayObject.HalfWidth;
            }

            if (origin.Y < gameplayObject.Position.Y)
            {
                yval = gameplayObject.Position.Y - gameplayObject.HalfHeight;
            }
            else
            {
                yval = gameplayObject.Position.Y + gameplayObject.HalfHeight;
            }

            // STEP 2:  Calculate teh ray parameter from the ray origin to the lines containting the two closest edges to the origin
            float rayParameter;
            Vector2 point1, point2; // these hold the coordinates of the candidate edge/lineseg

            if (direction.X == 0)
            {
                if (direction.Y == 0)
                {
                    throw new Exception("Ray Collision test called without valid direction.");
                }
                point1.X = gameplayObject.Position.X - gameplayObject.HalfWidth;
                point2.X = gameplayObject.Position.X + gameplayObject.HalfWidth;
                point1.Y = point2.Y = yval;
                
                rayParameter = (yval - origin.Y) / direction.Y;
            }
            else if (direction.Y == 0)
            {
                point1.Y = gameplayObject.Position.Y - gameplayObject.HalfHeight;
                point2.Y = gameplayObject.Position.Y + gameplayObject.HalfHeight;
                point1.X = point2.X = xval;

                rayParameter = (xval - origin.X) / direction.X;
            }
            else
            {
                // Find the potential candidate edge with the greatest ray parameter
                float rayParameterX = (xval - origin.X) / direction.X;
                float rayParameterY = (yval - origin.Y) / direction.Y;

                if (rayParameterX < rayParameterY)
                {
                    // y-edge is the candidate
                    point1.X = gameplayObject.Position.X - gameplayObject.HalfWidth;
                    point2.X = gameplayObject.Position.X + gameplayObject.HalfWidth;
                    point1.Y = point2.Y = yval;
                    rayParameter = rayParameterY;
                }
                else
                {
                    // x-edge is the candidate
                    point1.Y = gameplayObject.Position.Y - gameplayObject.HalfHeight;
                    point2.Y = gameplayObject.Position.Y + gameplayObject.HalfHeight;
                    point1.X = point2.X = xval;
                    rayParameter = rayParameterX;
                }
            }

            // if the ray parameter of the intersection between the ray and the line is negative,
            // the ray points away from the line and we know there can't be an intersection

            if (0 < rayParameter)
            {
                // intersection is possible

                //determine boolean result of ray-lineseg intersection test
                //since we need two points to describe the ray, use a point 10units along the ray
                // STEP 3: ray-line segment intersection test
                Vector2 raySecondPoint = new Vector2(origin.X + 100 * direction.X, origin.Y + 100 * direction.Y);

                float areaPQ1 = (((raySecondPoint.X - origin.X) * (point1.Y - origin.Y)) - ((point1.X - origin.X) * (raySecondPoint.Y - origin.Y)));
                float areaPQ2 = (((raySecondPoint.X - origin.X) * (point2.Y - origin.Y)) - ((point2.X - origin.X) * (raySecondPoint.Y - origin.Y)));

                if ((areaPQ1 * areaPQ2) < 0)
                {
                    // signed areas are different signs; intersection is true
                    end.X = origin.X + rayParameter * direction.X;
                    end.Y = origin.Y + rayParameter * direction.Y;
                    return true;
                }
            }
            // No collision found
            return false;
        }
    }
}
