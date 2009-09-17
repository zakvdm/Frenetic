using System;
using NUnit.Framework;
using Frenetic.Engine;

namespace UnitTestLibrary
{
    [TestFixture]
    public class TimerTests
    {
        Timer timer;
        [SetUp]
        public void SetUp()
        {
            timer = new Timer();
        }
        [Test]
        public void CanCallProcessWithNoTimersSet()
        {
            timer.UpdateElapsedTime(1f);
        }

        [Test]
        public void CanUseStopwatch()
        {
            timer.StartStopWatch();

            timer.UpdateElapsedTime(1f);
            timer.UpdateElapsedTime(3.5f);

            Assert.AreEqual(4.5f, timer.StopWatchReading);

            timer.StartStopWatch();
            Assert.AreEqual(0f, timer.StopWatchReading);
        }

        [Test]
        public void CanSetANewActionTimer()
        {
            bool wasCalled = false;

            timer.AddActionTimer(1f, () => wasCalled = !wasCalled);

            timer.UpdateElapsedTime(0.5f);
            Assert.IsFalse(wasCalled);

            timer.UpdateElapsedTime(0.5f);
            Assert.IsTrue(wasCalled);

            timer.UpdateElapsedTime(0.1f);
            Assert.IsTrue(wasCalled);
        }
        [Test]
        public void StartsActionTimerFromMomentItGetsSet()
        {
            timer.UpdateElapsedTime(100f);
            bool wasCalled = false;

            timer.AddActionTimer(1f, () => wasCalled = !wasCalled);

            timer.UpdateElapsedTime(0.5f);
            Assert.IsFalse(wasCalled);
        }
        [Test]
        public void CanHaveMultipleActionTimersAtOnce()
        {
            bool timer1Worked = false;
            bool timer2Worked = false;

            timer.AddActionTimer(10f, () => timer1Worked = !timer1Worked);
            timer.AddActionTimer(1f, () => timer2Worked = !timer2Worked);

            timer.UpdateElapsedTime(2f);

            Assert.IsTrue(timer2Worked);

            timer.UpdateElapsedTime(9f);

            Assert.IsTrue(timer1Worked && timer2Worked);
        }
    }
}
