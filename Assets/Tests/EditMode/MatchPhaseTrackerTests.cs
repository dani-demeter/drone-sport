using DroneSport.Gameplay;
using NUnit.Framework;

namespace DroneSport.Tests.EditMode
{
    public class MatchPhaseTrackerTests
    {
        [Test]
        public void InitialPhase_IsInProgress()
        {
            var tracker = new MatchPhaseTracker();

            Assert.AreEqual(MatchPhase.InProgress, tracker.Phase);
        }

        [Test]
        public void NotifyTimeExpired_NoOpenGates_EndsImmediately()
        {
            var tracker = new MatchPhaseTracker();

            bool ended = tracker.NotifyTimeExpired();

            Assert.IsTrue(ended);
            Assert.AreEqual(MatchPhase.Ended, tracker.Phase);
        }

        [Test]
        public void NotifyTimeExpired_WithOpenGate_EntersOvertime()
        {
            var tracker = new MatchPhaseTracker();
            tracker.NotifyGateOpened();

            bool ended = tracker.NotifyTimeExpired();

            Assert.IsFalse(ended);
            Assert.AreEqual(MatchPhase.Overtime, tracker.Phase);
        }

        [Test]
        public void Overtime_ClosingAllOpenGates_EndsOnTheLastOne()
        {
            var tracker = new MatchPhaseTracker();
            tracker.NotifyGateOpened();
            tracker.NotifyGateOpened();
            tracker.NotifyTimeExpired();

            bool endedAfterFirstClose = tracker.NotifyGateClosed();
            Assert.IsFalse(endedAfterFirstClose);
            Assert.AreEqual(MatchPhase.Overtime, tracker.Phase);

            bool endedAfterSecondClose = tracker.NotifyGateClosed();
            Assert.IsTrue(endedAfterSecondClose);
            Assert.AreEqual(MatchPhase.Ended, tracker.Phase);
        }

        [Test]
        public void NotifyGateClosed_DuringInProgress_DoesNotChangePhase()
        {
            var tracker = new MatchPhaseTracker();
            tracker.NotifyGateOpened();

            bool ended = tracker.NotifyGateClosed();

            Assert.IsFalse(ended);
            Assert.AreEqual(MatchPhase.InProgress, tracker.Phase);
        }

        [Test]
        public void NotifyGateClosed_MoreTimesThanOpened_DoesNotGoNegativeOrEnd()
        {
            var tracker = new MatchPhaseTracker();
            tracker.NotifyGateOpened();
            tracker.NotifyTimeExpired();

            tracker.NotifyGateClosed();
            bool endedAgain = tracker.NotifyGateClosed();

            Assert.IsFalse(endedAgain);
            Assert.AreEqual(MatchPhase.Ended, tracker.Phase);
        }

        [Test]
        public void NotifyTimeExpired_CalledTwice_IsIdempotent()
        {
            var tracker = new MatchPhaseTracker();

            tracker.NotifyTimeExpired();
            bool secondCallEnded = tracker.NotifyTimeExpired();

            Assert.IsFalse(secondCallEnded);
            Assert.AreEqual(MatchPhase.Ended, tracker.Phase);
        }
    }
}
