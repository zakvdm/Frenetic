using System;
using System.Collections.Generic;
using System.Text;

using Frenetic;

using NUnit.Framework;
using Microsoft.Xna.Framework;


namespace UnitTestLibrary
{
    [TestFixture]
    public class PhysicsManagerTests
    {
        TileGrid grid;
        PhysicsManager physicsManager;
        [TestFixtureSetUp]
        public void CreatePhysicsManager()
        {
            grid = new TileGrid(null);
            grid.Rows = 5;
            grid.Columns = 10;
            grid.TileWidth = 10;
            grid.TileHeight = 20;

            grid.Initialize();

            physicsManager = new PhysicsManager(grid);
        }

        [SetUp]
        public void Setup()
        {
            for (int row = 1; row < 6; row++)
            {
                for (int col = 1; col < 11; col++)
                {
                    grid[row][col].Type = TileType.Empty;
                }
            }
        }

        [Test]
        public void ShootRayHorizontally()
        {
            grid[1][5].Type = TileType.Solid;

            SimpleRay ray = new SimpleRay();
            ray.Origin = new Vector2(15, 30);
            Assert.AreEqual(grid[1][1], grid.GetTile(ray.Origin));
            ray.Direction = Vector2.UnitX;

            List<CollisionResult> collisions = physicsManager.ShootRay(ray);

            Assert.AreEqual(1, collisions.Count);
            Assert.AreEqual(grid[1][5], collisions[0].GameplayObject);
            Assert.AreEqual(new Vector2(50, 30), collisions[0].Position);
            Assert.AreEqual(ray.End, collisions[0].Position);
        }

        [Test]
        public void ShootRayVertically()
        {
            grid[4][3].Type = TileType.Solid;

            SimpleRay ray = new SimpleRay();
            ray.Origin = new Vector2(35, 50);
            Assert.AreEqual(grid[2][3], grid.GetTile(ray.Origin));
            ray.Direction = Vector2.UnitY;

            List<CollisionResult> collisions = physicsManager.ShootRay(ray);

            Assert.AreEqual(1, collisions.Count);
            Assert.AreEqual(grid[4][3], collisions[0].GameplayObject);
            Assert.AreEqual(new Vector2(35, 80), ray.End);
        }
        [Test]
        public void ShootRayDiagonal()
        {
            grid[1][1].Type = TileType.Solid;

            SimpleRay ray = new SimpleRay();
            ray.Origin = new Vector2(35, 70);
            Assert.AreEqual(grid[3][3], grid.GetTile(ray.Origin));
            ray.Direction = new Vector2(-1, -2);

            List<CollisionResult> collisions = physicsManager.ShootRay(ray);

            Assert.AreEqual(1, collisions.Count);
            Assert.AreEqual(grid[1][1], collisions[0].GameplayObject);
            Assert.AreEqual(new Vector2(20, 40), ray.End);
            Assert.AreEqual(ray.End, collisions[0].Position);
        }
        [Test]
        public void ShootRayIntoPlayer()
        {
            OldPlayer player = new OldPlayer();
            player.Position = new Vector2(45, 35);
            Assert.AreEqual(grid[1][4], grid.GetTile(player.Position));

            player.HookUpPhysics(grid);

            SimpleRay ray = new SimpleRay();
            ray.Origin = new Vector2(15, 50);
            Assert.AreEqual(grid[2][1], grid.GetTile(ray.Origin));

            ray.Direction = player.Position - ray.Origin;

            List<CollisionResult> collisions = physicsManager.ShootRay(ray);

            Assert.AreEqual(2, collisions.Count);
            Assert.AreEqual(player, collisions[0].GameplayObject);
            Assert.AreEqual(new Vector2(35, 40), collisions[0].Position);

            Tile tile = collisions[1].GameplayObject as Tile;
            Assert.IsNotNull(tile);
            Assert.AreEqual(new Vector2(75, 20), collisions[1].Position);
        }

        [Test]
        public void ShootRayIntoMultipleObjects()
        {
            OldPlayer player1 = new OldPlayer();
            player1.Position = new Vector2(100, 100);
            OldPlayer player2 = new OldPlayer();
            player2.Position = new Vector2(105, 100);

            player1.HookUpPhysics(grid);
            player2.HookUpPhysics(grid);

            SimpleRay ray = new SimpleRay();
            ray.Origin = new Vector2(50, 100);
            ray.Direction = Vector2.UnitX;

            List<CollisionResult> collisions = physicsManager.ShootRay(ray);
            Assert.AreEqual(new Vector2(90, 100), collisions[0].Position);
            Assert.AreEqual(3, collisions.Count);
            Assert.AreEqual(player1, collisions[0].GameplayObject);
            Assert.AreEqual(player2, collisions[1].GameplayObject);

            /*
            THIS CODE CHECKS IF COLLISIONS ARE GENERATED IN CORRECT ORDER (not needed yet...)
            ray.Origin = new Vector2(150, 100);
            ray.Direction = -1 * Vector2.UnitX;
            collisions = physicsManager.ShootRay(ray);
            Assert.AreEqual(new Vector2(115, 100), collisions[0].Position);
            Assert.AreEqual(player2, collisions[0]);
            Assert.AreEqual(player1, collisions[1]);
            */
        }
    }
}
