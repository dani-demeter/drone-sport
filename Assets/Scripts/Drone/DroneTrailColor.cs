using DroneSport.Gameplay;
using Mirror;
using UnityEngine;

namespace DroneSport.Drone
{
    [RequireComponent(typeof(TrailRenderer))]
    public class DroneTrailColor : NetworkBehaviour
    {
        [SerializeField] private DroneTeam droneTeam;
        [SerializeField] private TrailRenderer trail;
        [SerializeField, Range(0f, 0.15f)] private float hueJitterRange = 0.06f;

        private void Reset()
        {
            droneTeam = GetComponentInParent<DroneTeam>();
            trail = GetComponent<TrailRenderer>();
        }

        public override void OnStartClient()
        {
            ApplyColor();
        }

        private void ApplyColor()
        {
            if (trail == null || droneTeam == null || ScoreManager.Instance == null)
            {
                return;
            }

            Color shade = ApplyPlayerJitter(ScoreManager.Instance.GetTeamColor(droneTeam.Team), netId);

            var gradient = new Gradient();
            gradient.SetKeys(
                new[] { new GradientColorKey(shade, 0f), new GradientColorKey(shade, 1f) },
                new[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(0f, 1f) });

            trail.colorGradient = gradient;
        }

        // Deterministic per-drone hue offset so teammates read as the same
        // color family without every trail being pixel-identical.
        private Color ApplyPlayerJitter(Color color, uint id)
        {
            Color.RGBToHSV(color, out float h, out float s, out float v);
            float spread = (id % 11) / 10f; // 0..1, stable for this drone's whole match
            h = Mathf.Repeat(h + (spread - 0.5f) * 2f * hueJitterRange, 1f);
            return Color.HSVToRGB(h, s, v);
        }
    }
}
