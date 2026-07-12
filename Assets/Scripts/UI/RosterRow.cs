using DroneSport.Networking;
using TMPro;
using UnityEngine;

namespace DroneSport.UI
{
    public class RosterRow : MonoBehaviour
    {
        [SerializeField] private TMP_Text nameText;
        [SerializeField] private TMP_Text teamText;
        [SerializeField] private TMP_Text readyText;

        public void Bind(DroneSportRoomPlayer roomPlayer)
        {
            nameText.text = roomPlayer.PlayerName;
            teamText.text = roomPlayer.Team.HasValue ? $"Team {roomPlayer.Team}" : "No Team";
            readyText.text = roomPlayer.readyToBegin ? "Ready" : "Not Ready";
        }
    }
}
