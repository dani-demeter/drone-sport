using DroneSport.Gameplay;
using NUnit.Framework;

namespace DroneSport.Tests.EditMode
{
    public class MatchClockTests
    {
        [Test]
        public void Constructor_StartsActiveWithFullDuration()
        {
            var clock = new MatchClock(60f);

            Assert.IsTrue(clock.IsActive);
            Assert.AreEqual(60f, clock.RemainingSeconds, 1e-5f);
            Assert.IsFalse(clock.JustExpired);
        }

        [Test]
        public void Tick_UnderDuration_StaysActive()
        {
            var clock = new MatchClock(60f);

            clock.Tick(10f);

            Assert.IsTrue(clock.IsActive);
            Assert.AreEqual(50f, clock.RemainingSeconds, 1e-5f);
            Assert.IsFalse(clock.JustExpired);
        }

        [Test]
        public void Tick_CrossingDuration_ExpiresExactlyOnce()
        {
            var clock = new MatchClock(10f);

            clock.Tick(15f);

            Assert.IsFalse(clock.IsActive);
            Assert.AreEqual(0f, clock.RemainingSeconds, 1e-5f);
            Assert.IsTrue(clock.JustExpired);

            clock.Tick(1f);

            Assert.IsFalse(clock.IsActive);
            Assert.IsFalse(clock.JustExpired);
        }

        [Test]
        public void Tick_RemainingSecondsNeverGoesNegative()
        {
            var clock = new MatchClock(5f);

            clock.Tick(100f);

            Assert.AreEqual(0f, clock.RemainingSeconds, 1e-5f);
        }
    }
}
