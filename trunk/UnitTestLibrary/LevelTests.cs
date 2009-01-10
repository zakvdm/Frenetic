using System;
using System.Collections.Generic;
using System.Text;

using Frenetic;

using NUnit.Framework;
using Microsoft.Xna.Framework;

namespace UnitTestLibrary
{
    [TestFixture]
    public class LevelTests
    {
        private TileGrid level;
        [TestFixtureSetUp]
        public void CreateLevel()
        {

            level = new TileGrid(null);
            level.Rows = 5;
            level.Columns = 10;
            level.TileWidth = 10;
            level.TileHeight = 20;
            
            level.Initialize();

            Assert.AreEqual(12, level[0].Count);
            Assert.IsNotNull(level[6]);
            Assert.IsNull(level[7]);
        }

        [Test]
        public void TestInitialization()
        {
            // Check border
            Assert.AreEqual(level[0][0].Type, TileType.Solid);
            Assert.AreEqual(level[0][11].Type, TileType.Solid);
            Assert.AreEqual(level[6][0].Type, TileType.Solid);
            Assert.AreEqual(level[6][11].Type, TileType.Solid);
        }

        [Test]
        public void TestGetTile()
        {
            Tile tile = level.GetTile(new Vector2(25.3f, 62.1f));
            Assert.AreEqual(level[3][2], tile);
        }

        [Test]
        public void TestNeighbours()
        {
            Tile tile = level.GetTile(new Vector2(25.3f, 62.1f));

            Assert.AreSame(level[3][1], tile.Left);
            Assert.AreEqual(level[3][3], tile.Right);
            Assert.AreEqual(level[2][2], tile.Up);
            Assert.AreEqual(level[4][2], tile.Down);
        }
    }
}
