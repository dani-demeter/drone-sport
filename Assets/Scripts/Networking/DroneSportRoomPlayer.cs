using DroneSport.Gameplay;
using Mirror;
using UnityEngine;

namespace DroneSport.Networking
{
    public class DroneSportRoomPlayer : NetworkRoomPlayer
    {
        private const int MaxPlayerNameLength = 24;

        [SyncVar] private int teamRaw = -1;
        [SyncVar] private int teamSlotIndex;
        [SyncVar] private string playerName = "Player";

        public TeamId? Team => teamRaw < 0 ? null : (TeamId)teamRaw;
        public int TeamSlotIndex => teamSlotIndex;
        public string PlayerName => playerName;

        public override void OnStartAuthority()
        {
            CmdSetPlayerName(LocalPlayerSettings.PlayerName);
        }

        [Command]
        public void CmdSetTeam(TeamId newTeam)
        {
            teamRaw = (int)newTeam;

            if (NetworkManager.singleton is DroneSportNetworkManager roomManager)
            {
                roomManager.ServerRecomputeTeamSlots();
            }
        }

        public void SetTeamSlotIndexServerSide(int slotIndex)
        {
            if (!NetworkServer.active)
            {
                return;
            }

            teamSlotIndex = slotIndex;
        }

        [Command]
        public void CmdSelectMap(int mapIndex)
        {
            if (connectionToClient != NetworkServer.localConnection)
            {
                return;
            }

            MapSelection.Instance?.ServerSetSelectedMap(mapIndex);
        }

        [Command]
        public void CmdSelectMatchLength(float durationSeconds)
        {
            if (connectionToClient != NetworkServer.localConnection)
            {
                return;
            }

            MatchLengthSelection.Instance?.ServerSetSelectedDuration(durationSeconds);
        }

        [Command]
        private void CmdSetPlayerName(string newName)
        {
            newName = newName?.Trim();
            playerName = string.IsNullOrEmpty(newName)
                ? "Player"
                : newName[..Mathf.Min(newName.Length, MaxPlayerNameLength)];
        }
    }
}
