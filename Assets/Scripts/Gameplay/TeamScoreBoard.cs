using System.Collections.Generic;

namespace DroneSport.Gameplay
{
    public class TeamScoreBoard
    {
        private readonly Dictionary<TeamId, int> _scores = new()
        {
            { TeamId.A, 0 },
            { TeamId.B, 0 }
        };

        private TeamId? _multiplierControlledBy;

        public TeamId? MultiplierControlledBy => _multiplierControlledBy;

        public int GetScore(TeamId team) => _scores[team];

        public void SetMultiplierControl(TeamId team)
        {
            _multiplierControlledBy = team;
        }

        public int AwardPoints(TeamId scoringTeam, int basePoints)
        {
            int awarded = basePoints;
            if (_multiplierControlledBy == scoringTeam)
            {
                awarded *= 2;
                _multiplierControlledBy = null;
            }

            _scores[scoringTeam] += awarded;
            return awarded;
        }

        public int AwardPointsWithoutConsumingMultiplier(TeamId scoringTeam, int basePoints)
        {
            int awarded = _multiplierControlledBy == scoringTeam ? basePoints * 2 : basePoints;
            _scores[scoringTeam] += awarded;
            return awarded;
        }
    }
}
