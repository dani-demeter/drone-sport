using Mirror;
using UnityEngine;

namespace DroneSport.Gameplay
{
    [RequireComponent(typeof(Collider))]
    public class MultiplierGate : NetworkBehaviour
    {
        [SerializeField] private ScoreManager scoreManager;
        [SerializeField] private MatchManager matchManager;

        [Header("Gate Visuals")]
        [SerializeField] private Renderer[] gateRenderers;

        private TeamId? _lastPaintedController;
        private bool _hasPainted;

        private void Reset()
        {
            scoreManager = FindFirstObjectByType<ScoreManager>();
            matchManager = FindFirstObjectByType<MatchManager>();
            gateRenderers = GetComponentsInChildren<Renderer>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!isServer)
            {
                return;
            }

            DroneTeam drone = other.GetComponentInParent<DroneTeam>();
            if (drone == null || scoreManager == null)
            {
                return;
            }

            if (matchManager != null && matchManager.Phase == MatchPhase.Ended)
            {
                return;
            }

            scoreManager.SetMultiplierControl(drone.Team);
            Debug.Log($"[{name}] Multiplier control taken by Team {drone.Team}");
            RpcPlayMultiplierCapturedSound(drone.Team);
        }

        [ClientRpc]
        private void RpcPlayMultiplierCapturedSound(TeamId team)
        {
            GateAudioLibrary.Instance?.PlayMultiplierCaptured(team, transform.position);
        }

        private void Update()
        {
            if (scoreManager == null)
            {
                return;
            }

            TeamId? controller = scoreManager.MultiplierControlledBy;
            if (_hasPainted && controller == _lastPaintedController)
            {
                return;
            }

            _lastPaintedController = controller;
            _hasPainted = true;

            Color color = controller.HasValue ? scoreManager.GetTeamColor(controller.Value) : scoreManager.NeutralGateColor;
            GateColorPainter.Paint(gateRenderers, color);
        }
    }
}
