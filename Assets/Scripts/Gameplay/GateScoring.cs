namespace DroneSport.Gameplay
{
    public static class GateScoring
    {
        public static int PointsForUniqueDroneCount(int uniqueDroneCount)
        {
            if (uniqueDroneCount <= 0)
            {
                return 0;
            }

            int points = 0;
            for (int i = 0; i < uniqueDroneCount; i++)
            {
                points += 4 + i;
            }

            return points;
        }
    }
}
