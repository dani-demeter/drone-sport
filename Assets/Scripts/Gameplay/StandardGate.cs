using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

namespace DroneSport.Gameplay
{
    [RequireComponent(typeof(Collider))]
    public class StandardGate : NetworkBehaviour
    {
        [SerializeField] private float openDurationSeconds = 3f;
        [SerializeField] private ScoreManager scoreManager;
        [SerializeField] private MatchManager matchManager;

        [Header("Gate Visuals")]
        [SerializeField] private Renderer[] gateRenderers;
        [SerializeField, Range(0f, 1f)] private float minOpenSaturation = 0.35f;
        [SerializeField] private int maxUniqueDronesForFullSaturation = 3;

        private bool _isOpen;
        private TeamId _openingTeam;
        private readonly HashSet<DroneTeam> _passedDrones = new();
        private Coroutine _closeRoutine;

        private void Reset()
        {
            scoreManager = FindFirstObjectByType<ScoreManager>();
            matchManager = FindFirstObjectByType<MatchManager>();
            gateRenderers = GetComponentsInChildren<Renderer>();
        }

        public override void OnStartClient()
        {
            ApplyClosedColorLocal();
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

            if (matchManager != null)
            {
                if (matchManager.Phase == MatchPhase.Ended)
                {
                    return;
                }

                if (matchManager.Phase == MatchPhase.Overtime && !_isOpen)
                {
                    return;
                }
            }

            if (!_isOpen)
            {
                OpenGate(drone);
            }
            else if (drone.Team != _openingTeam)
            {
                DenyGate(drone.Team);
            }
            else
            {
                if (_passedDrones.Add(drone))
                {
                    Debug.Log($"[{name}] Team {_openingTeam} drone passed through ({_passedDrones.Count} unique so far)");
                    RpcPaintOpen(_openingTeam, _passedDrones.Count);
                }
            }
        }

        private void OpenGate(DroneTeam opener)
        {
            _isOpen = true;
            _openingTeam = opener.Team;
            _passedDrones.Clear();
            _passedDrones.Add(opener);
            _closeRoutine = StartCoroutine(CloseAndScoreAfterDelay());
            Debug.Log($"[{name}] Opened by Team {_openingTeam}");
            RpcPaintOpen(_openingTeam, _passedDrones.Count);
            RpcPlayOpenedSound(_openingTeam);

            if (matchManager != null)
            {
                matchManager.NotifyGateOpened();
            }
        }

        private void DenyGate(TeamId denyingTeam)
        {
            if (_closeRoutine != null)
            {
                StopCoroutine(_closeRoutine);
            }

            Debug.Log($"[{name}] Denied by Team {denyingTeam} (was opened by Team {_openingTeam}) - no points awarded");
            RpcPlayDeniedSound(denyingTeam);
            ResetGateState();
        }

        private IEnumerator CloseAndScoreAfterDelay()
        {
            yield return new WaitForSeconds(openDurationSeconds);

            int basePoints = GateScoring.PointsForUniqueDroneCount(_passedDrones.Count);
            if (scoreManager != null)
            {
                int awarded = scoreManager.AwardPoints(_openingTeam, basePoints);
                bool multiplierApplied = awarded != basePoints;
                Debug.Log($"[{name}] Closed: Team {_openingTeam} scored {awarded} points " +
                    $"({_passedDrones.Count} unique drone(s), base {basePoints}{(multiplierApplied ? ", multiplier applied" : "")}). " +
                    $"Team {_openingTeam} total: {scoreManager.GetScore(_openingTeam)}");
            }

            RpcPlayCapturedSound(_openingTeam);
            ResetGateState();
        }

        private void ResetGateState()
        {
            _isOpen = false;
            _passedDrones.Clear();
            _closeRoutine = null;
            RpcPaintClosed();

            if (matchManager != null)
            {
                matchManager.NotifyGateClosed();
            }
        }

        [ClientRpc]
        private void RpcPaintOpen(TeamId team, int passedCount)
        {
            if (scoreManager == null)
            {
                return;
            }

            float t = maxUniqueDronesForFullSaturation <= 1
                ? 1f
                : Mathf.InverseLerp(1, maxUniqueDronesForFullSaturation, passedCount);
            float saturation = Mathf.Lerp(minOpenSaturation, 1f, t);

            Color.RGBToHSV(scoreManager.GetTeamColor(team), out float h, out _, out float v);
            GateColorPainter.Paint(gateRenderers, Color.HSVToRGB(h, saturation, v));
        }

        [ClientRpc]
        private void RpcPaintClosed()
        {
            ApplyClosedColorLocal();
        }

        private void ApplyClosedColorLocal()
        {
            if (scoreManager == null)
            {
                return;
            }

            GateColorPainter.Paint(gateRenderers, scoreManager.NeutralGateColor);
        }

        [ClientRpc]
        private void RpcPlayOpenedSound(TeamId team)
        {
            GateAudioLibrary.Instance?.PlayOpened(team, transform.position);
        }

        [ClientRpc]
        private void RpcPlayCapturedSound(TeamId team)
        {
            GateAudioLibrary.Instance?.PlayCaptured(team, transform.position);
        }

        [ClientRpc]
        private void RpcPlayDeniedSound(TeamId team)
        {
            GateAudioLibrary.Instance?.PlayDenied(team, transform.position);
        }
    }
}
