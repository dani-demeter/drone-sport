using Mirror;

namespace DroneSport.Networking
{
    public class MatchLengthSelection : NetworkBehaviour
    {
        public static MatchLengthSelection Instance { get; private set; }

        public const float DefaultDurationSeconds = 300f;

        [SyncVar] private float selectedDurationSeconds = DefaultDurationSeconds;

        public float SelectedDurationSeconds => selectedDurationSeconds;

        private void Awake()
        {
            Instance = this;
        }

        [Server]
        public void ServerSetSelectedDuration(float durationSeconds)
        {
            selectedDurationSeconds = durationSeconds;
        }
    }
}
