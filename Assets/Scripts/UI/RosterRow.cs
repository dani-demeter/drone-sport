using DroneSport.Gameplay;
using DroneSport.Networking;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DroneSport.UI
{
    public class RosterRow : MonoBehaviour
    {
        [SerializeField] private TMP_Text nameText;
        [SerializeField] private GameObject teamAIcon;
        [SerializeField] private GameObject teamBIcon;
        [SerializeField] private GameObject teamAReadyCheckmark;
        [SerializeField] private GameObject teamBReadyCheckmark;
        [SerializeField] private Image[] tailColorSwatches;
        [SerializeField] private TeamTailColorPalette tailColorPalette;

        public void Bind(DroneSportRoomPlayer roomPlayer)
        {
            nameText.text = roomPlayer.PlayerName;

            bool isTeamA = roomPlayer.Team == TeamId.A;
            bool isTeamB = roomPlayer.Team == TeamId.B;
            bool isReady = roomPlayer.readyToBegin;

            teamAIcon.SetActive(isTeamA);
            teamBIcon.SetActive(isTeamB);
            teamAReadyCheckmark.SetActive(isTeamA && isReady);
            teamBReadyCheckmark.SetActive(isTeamB && isReady);

            bool hasTeam = roomPlayer.Team.HasValue;
            Color? swatchColor = hasTeam && tailColorPalette != null
                ? tailColorPalette.GetColor(roomPlayer.Team.Value, roomPlayer.TeamSlotIndex)
                : null;

            foreach (Image swatch in tailColorSwatches)
            {
                swatch.gameObject.SetActive(hasTeam);
                if (swatchColor.HasValue)
                {
                    swatch.color = swatchColor.Value;
                }
            }
        }
    }
}
