using DroneSport.Networking;
using Mirror;
using UnityEngine;

namespace DroneSport.Gameplay
{
    public class MatchManager : NetworkBehaviour
    {
        public static MatchManager Instance { get; private set; }

        [SerializeField] private float matchDurationSeconds = 300f;
        [SerializeField] private float returnToLobbyDelaySeconds = 10f;
        [SerializeField] private ScoreManager scoreManager;

        [SyncVar] private float syncedRemainingSeconds;
        [SyncVar] private MatchPhase syncedPhase = MatchPhase.InProgress;
        [SyncVar] private int syncedWinnerRaw = -1;

        private MatchClock _clock;
        private readonly MatchPhaseTracker _phaseTracker = new();

        public MatchPhase Phase => syncedPhase;
        public bool IsMatchActive => Phase != MatchPhase.Ended;
        public bool HasEnded => Phase == MatchPhase.Ended;
        public float RemainingSeconds => syncedRemainingSeconds;
        public TeamId? Winner => syncedWinnerRaw < 0 ? null : (TeamId)syncedWinnerRaw;

        private void Reset()
        {
            scoreManager = FindFirstObjectByType<ScoreManager>();
        }

        private void Awake()
        {
            Instance = this;

            if (NetworkManager.singleton is DroneSportNetworkManager roomManager)
            {
                matchDurationSeconds = roomManager.SelectedMatchDurationSeconds;
            }

            _clock = new MatchClock(matchDurationSeconds);
            syncedRemainingSeconds = matchDurationSeconds;
            Debug.Log($"[MatchManager] Match started ({matchDurationSeconds:0}s)");
        }

        private void Update()
        {
            if (!isServer || Phase == MatchPhase.Ended)
            {
                return;
            }

            _clock.Tick(Time.deltaTime);
            syncedRemainingSeconds = _clock.RemainingSeconds;

            if (_clock.JustExpired)
            {
                if (_phaseTracker.NotifyTimeExpired())
                {
                    EndMatch();
                }
                else
                {
                    syncedPhase = _phaseTracker.Phase;
                    Debug.Log("[MatchManager] Time expired with gates still open - entering overtime");
                }
            }
        }

        public void NotifyGateOpened()
        {
            _phaseTracker.NotifyGateOpened();
        }

        public void NotifyGateClosed()
        {
            if (_phaseTracker.NotifyGateClosed())
            {
                EndMatch();
            }
        }

        private void EndMatch()
        {
            int scoreA = scoreManager != null ? scoreManager.GetScore(TeamId.A) : 0;
            int scoreB = scoreManager != null ? scoreManager.GetScore(TeamId.B) : 0;
            TeamId? winner = MatchResult.DetermineWinner(scoreA, scoreB);

            syncedPhase = _phaseTracker.Phase;
            syncedWinnerRaw = winner.HasValue ? (int)winner.Value : -1;

            string outcome = winner.HasValue ? $"Team {winner} wins" : "Draw";
            Debug.Log($"[MatchManager] Match ended: {outcome} ({scoreA} - {scoreB})");

            Invoke(nameof(ServerReturnToLobby), returnToLobbyDelaySeconds);
        }

        private void ServerReturnToLobby()
        {
            (NetworkManager.singleton as DroneSportNetworkManager)?.ServerReturnToLobby();
        }
    }
}
