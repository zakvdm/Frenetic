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
            counter.SnapsPerSecond = 20;  // Every 50 ms is a snap...

            Assert.AreEqual(1, counter.CurrentSnap);

            counter.Process(45);

            Assert.AreEqual(1, counter.CurrentSnap);

            counter.Process(10);

            Assert.AreEqual(2, counter.CurrentSnap);
        }
    }
}
