using System.Collections.Generic;
using System.Linq;
using DroneSport.Gameplay;
using DroneSport.Networking;
using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace DroneSport.UI
{
    public class LobbyUIController : MonoBehaviour
    {
        [SerializeField] private Button teamAButton;
        [SerializeField] private Button teamBButton;
        [SerializeField] private Button readyButton;
        [SerializeField] private TMP_Text readyButtonText;
        [SerializeField] private Button exitLobbyButton;

        [Header("Roster")]
        [SerializeField] private Transform rosterContainer;
        [SerializeField] private RosterRow rosterRowPrefab;
        [SerializeField] private TMP_Text teamACountText;
        [SerializeField] private TMP_Text teamBCountText;

        [Header("Countdown")]
        [SerializeField] private TMP_Text countdownText;

        [Header("Map Selection")]
        [SerializeField] private TMP_Dropdown mapDropdown;

        private readonly Dictionary<DroneSportRoomPlayer, RosterRow> _rows = new();
        private DroneControlsActions _actions;

        private void Awake()
        {
            teamAButton.onClick.AddListener(() => RequestTeam(TeamId.A));
            teamBButton.onClick.AddListener(() => RequestTeam(TeamId.B));
            readyButton.onClick.AddListener(OnReadyClicked);
            exitLobbyButton.onClick.AddListener(OnExitLobbyClicked);
            mapDropdown.onValueChanged.AddListener(OnMapSelected);
            PopulateMapDropdown();

            _actions = new DroneControlsActions();
            _actions.Lobby.ToggleReady.performed += OnToggleReadyPerformed;
        }

        private void OnEnable()
        {
            _actions?.Lobby.Enable();
        }

        private void OnDisable()
        {
            _actions?.Lobby.Disable();
        }

        private void OnDestroy()
        {
            _actions.Lobby.ToggleReady.performed -= OnToggleReadyPerformed;
            _actions.Dispose();
        }

        private void OnToggleReadyPerformed(InputAction.CallbackContext context)
        {
            OnReadyClicked();
        }

        private void Update()
        {
            DroneSportRoomPlayer roomPlayer = GetLocalRoomPlayer();

            readyButton.interactable = roomPlayer != null && roomPlayer.Team.HasValue;
            readyButtonText.text = roomPlayer != null && roomPlayer.readyToBegin ? "Cancel Ready" : "Ready";

            UpdateRoster();
            UpdateCountdown();
            UpdateMapSelection();
        }

        private void PopulateMapDropdown()
        {
            List<string> displayNames = GetRoomManager().AvailableMaps.Select(map => map.displayName).ToList();
            mapDropdown.ClearOptions();
            mapDropdown.AddOptions(displayNames);
        }

        private void UpdateMapSelection()
        {
            mapDropdown.interactable = NetworkServer.active;

            int selectedIndex = MapSelection.Instance != null ? MapSelection.Instance.SelectedMapIndex : 0;
            if (mapDropdown.value != selectedIndex)
            {
                mapDropdown.SetValueWithoutNotify(selectedIndex);
            }
        }

        private static void OnMapSelected(int index)
        {
            GetLocalRoomPlayer()?.CmdSelectMap(index);
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
            if (roomPlayer != null && roomPlayer.Team.HasValue)
            {
                roomPlayer.CmdChangeReadyState(!roomPlayer.readyToBegin);
            }
        }

        private static void OnExitLobbyClicked()
        {
            NetworkManager manager = NetworkManager.singleton;

            if (NetworkServer.active && NetworkClient.isConnected)
            {
                manager.StopHost();
            }
            else if (NetworkClient.isConnected)
            {
                manager.StopClient();
            }
            else if (NetworkServer.active)
            {
                manager.StopServer();
            }
        }

        private static DroneSportRoomPlayer GetLocalRoomPlayer()
        {
            NetworkIdentity localPlayer = NetworkClient.localPlayer;
            return localPlayer != null ? localPlayer.GetComponent<DroneSportRoomPlayer>() : null;
        }
    }
}
