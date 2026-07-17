using Mirror;
using UnityEngine;

namespace DroneSport.Gameplay
{
    [RequireComponent(typeof(Collider))]
    public class ControlGate : NetworkBehaviour
    {
        [SerializeField] private ScoreManager scoreManager;
        [SerializeField] private MatchManager matchManager;

        [Header("Gate Visuals")]
        [SerializeField] private Renderer[] gateRenderers;

        [SyncVar] private int controlledByRaw = -1;

        public TeamId? ControlledBy => controlledByRaw < 0 ? null : (TeamId)controlledByRaw;

        private void Reset()
        {
            scoreManager = FindFirstObjectByType<ScoreManager>();
            matchManager = FindFirstObjectByType<MatchManager>();
            gateRenderers = GetComponentsInChildren<Renderer>();
        }

        public override void OnStartClient()
        {
            ApplyColorLocal(ControlledBy);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!isServer)
            {
                return;
            }

            DroneTeam drone = other.GetComponentInParent<DroneTeam>();
            if (drone == null)
            {
                return;
            }

            if (matchManager != null && matchManager.Phase == MatchPhase.Ended)
            {
                return;
            }

            if (ControlledBy == drone.Team)
            {
                return;
            }

            controlledByRaw = (int)drone.Team;
            Debug.Log($"[{name}] Control gate captured by Team {drone.Team}");
            RpcPaintControlled(drone.Team);
            RpcPlayCapturedSound(drone.Team);
        }

        [ClientRpc]
        private void RpcPaintControlled(TeamId team)
        {
            ApplyColorLocal(team);
        }

        private void ApplyColorLocal(TeamId? controller)
        {
            if (scoreManager == null)
            {
                return;
            }

            Color color = controller.HasValue ? scoreManager.GetTeamColor(controller.Value) : scoreManager.NeutralGateColor;
            GateColorPainter.Paint(gateRenderers, color);
        }

        [ClientRpc]
        private void RpcPlayCapturedSound(TeamId team)
        {
            GateAudioLibrary.Instance?.PlayControlGateCaptured(team, transform.position);
        }
    }
}
