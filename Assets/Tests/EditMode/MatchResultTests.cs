using DroneSport.Gameplay;
using NUnit.Framework;

namespace DroneSport.Tests.EditMode
{
    public class MatchResultTests
    {
        [Test]
        public void DetermineWinner_HigherAScore_ReturnsTeamA()
        {
            Assert.AreEqual(TeamId.A, MatchResult.DetermineWinner(10, 5));
        }

        [Test]
        public void DetermineWinner_HigherBScore_ReturnsTeamB()
        {
            Assert.AreEqual(TeamId.B, MatchResult.DetermineWinner(5, 10));
        }

        [Test]
        public void DetermineWinner_EqualScores_ReturnsNull()
        {
            Assert.IsNull(MatchResult.DetermineWinner(7, 7));
        }
    }
}
