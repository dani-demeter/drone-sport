using Mirror;
using UnityEngine;

namespace DroneSport.Gameplay
{
    public class ControlGateGroup : NetworkBehaviour
    {
        [SerializeField] private ControlGate[] gates;
        [SerializeField] private float awardIntervalSeconds = 10f;
        [SerializeField] private int pointsPerInterval = 5;
        [SerializeField] private ScoreManager scoreManager;
        [SerializeField] private MatchManager matchManager;

        private float _timeUntilNextAward;

        private void Reset()
        {
            gates = GetComponentsInChildren<ControlGate>();
            scoreManager = FindFirstObjectByType<ScoreManager>();
            matchManager = FindFirstObjectByType<MatchManager>();
        }

        private void Awake()
        {
            _timeUntilNextAward = awardIntervalSeconds;
        }

        private void Update()
        {
            if (!isServer || gates == null || gates.Length == 0)
            {
                return;
            }

            if (matchManager != null && matchManager.Phase == MatchPhase.Ended)
            {
                return;
            }

            _timeUntilNextAward -= Time.deltaTime;
            if (_timeUntilNextAward > 0f)
            {
                return;
            }

            _timeUntilNextAward = awardIntervalSeconds;
            AwardMajorityControlPoints();
        }

        private void AwardMajorityControlPoints()
        {
            int teamACount = 0;
            int teamBCount = 0;

            foreach (ControlGate gate in gates)
            {
                if (gate == null)
                {
                    continue;
                }

                TeamId? controller = gate.ControlledBy;
                if (controller == TeamId.A)
                {
                    teamACount++;
                }
                else if (controller == TeamId.B)
                {
                    teamBCount++;
                }
            }

            TeamId? majority = ControlGateMajority.DetermineMajorityController(teamACount, teamBCount, gates.Length);
            if (!majority.HasValue || scoreManager == null)
            {
                return;
            }

            int controlledCount = majority == TeamId.A ? teamACount : teamBCount;
            int awarded = scoreManager.AwardPointsWithoutConsumingMultiplier(majority.Value, pointsPerInterval);
            Debug.Log($"[{name}] Team {majority} controls majority of control gates ({controlledCount}/{gates.Length}) - awarded {awarded} points");
        }
    }
}
