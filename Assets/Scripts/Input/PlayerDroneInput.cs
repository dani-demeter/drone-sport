using DroneSport.Drone;
using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;

namespace DroneSport.Input
{
    public class PlayerDroneInput : NetworkBehaviour, IDroneInputSource
    {
        [Header("Expo (stick feel shaping: 0 = linear, 1 = full cubic)")]
        [SerializeField, Range(0f, 1f)] private float throttleExpo = 0f;
        [SerializeField, Range(0f, 1f)] private float rollExpo = 0.3f;
        [SerializeField, Range(0f, 1f)] private float pitchExpo = 0.3f;
        [SerializeField, Range(0f, 1f)] private float yawExpo = 0.2f;

        private DroneControlsActions _actions;
        private bool _isArmed;
        private bool _selfRightRequested;

        public bool IsArmed => _isArmed || _actions.Drone.ArmSwitch.IsPressed();

        private void Awake()
        {
            _actions = new DroneControlsActions();
        }

        public override void OnStartAuthority()
        {
            _actions.Drone.Enable();
            _actions.Drone.Arm.performed += OnArmPerformed;
            _actions.Drone.SelfRight.performed += OnSelfRightPerformed;
        }

        public override void OnStopAuthority()
        {
            _actions.Drone.Arm.performed -= OnArmPerformed;
            _actions.Drone.SelfRight.performed -= OnSelfRightPerformed;
            _actions.Drone.Disable();
        }

        private void OnArmPerformed(InputAction.CallbackContext context)
        {
            _isArmed = !_isArmed;
        }

        private void OnSelfRightPerformed(InputAction.CallbackContext context)
        {
            _selfRightRequested = true;
        }

        public DroneInputChannels ReadChannels()
        {
            float throttle = DroneFlightMath.ShapeExpoUnipolar(_actions.Drone.Throttle.ReadValue<float>(), throttleExpo);
            float roll = DroneFlightMath.ShapeExpo(_actions.Drone.Roll.ReadValue<float>(), rollExpo);
            float pitch = DroneFlightMath.ShapeExpo(_actions.Drone.Pitch.ReadValue<float>(), pitchExpo);
            float yaw = DroneFlightMath.ShapeExpo(_actions.Drone.Yaw.ReadValue<float>(), yawExpo);

            bool selfRight = _selfRightRequested;
            _selfRightRequested = false;

            return new DroneInputChannels(throttle, roll, pitch, yaw, IsArmed, selfRight);
        }
    }
}
