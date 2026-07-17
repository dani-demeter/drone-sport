using DroneSport.Gameplay;
using NUnit.Framework;

namespace DroneSport.Tests.EditMode
{
    public class TeamScoreBoardTests
    {
        [Test]
        public void GetScore_InitiallyZeroForBothTeams()
        {
            var board = new TeamScoreBoard();
            Assert.AreEqual(0, board.GetScore(TeamId.A));
            Assert.AreEqual(0, board.GetScore(TeamId.B));
        }

        [Test]
        public void AwardPoints_NoMultiplierControl_AwardsBasePointsUnchanged()
        {
            var board = new TeamScoreBoard();
            int awarded = board.AwardPoints(TeamId.A, 4);

            Assert.AreEqual(4, awarded);
            Assert.AreEqual(4, board.GetScore(TeamId.A));
        }

        [Test]
        public void AwardPoints_ScoringTeamControlsMultiplier_DoublesPointsAndConsumesControl()
        {
            var board = new TeamScoreBoard();
            board.SetMultiplierControl(TeamId.A);

            int awarded = board.AwardPoints(TeamId.A, 4);

            Assert.AreEqual(8, awarded);
            Assert.AreEqual(8, board.GetScore(TeamId.A));
            Assert.IsNull(board.MultiplierControlledBy);
        }

        [Test]
        public void AwardPoints_MultiplierConsumed_SubsequentAwardIsNotDoubled()
        {
            var board = new TeamScoreBoard();
            board.SetMultiplierControl(TeamId.A);
            board.AwardPoints(TeamId.A, 4);

            int secondAward = board.AwardPoints(TeamId.A, 9);

            Assert.AreEqual(9, secondAward);
            Assert.AreEqual(17, board.GetScore(TeamId.A));
        }

        [Test]
        public void AwardPoints_NonControllingTeamScores_MultiplierRemainsPending()
        {
            var board = new TeamScoreBoard();
            board.SetMultiplierControl(TeamId.A);

            int awarded = board.AwardPoints(TeamId.B, 4);

            Assert.AreEqual(4, awarded);
            Assert.AreEqual(4, board.GetScore(TeamId.B));
            Assert.AreEqual(TeamId.A, board.MultiplierControlledBy);
        }

        [Test]
        public void SetMultiplierControl_OverwritesPreviousController()
        {
            var board = new TeamScoreBoard();
            board.SetMultiplierControl(TeamId.A);
            board.SetMultiplierControl(TeamId.B);

            Assert.AreEqual(TeamId.B, board.MultiplierControlledBy);
        }

        [Test]
        public void SetMultiplierControl_OverwritesEvenWithoutExpiry()
        {
            var board = new TeamScoreBoard();
            board.SetMultiplierControl(TeamId.A);
            board.SetMultiplierControl(TeamId.A);

            Assert.AreEqual(TeamId.A, board.MultiplierControlledBy);
        }

        [Test]
        public void AwardPointsWithoutConsumingMultiplier_ScoringTeamControlsMultiplier_DoublesPointsButKeepsControl()
        {
            var board = new TeamScoreBoard();
            board.SetMultiplierControl(TeamId.A);

            int awarded = board.AwardPointsWithoutConsumingMultiplier(TeamId.A, 5);

            Assert.AreEqual(10, awarded);
            Assert.AreEqual(10, board.GetScore(TeamId.A));
            Assert.AreEqual(TeamId.A, board.MultiplierControlledBy);
        }

        [Test]
        public void AwardPointsWithoutConsumingMultiplier_RepeatedCalls_DoubleEveryTimeUntilExplicitlyConsumed()
        {
            var board = new TeamScoreBoard();
            board.SetMultiplierControl(TeamId.A);

            board.AwardPointsWithoutConsumingMultiplier(TeamId.A, 5);
            int secondAward = board.AwardPointsWithoutConsumingMultiplier(TeamId.A, 5);

            Assert.AreEqual(10, secondAward);
            Assert.AreEqual(20, board.GetScore(TeamId.A));
            Assert.AreEqual(TeamId.A, board.MultiplierControlledBy);

            int consumedAward = board.AwardPoints(TeamId.A, 5);
            Assert.AreEqual(10, consumedAward);
            Assert.IsNull(board.MultiplierControlledBy);
        }

        [Test]
        public void AwardPointsWithoutConsumingMultiplier_NoMultiplierControl_AwardsBasePointsUnchanged()
        {
            var board = new TeamScoreBoard();

            int awarded = board.AwardPointsWithoutConsumingMultiplier(TeamId.A, 5);

            Assert.AreEqual(5, awarded);
            Assert.AreEqual(5, board.GetScore(TeamId.A));
        }
    }
}
