using System.Collections.Generic;
using DroneSport.Gameplay;
using Mirror;
using UnityEngine;

namespace DroneSport.Networking
{
    public class DroneSportNetworkManager : NetworkRoomManager
    {
        [SerializeField] private float countdownSeconds = 5f;

        [Header("Maps")]
        [Tooltip("Maps the host can pick between in the lobby. Falls back to Gameplay Scene above if empty.")]
        [SerializeField] private MapOption[] availableMaps;

        public IReadOnlyList<MapOption> AvailableMaps => availableMaps;

        public override bool OnRoomServerSceneLoadedForPlayer(NetworkConnectionToClient conn, GameObject roomPlayer, GameObject gamePlayer)
        {
            DroneSportRoomPlayer droneRoomPlayer = roomPlayer.GetComponent<DroneSportRoomPlayer>();
            DroneTeam droneTeam = gamePlayer.GetComponent<DroneTeam>();

            if (droneRoomPlayer != null && droneTeam != null)
            {
                droneTeam.SetTeamServerSide(droneRoomPlayer.Team ?? TeamId.A);
            }

            return true;
        }

        public override GameObject OnRoomServerCreateGamePlayer(NetworkConnectionToClient conn, GameObject roomPlayer)
        {
            DroneSportRoomPlayer droneRoomPlayer = roomPlayer.GetComponent<DroneSportRoomPlayer>();
            TeamId team = droneRoomPlayer != null ? droneRoomPlayer.Team ?? TeamId.A : TeamId.A;

            Transform spawnPoint = DroneSpawnPoints.Instance != null
                ? DroneSpawnPoints.Instance.GetNextSpawnPoint(team)
                : null;

            return spawnPoint != null
                ? Instantiate(playerPrefab, spawnPoint.position, spawnPoint.rotation)
                : null;
        }

        public override void OnRoomServerPlayersReady()
        {
            LobbyCountdown.Instance?.ServerBeginCountdown(countdownSeconds);
        }

        public override void OnRoomServerPlayersNotReady()
        {
            LobbyCountdown.Instance?.ServerCancelCountdown();
        }

        public void ServerCompleteCountdownAndStartMatch()
        {
            GameplayScene = ResolveSelectedMapScenePath();
            ServerChangeScene(GameplayScene);
        }

        public void ServerReturnToLobby()
        {
            ServerChangeScene(RoomScene);
        }

        private string ResolveSelectedMapScenePath()
        {
            if (availableMaps == null || availableMaps.Length == 0)
            {
                return GameplayScene;
            }

            int index = MapSelection.Instance != null
                ? Mathf.Clamp(MapSelection.Instance.SelectedMapIndex, 0, availableMaps.Length - 1)
                : 0;

            return availableMaps[index].sceneName;
        }
    }
}
