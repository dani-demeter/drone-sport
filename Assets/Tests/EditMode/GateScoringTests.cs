using DroneSport.Gameplay;
using NUnit.Framework;

namespace DroneSport.Tests.EditMode
{
    public class GateScoringTests
    {
        [Test]
        public void PointsForUniqueDroneCount_OneTwoThreeDrones_MatchesTable()
        {
            Assert.AreEqual(4, GateScoring.PointsForUniqueDroneCount(1));
            Assert.AreEqual(9, GateScoring.PointsForUniqueDroneCount(2));
            Assert.AreEqual(15, GateScoring.PointsForUniqueDroneCount(3));
        }

        [Test]
        public void PointsForUniqueDroneCount_ZeroDrones_IsZero()
        {
            Assert.AreEqual(0, GateScoring.PointsForUniqueDroneCount(0));
            Assert.AreEqual(0, GateScoring.PointsForUniqueDroneCount(-1));
        }

        [Test]
        public void PointsForUniqueDroneCount_IsMonotonicallyIncreasing()
        {
            int previousValue = GateScoring.PointsForUniqueDroneCount(1);
            for (int count = 2; count <= 6; count++)
            {
                int value = GateScoring.PointsForUniqueDroneCount(count);
                Assert.Greater(value, previousValue);
                previousValue = value;
            }
        }
    }
}
