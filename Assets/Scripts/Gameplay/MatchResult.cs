namespace DroneSport.Gameplay
{
    public static class MatchResult
    {
        public static TeamId? DetermineWinner(int teamAScore, int teamBScore)
        {
            if (teamAScore == teamBScore)
            {
                return null;
            }

            return teamAScore > teamBScore ? TeamId.A : TeamId.B;
        }
    }
}
