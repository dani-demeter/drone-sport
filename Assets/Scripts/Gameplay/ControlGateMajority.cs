namespace DroneSport.Gameplay
{
    public static class ControlGateMajority
    {
        public static TeamId? DetermineMajorityController(int teamAControlledCount, int teamBControlledCount, int totalGateCount)
        {
            if (totalGateCount <= 0)
            {
                return null;
            }

            if (teamAControlledCount * 2 > totalGateCount)
            {
                return TeamId.A;
            }

            if (teamBControlledCount * 2 > totalGateCount)
            {
                return TeamId.B;
            }

            return null;
        }
    }
}
