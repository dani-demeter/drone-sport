using UnityEngine;

namespace DroneSport.Drone
{
    public static class DroneFlightMath
    {
        public static float ShapeExpo(float input, float expo)
        {
            input = Mathf.Clamp(input, -1f, 1f);
            return expo * (input * input * input) + (1f - expo) * input;
        }

        public static float ShapeExpoUnipolar(float input, float expo)
        {
            input = Mathf.Clamp01(input);
            return expo * (input * input * input) + (1f - expo) * input;
        }

        public static float ComputeThrustForce(float throttle01, float mass, float gravityMagnitude, float thrustToWeightRatio)
        {
            return Mathf.Clamp01(throttle01) * mass * gravityMagnitude * thrustToWeightRatio;
        }

        public static Vector3 ComputeRateTorqueAccel(
            Vector3 currentLocalAngularVelocity,
            Vector3 targetLocalAngularVelocity,
            Vector3 gainPerAxis,
            float maxAngularAccel)
        {
            Vector3 error = targetLocalAngularVelocity - currentLocalAngularVelocity;
            Vector3 accel = Vector3.Scale(error, gainPerAxis);
            return Vector3.ClampMagnitude(accel, maxAngularAccel);
        }
    }
}
