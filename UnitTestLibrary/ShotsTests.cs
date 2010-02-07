using System;
using NUnit.Framework;
using Frenetic.Gameplay.Weapons;

namespace UnitTestLibrary
{
    [TestFixture]
    public class ShotsTests
    {
        [Test]
        public void CanDirtyState()
        {
            Shots shots = new Shots();

            shots.Add(new Shot());

            Assert.IsTrue(shots.IsDirty);
        }

        [Test]
        public void CanCleanState()
        {
            Shots shots = new Shots();
            shots.Add(new Shot());

            shots.Clean();

            Assert.IsFalse(shots.IsDirty);
        }

        [Test]
        public void ReturnsCorrectDiff()
        {
            Shots shots = new Shots();
            shots.Add(new Shot());
            shots.Clean();

            shots.Add(new Shot());
            shots.Add(new Shot());

            Assert.AreEqual(2, shots.GetDiff().Count);
        }
    }
}
