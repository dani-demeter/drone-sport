using DroneSport.Gameplay;
using TMPro;
using UnityEngine;

namespace DroneSport.UI
{
    public class ScoreboardUIController : MonoBehaviour
    {
        [SerializeField] private TMP_Text teamAScoreText;
        [SerializeField] private TMP_Text teamBScoreText;
        [SerializeField] private TMP_Text timeText;

        private void Update()
        {
            ScoreManager scoreManager = ScoreManager.Instance;
            if (scoreManager != null)
            {
                teamAScoreText.text = scoreManager.GetScore(TeamId.A).ToString();
                teamBScoreText.text = scoreManager.GetScore(TeamId.B).ToString();
            }

            timeText.text = FormatTime(MatchManager.Instance);
        }

        private static string FormatTime(MatchManager matchManager)
        {
            if (matchManager == null)
            {
                return "--:--";
            }

            switch (matchManager.Phase)
            {
                case MatchPhase.Ended:
                    return "GAME OVER";
                case MatchPhase.Overtime:
                    return "OVERTIME";
                default:
                    int totalSeconds = Mathf.CeilToInt(matchManager.RemainingSeconds);
                    return $"{totalSeconds / 60:00}:{totalSeconds % 60:00}";
            }
        }
    }
}
