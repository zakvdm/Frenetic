using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;
using Frenetic;

using NUnit.Framework;

namespace UnitTestLibrary
{

    [TestFixture]
    public class NewLevelTests
    {
        LevelCollisionChecker level;
        MockGameplayObject block;

        List<int>[] levelData = new List<int>[] { 
            new List<int> {1,1,1,1,1,1,1,1,1,1},
            new List<int> {1,0,0,0,0,0,0,0,0,1},
            new List<int> {1,0,0,0,0,0,0,0,0,1},
            new List<int> {1,0,0,0,1,1,0,0,0,1},
            new List<int> {1,0,0,0,1,1,0,0,0,1},
            new List<int> {1,0,0,0,0,0,0,0,0,1},
            new List<int> {1,0,0,0,0,0,0,0,0,1},
            new List<int> {1,0,0,0,0,0,0,0,0,1},
            new List<int> {1,0,0,0,0,0,0,0,0,1},
            new List<int> {1,1,1,1,1,1,1,1,1,1}};

        [TestFixtureSetUp]
        public void LevelCanBeCreated()
        {
            level = new LevelCollisionChecker(levelData);
            block = new MockGameplayObject();
            Assert.IsNotNull(level);
        }

        [SetUp]
        public void SetBlockSize()
        {
            block.Width = 2;
            block.Height = 2;
        }

        [Test]
        public void AreRowsAndColumnsNumbersInitiliazed()
        {
            Assert.AreEqual(10, level.NumberOfRows);
            Assert.AreEqual(10, level.NumberOfColumns);
        }

        [Test]
        public void EmptyArrayHandledCorrectly()
        {
            LevelCollisionChecker tempLevel = new LevelCollisionChecker(new List<int>[] {});
            Assert.IsNotNull(tempLevel.NumberOfRows);
        }

        [Test]
        public void NullArrayHandledCorrectly()
        {
            LevelCollisionChecker tempLevel = new LevelCollisionChecker(null);
            Assert.AreEqual(0, tempLevel.NumberOfRows);
            Assert.AreEqual(0, tempLevel.NumberOfColumns);
        }

        [Test]
        public void LevelFindsNoCollisionCorrectly()
        {
            block.Position = new Vector2(2, 2);

            Assert.IsFalse(level.DoesBoundingBoxCollideWithLevel(block));
        }

        [Test]
        public void LevelFindsCollisionCorrectly()
        {
            block.Position = new Vector2(0, 0);
            Assert.IsTrue(level.DoesBoundingBoxCollideWithLevel(block));
        }

        [Test]
        public void BoundaryCollisionCasesHandledCorrectly()
        {
            block.Position = new Vector2(2, 1); //above
            Assert.IsFalse(level.DoesBoundingBoxCollideWithLevel(block));
            block.Position = new Vector2(3, 7); //below
            Assert.IsFalse(level.DoesBoundingBoxCollideWithLevel(block));
            block.Position = new Vector2(1, 2); //left
            Assert.IsFalse(level.DoesBoundingBoxCollideWithLevel(block));
            block.Position = new Vector2(7, 2); //right
            Assert.IsFalse(level.DoesBoundingBoxCollideWithLevel(block));
        }

        [Test]
        public void ArrayIndexedCorrectly()
        {
            LevelCollisionChecker templevel = new LevelCollisionChecker(new List<int>[]{ new List<int> { 0, 0, 0, 0 }, new List<int> { 0, 0, 0, 0 } });
            Assert.IsNotNull(templevel.CollisionArray[1][3]);
        }

        [Test]
        public void RectangularGameplayObjectsHandledCorrectly()
        {
            block.Height = 4;
            block.Position = new Vector2(1, 1);
            Assert.IsFalse(level.DoesBoundingBoxCollideWithLevel(block));
            block.Position = new Vector2(2, 1);
            Assert.IsTrue(level.DoesBoundingBoxCollideWithLevel(block));
        }

        [Test]
        public void GameplayObjectOutsideCollisionArray()
        {
            block.Position = new Vector2(-1, 0);
            Assert.IsTrue(level.DoesBoundingBoxCollideWithLevel(block));
            block.Position = new Vector2(7, 8);
            Assert.IsTrue(level.DoesBoundingBoxCollideWithLevel(block));

        }

        private class MockGameplayObject : GameplayObject
        {
            public MockGameplayObject()
            {
            }
        }
    }
}
