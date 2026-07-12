using System.Collections.Generic;
using System.Linq;
using DroneSport.Gameplay;
using DroneSport.Networking;
using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DroneSport.UI
{
    public class LobbyUIController : MonoBehaviour
    {
        [SerializeField] private Button teamAButton;
        [SerializeField] private Button teamBButton;
        [SerializeField] private Button readyButton;
        [SerializeField] private TMP_Text readyButtonText;

        [Header("Roster")]
        [SerializeField] private Transform rosterContainer;
        [SerializeField] private RosterRow rosterRowPrefab;
        [SerializeField] private TMP_Text teamACountText;
        [SerializeField] private TMP_Text teamBCountText;

        [Header("Countdown")]
        [SerializeField] private TMP_Text countdownText;

        private readonly Dictionary<DroneSportRoomPlayer, RosterRow> _rows = new();

        private void Awake()
        {
            teamAButton.onClick.AddListener(() => RequestTeam(TeamId.A));
            teamBButton.onClick.AddListener(() => RequestTeam(TeamId.B));
            readyButton.onClick.AddListener(OnReadyClicked);
        }

        private void Update()
        {
            DroneSportRoomPlayer roomPlayer = GetLocalRoomPlayer();

            readyButton.interactable = roomPlayer != null && roomPlayer.Team.HasValue;
            readyButtonText.text = roomPlayer != null && roomPlayer.readyToBegin ? "Cancel Ready" : "Ready";

            UpdateRoster();
            UpdateCountdown();
        }

        private void UpdateCountdown()
        {
            LobbyCountdown countdown = LobbyCountdown.Instance;
            countdownText.text = countdown != null && countdown.IsCountingDown
                ? Mathf.CeilToInt(countdown.RemainingSeconds).ToString()
                : "";
        }

        private void UpdateRoster()
        {
            List<DroneSportRoomPlayer> roomPlayers = GetRoomManager().roomSlots
                .Select(slot => slot as DroneSportRoomPlayer)
                .Where(slot => slot != null)
                .ToList();

            foreach (DroneSportRoomPlayer stalePlayer in _rows.Keys.Except(roomPlayers).ToList())
            {
                Destroy(_rows[stalePlayer].gameObject);
                _rows.Remove(stalePlayer);
            }

            foreach (DroneSportRoomPlayer roomPlayer in roomPlayers)
            {
                if (!_rows.TryGetValue(roomPlayer, out RosterRow row))
                {
                    row = Instantiate(rosterRowPrefab, rosterContainer);
                    _rows.Add(roomPlayer, row);
                }

                row.Bind(roomPlayer);
            }

            teamACountText.text = roomPlayers.Count(p => p.Team == TeamId.A).ToString();
            teamBCountText.text = roomPlayers.Count(p => p.Team == TeamId.B).ToString();
        }

        private static DroneSportNetworkManager GetRoomManager()
        {
            return (DroneSportNetworkManager)NetworkManager.singleton;
        }

        private static void RequestTeam(TeamId team)
        {
            GetLocalRoomPlayer()?.CmdSetTeam(team);
        }

        private static void OnReadyClicked()
        {
            DroneSportRoomPlayer roomPlayer = GetLocalRoomPlayer();
            if (roomPlayer != null)
            {
                roomPlayer.CmdChangeReadyState(!roomPlayer.readyToBegin);
            }
        }

        private static DroneSportRoomPlayer GetLocalRoomPlayer()
        {
            NetworkIdentity localPlayer = NetworkClient.localPlayer;
            return localPlayer != null ? localPlayer.GetComponent<DroneSportRoomPlayer>() : null;
        }
    }
}
