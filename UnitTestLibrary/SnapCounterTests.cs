using System;
using NUnit.Framework;
using Frenetic;

namespace UnitTestLibrary
{
    [TestFixture]
    public class SnapCounterTests
    {
        [Test]
        public void CountsSnaps()
        {
            SnapCounter counter = new SnapCounter();
            counter.SnapsPerSecond = 20;  // Every 0.05f seconds is a snap...

            Assert.AreEqual(1, counter.CurrentSnap);

            counter.Process(0.045f);

            Assert.AreEqual(1, counter.CurrentSnap);

            counter.Process(0.01f);

            Assert.AreEqual(2, counter.CurrentSnap);
        }
    }
}
