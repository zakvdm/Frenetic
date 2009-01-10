using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

using NUnit.Framework;

using Frenetic;

namespace UnitTestLibrary
{
    [TestFixture]
    public class RailgunTests
    {
        [Test]
        public void ShootGunHandledCorrectly()
        {
            TileGrid grid = new TileGrid(null);
            grid.Rows = 10;
            grid.Columns = 10;
            grid.TileHeight = grid.TileWidth = 10;

            grid.Initialize();

            PhysicsManager physicsManager = new PhysicsManager(grid);
            
            RailGun gun = new RailGun();

            MockGameplayObject mockObject1 = new MockGameplayObject();
            MockGameplayObject mockObject2 = new MockGameplayObject();


            mockObject1.Position = new Vector2(15, 15);
            mockObject2.Position = new Vector2(35, 17);

            mockObject1.HookUpPhysics(grid);
            mockObject2.HookUpPhysics(grid);

            Assert.AreEqual(grid[1][1], mockObject1.Tile);
            Assert.AreEqual(grid[1][3], mockObject2.Tile);

            Assert.AreEqual(0, mockObject1.DamageDone);
            Assert.AreEqual(0, mockObject2.DamageDone);

            gun.DamageAmount = 65;
            gun.Fire(new Vector2(50, 17), new Vector2(20, 17), physicsManager);

            Assert.AreEqual(gun.DamageAmount, mockObject1.DamageDone);
            Assert.AreEqual(gun.DamageAmount, mockObject2.DamageDone);

            Assert.AreEqual(gun, mockObject1.LastDamagedBy);
            Assert.AreEqual(gun, mockObject2.LastDamagedBy);
        }
    }
    public class MockGameplayObject : GameplayObject
    {
        public MockGameplayObject()
        {
            Width = 10;
            Height = 10;
        }

        public Tile Tile;
        public void HookUpPhysics(TileGrid grid)
        {
            Tile = grid.GetTile(position);
            Tile.GameplayObjects.Add(this);
        }

        public float DamageDone;
        public GameplayObject LastDamagedBy;
        public override bool Damage(GameplayObject source, float damageAmount)
        {
            DamageDone += damageAmount;
            LastDamagedBy = source;

            return true;
        }
    }

}
