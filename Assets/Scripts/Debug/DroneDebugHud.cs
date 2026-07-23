using DroneSport.Drone;
using UnityEngine;

namespace DroneSport.DebugTools
{
    public class DroneDebugHud : MonoBehaviour
    {
        [SerializeField] private DroneFlightController flightController;

        private void Reset()
        {
            flightController = GetComponent<DroneFlightController>();
        }

        private void OnGUI()
        {
            if (flightController == null)
            {
                return;
            }

            Vector3 angularVelocity = flightController.LocalAngularVelocityDegPerSec;
            GUI.Label(new Rect(10, 10, 400, 20), $"Armed: {flightController.IsArmed}");
            GUI.Label(new Rect(10, 30, 400, 20), $"Hover Throttle: {flightController.HoverThrottle01:P0}");
            GUI.Label(new Rect(10, 50, 400, 20), $"Throttle: {flightController.LastThrottle01:P0}");
            GUI.Label(new Rect(10, 70, 400, 20), $"Throttle Safe: {flightController.ThrottleIsSafe}");
            GUI.Label(new Rect(10, 90, 400, 20),
                $"Angular Velocity (deg/s): roll {angularVelocity.z:F0}  pitch {angularVelocity.x:F0}  yaw {angularVelocity.y:F0}");
        }
    }
}
