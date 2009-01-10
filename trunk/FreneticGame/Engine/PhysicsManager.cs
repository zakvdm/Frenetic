using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Frenetic
{
    /// <summary>
    /// The result of a collision query.
    /// </summary>
    public struct CollisionResult
    {
        /// <summary>
        /// Position of the collision 
        /// </summary>
        public Vector2 Position;

        /// <summary>
        /// How far away did the collision occur down the ray
        /// </summary>
        public float Distance;

        /// <summary>
        /// The collision "direction"
        /// </summary>
        public Vector2 Normal;

        /// <summary>
        /// What caused the collison (what the source ran into)
        /// </summary>
        public GameplayObject GameplayObject;


        public static int Compare(CollisionResult a, CollisionResult b)
        {
            return a.Distance.CompareTo(b.Distance);
        }
    }

    public class PhysicsManager
    {
        private const float gravity = 0.2f;//.3 is a bit much, .1 is a bit "on the moon"..
        private const float drag = 0.999999f;//0 means full drag, 1 is no drag
        private const float bounce = 0.3f;//must be in [0,1], where 1 means full bounce. but 1 seems to incite "the flubber effect" so use 0.9 as a practical upper bound
        private const float friction = 0.05f;

        private TileGrid grid;

        public PhysicsManager(TileGrid grid)
        {
            this.grid = grid;
        }

        public void IntegrateVerlet(OldPlayer player)
        {
            Vector2 position = player.Position;
            Vector2 oldPosition = player.LastPosition;

            float oldX = oldPosition.X;
            float oldY = oldPosition.Y;

            oldPosition.X = position.X;
            oldPosition.Y = position.Y;

            position.X += (drag * position.X) - (drag * oldX);
            position.Y += (drag * position.Y) - (drag * oldY) + gravity;

            player.Position = position;
            player.LastPosition = oldPosition;
        }

        private void ReportCollisionVsWorld(Vector2 projection, Vector2 normal, OldPlayer player)
        {
            Vector2 velocity = player.Position - player.LastPosition;

            float normalComponent = Vector2.Dot(velocity, normal);

            //we only want to apply collision response forces if the object is travelling into, and not out of, the collision
            if (normalComponent >= 0)
            {
                return;
            }

            Vector2 normalVelocity = normal * normalComponent;
            Vector2 tangentVelocity = velocity - normalVelocity;

            // Project object out of collision
            player.Position += projection;

            player.LastPosition += projection + ((1 + bounce) * normalVelocity) + (friction * tangentVelocity);
        }

        #region World Boundary Collisions
        public void CollideRectangleVsWorldBounds(OldPlayer player)
        {
            Vector2 position = player.Position;
            float delta;

            delta = grid.MinimumPoint.X - (position.X - player.HalfWidth);
            if (delta > 0)
            {
                ReportCollisionVsWorld(new Vector2(delta, 0), Vector2.UnitX, player);
            }
            else
            {
                delta = (position.X + player.HalfWidth) - grid.MaximumPoint.X;
                if (delta > 0)
                {
                    ReportCollisionVsWorld(new Vector2(-delta, 0), -1 * Vector2.UnitX, player);
                }
            }
            
            delta = grid.MinimumPoint.Y - (position.Y - player.HalfHeight);
            if (delta > 0)
            {
                ReportCollisionVsWorld(new Vector2(0, delta), Vector2.UnitY, player);
            }
            else
            {
                delta = (position.Y + player.HalfHeight) - grid.MaximumPoint.Y;
                if (delta > 0)
                {
                    ReportCollisionVsWorld(new Vector2(0, -delta), -1 * Vector2.UnitY, player);
                }
            }
        }

        public void CollideCircleVsWorldBounds(OldPlayer player)
        {
            Vector2 position = player.Position;
            int radius = player.Radius;
            float delta;

            delta = grid.MinimumPoint.X - (position.X - radius);
            if (delta > 0)
            {
                ReportCollisionVsWorld(new Vector2(delta, 0), Vector2.UnitX, player);
            }
            else
            {
                delta = (position.X + radius) - grid.MaximumPoint.X;
                if (delta > 0)
                {
                    ReportCollisionVsWorld(new Vector2(-delta, 0), -1 * Vector2.UnitX, player);
                }
            }

            delta = grid.MinimumPoint.Y - (position.Y - radius);
            if (delta > 0)
            {
                ReportCollisionVsWorld(new Vector2(0, delta), Vector2.UnitY, player);
            }
            else 
            {
                delta = (position.Y + radius) - grid.MaximumPoint.Y;
                if (delta > 0)
                {
                    ReportCollisionVsWorld(new Vector2(0, -delta), -1 * Vector2.UnitY, player);
                }
            }
        }
        #endregion

        #region Object Tile Collisions
        public bool CollisionCheckPointVsTile(Vector2 point, Tile tile)
        {
            float delta = point.X - tile.Position.X;
            if (tile.HalfWidth - (Math.Abs(delta)) > 0)
            {
                delta = point.Y - tile.Position.Y;
                if (tile.HalfHeight - (Math.Abs(delta)) > 0)
                {
                    return true;
                }
            }
            return false;
        }
        public void CollideRectangleVsTile(OldPlayer player, Tile tile)
        {
            Vector2 objectPosition = player.Position;

            Vector2 penetration = new Vector2();

            float deltaX = objectPosition.X - tile.Position.X;
            penetration.X = (tile.HalfWidth + player.HalfWidth) - Math.Abs(deltaX);
            if (penetration.X > 0)   // Colliding along X-axis
            {
                float deltaY = objectPosition.Y - tile.Position.Y;
                penetration.Y = (tile.HalfHeight + player.HalfHeight) - Math.Abs(deltaY);
                if (penetration.Y > 0)   // Colliding along Y-axis
                {
                    if (penetration.Y > penetration.X)    // project in X
                    {
                        penetration.Y = 0;
                        if (deltaX < 0) // project to left
                            penetration.X *= -1;
                    }
                    else
                    {
                        //  project in Y
                        penetration.X = 0;
                        if (deltaY < 0) // project up
                            penetration.Y *= -1;
                    }

                    // There may be a collision
                    Vector2 normal = new Vector2();
                    normal = penetration;
                    normal.Normalize();

                    ReportCollisionVsWorld(penetration, normal, player);
                }
            }
        }

        public void CollidePlayerWithTileGrid(OldPlayer player)
        {
            foreach (Tile tile in grid)
            {
                if (tile.Type != TileType.Empty)
                {
                    CollideRectangleVsTile(player, tile);
                }
            }
        }

        public bool IsNearFloor(OldPlayer player)
        {
            foreach (Tile tile in grid)
            {
                if (tile.Type != TileType.Empty)
                {
                    Vector2 playerFoot = new Vector2(player.Position.X, player.Position.Y + player.HalfHeight + 1);
                    playerFoot.X -= player.HalfWidth;
                    if (CollisionCheckPointVsTile(playerFoot, tile))
                    {
                        return true;
                    }
                    playerFoot.X += (2 * player.HalfWidth);
                    if (CollisionCheckPointVsTile(playerFoot, tile))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        #endregion

        #region Ray Queries
        public List<CollisionResult> ShootRay(SimpleRay ray)
        {
            Tile currentTile = grid.GetTile(ray.Origin);

            int stepX, stepY;
            float tMaxX;    // value of t (ray parameter) at which ray crosses the first x boundary
            float tMaxY;    // value of t (ray parameter) at which ray crosses the first y boundary
            float tDeltaX;  // width of a cell in units of 't'
            float tDeltaY;  // height of a cell in units of 't'

            if (ray.Direction.X < 0)
            {
                stepX = -1;
                tMaxX = ((currentTile.Position.X - currentTile.HalfWidth) - ray.Origin.X) / ray.Direction.X;
                tDeltaX = currentTile.Width / -ray.Direction.X;
            }
            else if (ray.Direction.X > 0)
            {
                stepX = 1;
                tMaxX = ((currentTile.Position.X + currentTile.HalfWidth) - ray.Origin.X) / ray.Direction.X;
                tDeltaX = currentTile.Width / ray.Direction.X;
            }
            else
            {
                stepX = 0;
                tMaxX = 100000000;//this is hacky, we SHOULD just handle this case seperately, but for now..
                tDeltaX = 0;
            }

            if (ray.Direction.Y < 0)
            {
                stepY = -1;
                tMaxY = ((currentTile.Position.Y - currentTile.HalfHeight) - ray.Origin.Y) / ray.Direction.Y;
                tDeltaY = currentTile.Height / -ray.Direction.Y;
            }
            else if (ray.Direction.Y > 0)
            {
                stepY = 1;
                tMaxY = ((currentTile.Position.Y + currentTile.HalfHeight) - ray.Origin.Y) / ray.Direction.Y;
                tDeltaY = currentTile.Height / ray.Direction.Y;
            }
            else
            {
                stepY = 0;
                tMaxY = 100000000;  //this is hacky, we SHOULD just handle this case seperately, but for now..
                tDeltaY = 0;
            }

            List<CollisionResult> collisions = new List<CollisionResult>();
            //  Voxel Traversal

            // Test the current Cell
            if (ray.CollideWithGameObject(currentTile))
            {
                CollisionResult currentCollision = new CollisionResult();
                currentCollision.GameplayObject = currentTile;
                currentCollision.Position = ray.End;
                collisions.Add(currentCollision);
                // TODO:  Check if it hits an object first once more tiletypes are possible...
                return collisions;
                
                #region Flashcode
                /*
                 //test vs. object
			var tx = out.x;//cache ray-tile intersection point
			var ty = out.y;
			if(TestRayObj(out,p0.x,p0.y,dx,dy,obj))
			{
				//ray also collides with object; check which is closer to ray origin
				//find the dotprod of obj-intersection->origin vs. raydirection and 
				//tile-intersection->origin vs. raydirection; the largest value (least negative) is the closest
				var dpO = (p0.x - out.x)*dx + (p0.y - out.y)*dy;
				var dpT = (p0.x - tx)*dx + (p0.y - ty)*dy;
				
				//DEBUG
				trace("dpO: " + dpO + "  dpT: " + dpT);
				
				if(dpO < dpT)
				{
					//ray hits tile first
					out.x = tx;
					out.y = ty;
					return false;
				}
				else
				{
					//ray hits object first
					return true;
				}
			}
			else
			{
				//ray missses object; return ray-tile intersection
				out.x = tx;
				out.y = ty;
				return false;
			}
                 */
                #endregion
            }

            SimpleRay traversalRay = new SimpleRay();
            traversalRay.Origin = ray.Origin;
            traversalRay.Direction = ray.Direction;
            Vector2 currentPosition = new Vector2();

            while (currentTile != null)
            {
                foreach (GameplayObject gameplayObject in currentTile.GameplayObjects)
                {
                    if (ray.CollideWithGameObject(gameplayObject))
                    {
                        CollisionResult currentCollision = new CollisionResult();
                        currentCollision.GameplayObject = gameplayObject;
                        currentCollision.Position = ray.End;
                        collisions.Add(currentCollision);
                    }
                }
                if (tMaxX < tMaxY)  // Walk along x
                {
                    if (stepX < 0)
                    {
                        currentTile = currentTile.Left;
                    }
                    else
                    {
                        currentTile = currentTile.Right;
                    }
                    currentPosition.X = ray.Origin.X + (tMaxX * ray.Direction.X);
                    currentPosition.Y = ray.Origin.Y + (tMaxX * ray.Direction.Y);

                    traversalRay.Origin = currentPosition;
                    if (traversalRay.CollideWithGameObject(currentTile))
                    {
                        // TODO:  Check if it hits an object first
                        #region Flashcode
                        /*
                         //test vs. object
						var tx = px;//cache ray-tile intersection point
						var ty = py;
						if(TestRayObj(out,p0.x,p0.y,dx,dy,obj))
						{
							//ray also collides with object; check which is closer to ray origin
							//find the dotprod of obj-intersection->origin vs. raydirection and 
							//tile-intersection->origin vs. raydirection; the largest value (least negative) is the closest
							var dpO = (p0.x - out.x)*dx + (p0.y - out.y)*dy;
							var dpT = (p0.x - tx)*dx + (p0.y - ty)*dy;

							//DEBUG
							trace("dpO: " + dpO + "  dpT: " + dpT);

							if(dpO < dpT)
							{
								//ray hits tile first
								out.x = tx;
								out.y = ty;
								return false;
							}
							else
							{
								//ray hits object first
								return true;
							}
						}
                         */
                        #endregion
                        CollisionResult currentCollision = new CollisionResult();
                        currentCollision.GameplayObject = currentTile;
                        currentCollision.Position = traversalRay.End;
                        collisions.Add(currentCollision);
                        ray.End = traversalRay.End;
                        return collisions;
                    }

                    // Traverse the grid
                    tMaxX += tDeltaX;
                }
                else
                {
                    if (stepY < 0)
                    {
                        currentTile = currentTile.Up;
                    }
                    else
                    {
                        currentTile = currentTile.Down;
                    }
                    currentPosition.X = ray.Origin.X + (tMaxY * ray.Direction.X);
                    currentPosition.Y = ray.Origin.Y + (tMaxY * ray.Direction.Y);

                    traversalRay.Origin = currentPosition;
                    if (traversalRay.CollideWithGameObject(currentTile))
                    {
                        // TODO:  Check if it hits an object first
                        #region Flashcode
                        /*
                         //test vs. object
						var tx = px;//cache ray-tile intersection point
						var ty = py;
						if(TestRayObj(out,p0.x,p0.y,dx,dy,obj))
						{
							//ray also collides with object; check which is closer to ray origin
							//find the dotprod of obj-intersection->origin vs. raydirection and 
							//tile-intersection->origin vs. raydirection; the largest value (least negative) is the closest
							var dpO = (p0.x - out.x)*dx + (p0.y - out.y)*dy;
							var dpT = (p0.x - tx)*dx + (p0.y - ty)*dy;

							//DEBUG
							trace("dpO: " + dpO + "  dpT: " + dpT);

							if(dpO < dpT)
							{
								//ray hits tile first
								out.x = tx;
								out.y = ty;
								return false;
							}
							else
							{
								//ray hits object first
								return true;
							}
						}
                         */
                        #endregion
                        CollisionResult currentCollision = new CollisionResult();
                        currentCollision.GameplayObject = currentTile;
                        currentCollision.Position = traversalRay.End;
                        collisions.Add(currentCollision);
                        ray.End = traversalRay.End;
                        return collisions;
                    }
                    tMaxY += tDeltaY;
                }
            }

            throw (new Exception("Ray Tracer didn't hit anything!"));
        }

        #endregion
    }
}
