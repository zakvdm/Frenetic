using System;
using Microsoft.Xna.Framework;
using Frenetic;
using NUnit.Framework;

namespace UnitTestLibrary
{
    [TestFixture]
    public class OldPlayerTests
    {
        [Test]
        public void TestPlayerSize()
        {
            OldPlayer player = new OldPlayer();

            Assert.AreEqual(20, player.Width);
            Assert.AreEqual(20, player.Height);
            Assert.AreEqual(10, player.Radius);
        }

        [Test]
        public void TestPlayerGetsInsertedIntoGrid()
        {
            TileGrid grid = new TileGrid(null);
            grid.Rows = 5;
            grid.Columns = 5;
            grid.TileHeight = grid.TileWidth = 20;

            grid.Initialize();

            OldPlayer player = new OldPlayer();
            player.Position = new Vector2(50, 50);
            player.HookUpPhysics(grid);
            Assert.AreEqual(1, grid.GetTile(player.Position).GameplayObjects.Count);
            Assert.AreEqual(player, grid.GetTile(player.Position).GameplayObjects[0]);
        }

        [Test]
        public void TestPlayerMovesCorrectlyInGrid()
        {
            TileGrid grid = new TileGrid(null);
            grid.Rows = 5;
            grid.Columns = 5;
            grid.TileHeight = grid.TileWidth = 20;

            grid.Initialize();

            OldPlayer player = new OldPlayer();
            player.Position = new Vector2(50, 50);
            player.HookUpPhysics(grid);

            Tile tile = grid.GetTile(player.Position);

            player.Position = new Vector2(5, 5);
            Assert.AreEqual(0, tile.GameplayObjects.Count);

            Assert.AreEqual(1, grid.GetTile(player.Position).GameplayObjects.Count);
            Assert.AreEqual(player, grid.GetTile(player.Position).GameplayObjects[0]);
        }

        //[Test]
        public void TestPlayerShootingOtherPlayerWithRailgun()
        {
            TileGrid grid = new TileGrid(null);
            grid.Rows = 10;
            grid.Columns = 10;
            grid.TileHeight = grid.TileWidth = 10;

            grid.Initialize();

            PhysicsManager physicsManager = new PhysicsManager(grid);

            OldPlayer player = new OldPlayer();

            player.Position = new Vector2(15, 15);

            player.HookUpPhysics(grid);

            player.Life = 100;
            player.Weapon.DamageAmount = 65;
            player.Weapon.Fire(new Vector2(50, 17), new Vector2(20, 17), physicsManager);

            Assert.AreEqual(35, player.Life);
            
            Assert.AreEqual(player.Weapon, player.LastDamagedBy);
        }
    }
}
