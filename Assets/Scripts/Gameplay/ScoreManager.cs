using Mirror;
using UnityEngine;

namespace DroneSport.Gameplay
{
    public class ScoreManager : NetworkBehaviour
    {
        public static ScoreManager Instance { get; private set; }

        [Header("Team Colors")]
        [SerializeField] private Color teamAColor = new(0.9f, 0.15f, 0.15f);
        [SerializeField] private Color teamBColor = new(0.15f, 0.45f, 0.95f);
        [SerializeField] private Color neutralGateColor = new(0.6f, 0.6f, 0.6f);

        [SyncVar] private int teamAScore;
        [SyncVar] private int teamBScore;
        [SyncVar] private int multiplierControlledByRaw = -1;

        private readonly TeamScoreBoard _scoreBoard = new();

        public TeamId? MultiplierControlledBy => FromRaw(multiplierControlledByRaw);

        public Color NeutralGateColor => neutralGateColor;

        private void Awake()
        {
            Instance = this;
        }

        public int GetScore(TeamId team) => team == TeamId.A ? teamAScore : teamBScore;

        public int AwardPoints(TeamId scoringTeam, int basePoints)
        {
            int awarded = _scoreBoard.AwardPoints(scoringTeam, basePoints);

            teamAScore = _scoreBoard.GetScore(TeamId.A);
            teamBScore = _scoreBoard.GetScore(TeamId.B);
            multiplierControlledByRaw = ToRaw(_scoreBoard.MultiplierControlledBy);

            return awarded;
        }

        public int AwardPointsWithoutConsumingMultiplier(TeamId scoringTeam, int basePoints)
        {
            int awarded = _scoreBoard.AwardPointsWithoutConsumingMultiplier(scoringTeam, basePoints);

            teamAScore = _scoreBoard.GetScore(TeamId.A);
            teamBScore = _scoreBoard.GetScore(TeamId.B);

            return awarded;
        }

        public void SetMultiplierControl(TeamId team)
        {
            _scoreBoard.SetMultiplierControl(team);
            multiplierControlledByRaw = ToRaw(_scoreBoard.MultiplierControlledBy);
        }

        public Color GetTeamColor(TeamId team) => team == TeamId.A ? teamAColor : teamBColor;

        private static int ToRaw(TeamId? team) => team.HasValue ? (int)team.Value : -1;

        private static TeamId? FromRaw(int raw) => raw < 0 ? null : (TeamId)raw;
    }
}
