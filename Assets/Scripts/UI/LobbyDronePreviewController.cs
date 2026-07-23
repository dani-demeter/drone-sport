using UnityEngine;

namespace DroneSport.UI
{
    // Non-networked stick-mapping demo for the lobby: spins a stationary drone
    // model so players can see which stick does what before a match starts.
    // Rate-mode (acro) like real flight, not angle-mode: releasing the sticks
    // holds whatever attitude the drone is at instead of leveling out, since
    // self-leveling isn't how the drones actually fly. Throttle is ignored
    // entirely since the model never needs to move.
    public class LobbyDronePreviewController : MonoBehaviour
    {
        [Header("Max angular rate (degrees/second)")]
        [SerializeField] private float maxRollRateDegPerSec = 360f;
        [SerializeField] private float maxPitchRateDegPerSec = 360f;
        [SerializeField] private float maxYawRateDegPerSec = 200f;

        [SerializeField] private float responseSpeed = 10f;

        private DroneControlsActions _actions;
        private Vector3 _currentLocalAngularVelocityDeg;

        private void Awake()
        {
            _actions = new DroneControlsActions();
        }

        private void OnEnable()
        {
            _actions.Drone.Enable();
        }

        private void OnDisable()
        {
            _actions.Drone.Disable();
        }

        private void OnDestroy()
        {
            _actions.Dispose();
        }

        private void Update()
        {
            float roll = _actions.Drone.Roll.ReadValue<float>();
            float pitch = _actions.Drone.Pitch.ReadValue<float>();
            float yaw = _actions.Drone.Yaw.ReadValue<float>();

            var targetLocalAngularVelocityDeg = new Vector3(
                pitch * maxPitchRateDegPerSec,
                yaw * maxYawRateDegPerSec,
                roll * maxRollRateDegPerSec);

            float lerpFactor = 1f - Mathf.Exp(-responseSpeed * Time.deltaTime);
            _currentLocalAngularVelocityDeg = Vector3.Lerp(_currentLocalAngularVelocityDeg, targetLocalAngularVelocityDeg, lerpFactor);

            transform.Rotate(_currentLocalAngularVelocityDeg * Time.deltaTime, Space.Self);
        }
    }
}
