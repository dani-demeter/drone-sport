using DroneSport.Gameplay;
using NUnit.Framework;

namespace DroneSport.Tests.EditMode
{
    public class ControlGateMajorityTests
    {
        [Test]
        public void DetermineMajorityController_TeamControlsMoreThanHalf_ReturnsThatTeam()
        {
            Assert.AreEqual(TeamId.A, ControlGateMajority.DetermineMajorityController(2, 1, 3));
            Assert.AreEqual(TeamId.B, ControlGateMajority.DetermineMajorityController(1, 3, 4));
        }

        [Test]
        public void DetermineMajorityController_ExactlyHalf_ReturnsNull()
        {
            Assert.IsNull(ControlGateMajority.DetermineMajorityController(2, 2, 4));
        }

        [Test]
        public void DetermineMajorityController_SomeGatesStillNeutral_RequiresTrueMajorityOfTotal()
        {
            // 3 of 5 gates neutral - 1 gate each for A and B is not a majority of the total set.
            Assert.IsNull(ControlGateMajority.DetermineMajorityController(1, 1, 5));
        }

        [Test]
        public void DetermineMajorityController_NoGates_ReturnsNull()
        {
            Assert.IsNull(ControlGateMajority.DetermineMajorityController(0, 0, 0));
        }

        [Test]
        public void DetermineMajorityController_AllGatesNeutral_ReturnsNull()
        {
            Assert.IsNull(ControlGateMajority.DetermineMajorityController(0, 0, 4));
        }

        [Test]
        public void DetermineMajorityController_SingleGateControlled_ReturnsControllingTeam()
        {
            Assert.AreEqual(TeamId.A, ControlGateMajority.DetermineMajorityController(1, 0, 1));
        }
    }
}
