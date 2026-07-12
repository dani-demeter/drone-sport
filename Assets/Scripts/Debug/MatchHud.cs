using DroneSport.Gameplay;
using UnityEngine;

namespace DroneSport.DebugTools
{
    public class MatchHud : MonoBehaviour
    {
        [SerializeField] private ScoreManager scoreManager;
        [SerializeField] private MatchManager matchManager;

        private void Reset()
        {
            scoreManager = FindFirstObjectByType<ScoreManager>();
            matchManager = FindFirstObjectByType<MatchManager>();
        }

        private void OnGUI()
        {
            if (scoreManager == null && matchManager == null)
            {
                return;
            }

            float x = Screen.width - 220f;
            GUI.Label(new Rect(x, 10, 210, 20), TimeLabel());

            if (scoreManager != null)
            {
                GUI.Label(new Rect(x, 30, 210, 20), $"Team A: {scoreManager.GetScore(TeamId.A)}");
                GUI.Label(new Rect(x, 50, 210, 20), $"Team B: {scoreManager.GetScore(TeamId.B)}");
            }
        }

        private string TimeLabel()
        {
            if (matchManager == null)
            {
                return "Time Left: --:--";
            }

            switch (matchManager.Phase)
            {
                case MatchPhase.Ended:
                    string outcome = matchManager.Winner.HasValue ? $"Team {matchManager.Winner} Wins" : "Draw";
                    return $"Match Ended: {outcome}";
                case MatchPhase.Overtime:
                    return "OVERTIME";
                default:
                    int totalSeconds = Mathf.CeilToInt(matchManager.RemainingSeconds);
                    int minutes = totalSeconds / 60;
                    int seconds = totalSeconds % 60;
                    return $"Time Left: {minutes:00}:{seconds:00}";
            }
        }
    }
}
