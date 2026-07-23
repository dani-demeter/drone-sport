using DroneSport.Gameplay;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DroneSport.UI
{
    public class MatchEndPanelController : MonoBehaviour
    {
        [SerializeField] private GameObject panelRoot;
        [SerializeField] private TMP_Text winnerText;
        [SerializeField] private Image winnerColorGraphic;

        private void Update()
        {
            MatchManager matchManager = MatchManager.Instance;
            bool hasEnded = matchManager != null && matchManager.Phase == MatchPhase.Ended;

            panelRoot.SetActive(hasEnded);

            if (!hasEnded)
            {
                return;
            }

            TeamId? winner = matchManager.Winner;
            winnerText.text = winner.HasValue ? $"Team {winner} Wins" : "Draw";

            if (ScoreManager.Instance != null)
            {
                winnerColorGraphic.color = winner.HasValue
                    ? ScoreManager.Instance.GetTeamColor(winner.Value)
                    : ScoreManager.Instance.NeutralGateColor;
            }
        }
    }
}
