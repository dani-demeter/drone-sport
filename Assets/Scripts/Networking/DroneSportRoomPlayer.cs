using DroneSport.Gameplay;
using Mirror;
using UnityEngine;

namespace DroneSport.Networking
{
    public class DroneSportRoomPlayer : NetworkRoomPlayer
    {
        private const int MaxPlayerNameLength = 24;

        [SyncVar] private int teamRaw = -1;
        [SyncVar] private string playerName = "Player";

        public TeamId? Team => teamRaw < 0 ? null : (TeamId)teamRaw;
        public string PlayerName => playerName;

        public override void OnStartAuthority()
        {
            CmdSetPlayerName(LocalPlayerSettings.PlayerName);
        }

        [Command]
        public void CmdSetTeam(TeamId newTeam)
        {
            teamRaw = (int)newTeam;
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
        private void CmdSetPlayerName(string newName)
        {
            newName = newName?.Trim();
            playerName = string.IsNullOrEmpty(newName)
                ? "Player"
                : newName[..Mathf.Min(newName.Length, MaxPlayerNameLength)];
        }
    }
}
