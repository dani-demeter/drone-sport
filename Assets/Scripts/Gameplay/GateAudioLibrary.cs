using Mirror;
using UnityEngine;

namespace DroneSport.Gameplay
{
    public class GateAudioLibrary : MonoBehaviour
    {
        public static GateAudioLibrary Instance { get; private set; }

        [System.Serializable]
        private struct AllyOpponentClips
        {
            public AudioClip ally;
            public AudioClip opponent;

            public readonly AudioClip For(bool isAlly) => isAlly ? ally : opponent;
        }

        [SerializeField] private AllyOpponentClips openedClips;
        [SerializeField] private AllyOpponentClips capturedClips;
        [SerializeField] private AllyOpponentClips deniedClips;
        [SerializeField] private AllyOpponentClips multiplierCapturedClips;

        private void Awake()
        {
            Instance = this;
        }

        public void PlayOpened(TeamId team, Vector3 position) => Play(openedClips, team, position);
        public void PlayCaptured(TeamId team, Vector3 position) => Play(capturedClips, team, position);
        public void PlayDenied(TeamId team, Vector3 position) => Play(deniedClips, team, position);
        public void PlayMultiplierCaptured(TeamId team, Vector3 position) => Play(multiplierCapturedClips, team, position);

        private static void Play(AllyOpponentClips clips, TeamId team, Vector3 position)
        {
            TeamId? localTeam = GetLocalTeam();
            if (!localTeam.HasValue)
            {
                return;
            }

            AudioClip clip = clips.For(team == localTeam.Value);
            if (clip != null)
            {
                AudioSource.PlayClipAtPoint(clip, position);
            }
        }

        private static TeamId? GetLocalTeam()
        {
            NetworkIdentity localPlayer = NetworkClient.localPlayer;
            if (localPlayer == null)
            {
                return null;
            }

            DroneTeam droneTeam = localPlayer.GetComponent<DroneTeam>();
            return droneTeam != null ? droneTeam.Team : null;
        }
    }
}
