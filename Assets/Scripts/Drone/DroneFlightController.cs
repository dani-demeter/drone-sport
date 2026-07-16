using DroneSport.DebugTools;
using DroneSport.Input;
using Mirror;
using UnityEngine;

namespace DroneSport.Drone
{
    [RequireComponent(typeof(Rigidbody))]
    public class DroneFlightController : NetworkBehaviour
    {
        [Header("Thrust")]
        [SerializeField] private float thrustToWeightRatio = 6f;

        [Header("Max angular rate (degrees/second)")]
        [SerializeField] private float maxRollRateDegPerSec = 720f;
        [SerializeField] private float maxPitchRateDegPerSec = 720f;
        [SerializeField] private float maxYawRateDegPerSec = 400f;

        [Header("Rate controller response")]
        [SerializeField] private float rateGain = 35f;
        [SerializeField] private float maxAngularAccelDegPerSec2 = 10000f;

        [Header("Local Player Presentation")]
        [SerializeField] private DroneDebugHud debugHud;
        [SerializeField] private Camera playerCamera;

        [Header("Self-Right")]
        [SerializeField] private float selfRightLiftMeters = 0.15f;

        private Rigidbody _rigidbody;
        private IDroneInputSource _inputSource;

        public bool IsArmed { get; private set; }
        public float HoverThrottle01 => 1f / thrustToWeightRatio;

        public Vector3 LocalAngularVelocityDegPerSec =>
            transform.InverseTransformDirection(_rigidbody.angularVelocity) * Mathf.Rad2Deg;

        private void Reset()
        {
            debugHud = GetComponent<DroneDebugHud>();
            playerCamera = GetComponentInChildren<Camera>();
        }

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _inputSource = GetComponent<IDroneInputSource>();
        }

        public override void OnStartClient()
        {
            _rigidbody.isKinematic = !isOwned;

            if (!isOwned)
            {
                if (debugHud != null)
                {
                    debugHud.enabled = false;
                }

                if (playerCamera != null)
                {
                    playerCamera.gameObject.SetActive(false);
                }
            }
        }

        public override void OnStartAuthority()
        {
            _rigidbody.isKinematic = false;
        }

        public override void OnStopAuthority()
        {
            _rigidbody.isKinematic = true;
        }

        private void FixedUpdate()
        {
            if (!isOwned)
            {
                return;
            }

            ApplyChannels(_inputSource.ReadChannels());
        }

        public void ApplyChannels(DroneInputChannels channels)
        {
            IsArmed = channels.IsArmed;

            if (channels.SelfRight)
            {
                SelfRight();
            }

            if (!IsArmed)
            {
                return;
            }

            float thrustForce = DroneFlightMath.ComputeThrustForce(
                channels.Throttle, _rigidbody.mass, Mathf.Abs(Physics.gravity.y), thrustToWeightRatio);
            _rigidbody.AddForce(transform.up * thrustForce, ForceMode.Force);

            // Local axes: rotation around X = pitch, Y = yaw, Z = roll.
            Vector3 targetLocalAngularVelocity = Mathf.Deg2Rad * new Vector3(
                channels.Pitch * maxPitchRateDegPerSec,
                channels.Yaw * maxYawRateDegPerSec,
                channels.Roll * maxRollRateDegPerSec);

            Vector3 currentLocalAngularVelocity = transform.InverseTransformDirection(_rigidbody.angularVelocity);

            Vector3 angularAccel = DroneFlightMath.ComputeRateTorqueAccel(
                currentLocalAngularVelocity,
                targetLocalAngularVelocity,
                Vector3.one * rateGain,
                Mathf.Deg2Rad * maxAngularAccelDegPerSec2);

            _rigidbody.AddRelativeTorque(angularAccel, ForceMode.Acceleration);
        }

        // Keeps current yaw heading (don't spin the pilot around), zeroes pitch/roll,
        // and lifts slightly so the frame doesn't stay clipped into whatever it landed on.
        private void SelfRight()
        {
            _rigidbody.angularVelocity = Vector3.zero;
            _rigidbody.linearVelocity = Vector3.zero;
            _rigidbody.MoveRotation(Quaternion.Euler(0f, transform.eulerAngles.y, 0f));
            _rigidbody.MovePosition(_rigidbody.position + Vector3.up * selfRightLiftMeters);
        }
    }
}
