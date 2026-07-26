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
        [SerializeField] private TeamTailColorPalette palette;

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
            if (trail == null || droneTeam == null || palette == null)
            {
                return;
            }

            Color shade = palette.GetColor(droneTeam.Team, droneTeam.TeamSlotIndex);

            var gradient = new Gradient();
            gradient.SetKeys(
                new[] { new GradientColorKey(shade, 0f), new GradientColorKey(shade, 1f) },
                new[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(0f, 1f) });

            trail.colorGradient = gradient;
        }
    }
}
