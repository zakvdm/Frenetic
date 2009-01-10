using System;
using System.Collections.Generic;
using System.Text;

using Frenetic;

using NUnit.Framework;
using Microsoft.Xna.Framework;

namespace UnitTestLibrary
{
    [TestFixture]
    public class RayTracerTests
    {
        SimpleRay ray;
        [TestFixtureSetUp]
        public void CreateRay()
        {
            ray = new SimpleRay();
            Assert.AreEqual(Vector2.Zero, ray.Origin);
            Assert.AreEqual(Vector2.Zero, ray.Direction);
        }

        [SetUp]
        public void Setup()
        {
            ray.End = Vector2.Zero;
        }

        [Test]
        public void AimRay()
        {
            ray.Direction = new Vector2(50, 100);
            ray.Origin = new Vector2(10, 20);

            Assert.AreEqual(1, ray.Direction.Length());
            Vector2 normalDirection = new Vector2(50, 100);
            normalDirection.Normalize();
            Assert.AreEqual(normalDirection, ray.Direction);
            Assert.AreEqual(new Vector2(10, 20), ray.Origin);
        }

        [Test]
        public void TestCollideRayWithGameplayObjectThatIsTile()
        {
            TileGrid level = new TileGrid(null);
            level.Rows = 5;
            level.Columns = 5;
            level.TileWidth = 10;
            level.TileHeight = 10;

            level.Initialize();

            level[3][3].Type = TileType.Solid;

            ray.Origin = new Vector2(33, 35);
            ray.Direction = Vector2.UnitX;

            Assert.AreEqual(level[3][3], level.GetTile(ray.Origin));

            Assert.IsTrue(ray.CollideWithGameObject(level[3][3]));

            Assert.AreEqual(new Vector2(33, 35), ray.End);
        }

        [Test]
        public void TestCollideRayWithGameplayObjectThatIsPlayer()
        {
            TileGrid level = new TileGrid(null);
            level.Rows = 5;
            level.Columns = 5;
            level.TileWidth = 10;
            level.TileHeight = 10;

            level.Initialize();

            OldPlayer player = new OldPlayer();
            player.Width = 20;
            player.Height = 40;

            player.Position = new Vector2(55, 35);

            ray.Origin = new Vector2(35, 40);
            ray.Direction = new Vector2(10, -5);

            Assert.IsTrue(ray.CollideWithGameObject(player));

            Assert.AreEqual(new Vector2(45, 35), ray.End);
        }
    }
}
