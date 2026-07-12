using Mirror;
using UnityEngine;

namespace DroneSport.Networking
{
    public class LobbyCountdown : NetworkBehaviour
    {
        public static LobbyCountdown Instance { get; private set; }

        [SyncVar] private float syncedRemainingSeconds = -1f;

        public float RemainingSeconds => syncedRemainingSeconds;
        public bool IsCountingDown => syncedRemainingSeconds >= 0f;

        private void Awake()
        {
            Instance = this;
        }

        private void Update()
        {
            if (!isServer || !IsCountingDown)
            {
                return;
            }

            syncedRemainingSeconds -= Time.deltaTime;

            if (syncedRemainingSeconds <= 0f)
            {
                syncedRemainingSeconds = -1f;
                ((DroneSportNetworkManager)NetworkManager.singleton).ServerCompleteCountdownAndStartMatch();
            }
        }

        [Server]
        public void ServerBeginCountdown(float seconds)
        {
            syncedRemainingSeconds = seconds;
        }

        [Server]
        public void ServerCancelCountdown()
        {
            syncedRemainingSeconds = -1f;
        }
    }
}
