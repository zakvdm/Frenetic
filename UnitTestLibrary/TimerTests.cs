using System;
using NUnit.Framework;
using Frenetic.Engine;

namespace UnitTestLibrary
{
    [TestFixture]
    public class TimerTests
    {
        [Test]
        public void CanCallProcessWithNoTimersSet()
        {
            Timer timer = new Timer();

            timer.Process(1f);
        }
        [Test]
        public void CanSetANewTimer()
        {
            Timer timer = new Timer();
            bool wasCalled = false;

            timer.AddActionTimer(1f, () => wasCalled = !wasCalled);

            timer.Process(0.5f);
            Assert.IsFalse(wasCalled);

            timer.Process(0.5f);
            Assert.IsTrue(wasCalled);

            timer.Process(0.1f);
            Assert.IsTrue(wasCalled);
        }
        [Test]
        public void StartsTimerFromMomentItGetsSet()
        {
            Timer timer = new Timer();
            timer.Process(100f);
            bool wasCalled = false;

            timer.AddActionTimer(1f, () => wasCalled = !wasCalled);

            timer.Process(0.5f);
            Assert.IsFalse(wasCalled);
        }
        [Test]
        public void CanHaveMultipleToggleTimersAtOnce()
        {
            bool timer1Worked = false;
            bool timer2Worked = false;

            Timer timer = new Timer();

            timer.AddActionTimer(10f, () => timer1Worked = !timer1Worked);
            timer.AddActionTimer(1f, () => timer2Worked = !timer2Worked);

            timer.Process(2f);

            Assert.IsTrue(timer2Worked);

            timer.Process(9f);

            Assert.IsTrue(timer1Worked && timer2Worked);
        }
    }
}
