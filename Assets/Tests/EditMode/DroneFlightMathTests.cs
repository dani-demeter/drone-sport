using DroneSport.Drone;
using NUnit.Framework;
using UnityEngine;

namespace DroneSport.Tests.EditMode
{
    public class DroneFlightMathTests
    {
        [Test]
        public void ShapeExpo_ZeroExpo_IsIdentity()
        {
            Assert.AreEqual(0.5f, DroneFlightMath.ShapeExpo(0.5f, 0f), 1e-5f);
            Assert.AreEqual(-0.7f, DroneFlightMath.ShapeExpo(-0.7f, 0f), 1e-5f);
        }

        [Test]
        public void ShapeExpo_PreservesSignAndEndpoints()
        {
            Assert.AreEqual(1f, DroneFlightMath.ShapeExpo(1f, 0.5f), 1e-5f);
            Assert.AreEqual(-1f, DroneFlightMath.ShapeExpo(-1f, 0.5f), 1e-5f);
            Assert.Greater(DroneFlightMath.ShapeExpo(0.5f, 0.5f), 0f);
            Assert.Less(DroneFlightMath.ShapeExpo(-0.5f, 0.5f), 0f);
        }

        [Test]
        public void ShapeExpo_IsMonotonicallyIncreasing()
        {
            float prevValue = DroneFlightMath.ShapeExpo(-1f, 0.4f);
            for (float x = -0.9f; x <= 1f; x += 0.1f)
            {
                float value = DroneFlightMath.ShapeExpo(x, 0.4f);
                Assert.Greater(value, prevValue);
                prevValue = value;
            }
        }

        [Test]
        public void ShapeExpoUnipolar_ZeroExpo_IsIdentity()
        {
            Assert.AreEqual(0.5f, DroneFlightMath.ShapeExpoUnipolar(0.5f, 0f), 1e-5f);
            Assert.AreEqual(0f, DroneFlightMath.ShapeExpoUnipolar(0f, 0f), 1e-5f);
        }

        [Test]
        public void ShapeExpoUnipolar_PreservesEndpoints()
        {
            Assert.AreEqual(0f, DroneFlightMath.ShapeExpoUnipolar(0f, 0.5f), 1e-5f);
            Assert.AreEqual(1f, DroneFlightMath.ShapeExpoUnipolar(1f, 0.5f), 1e-5f);
        }

        [Test]
        public void ShapeExpoUnipolar_ClampsToUnitRange()
        {
            Assert.AreEqual(0f, DroneFlightMath.ShapeExpoUnipolar(-0.5f, 0.5f), 1e-5f);
            Assert.AreEqual(1f, DroneFlightMath.ShapeExpoUnipolar(1.5f, 0.5f), 1e-5f);
        }

        [Test]
        public void ShapeExpoUnipolar_IsMonotonicallyIncreasing()
        {
            float prevValue = DroneFlightMath.ShapeExpoUnipolar(0f, 0.4f);
            for (float x = 0.1f; x <= 1f; x += 0.1f)
            {
                float value = DroneFlightMath.ShapeExpoUnipolar(x, 0.4f);
                Assert.Greater(value, prevValue);
                prevValue = value;
            }
        }

        [Test]
        public void ComputeThrustForce_AtHoverThrottle_BalancesGravity()
        {
            const float mass = 0.5f;
            const float gravity = 9.81f;
            const float twr = 2.5f;
            float hoverThrottle = 1f / twr;

            float force = DroneFlightMath.ComputeThrustForce(hoverThrottle, mass, gravity, twr);

            Assert.AreEqual(mass * gravity, force, 1e-4f);
        }

        [Test]
        public void ComputeThrustForce_ClampsThrottleToUnitRange()
        {
            float forceAboveOne = DroneFlightMath.ComputeThrustForce(2f, 1f, 9.81f, 2f);
            float forceAtOne = DroneFlightMath.ComputeThrustForce(1f, 1f, 9.81f, 2f);
            Assert.AreEqual(forceAtOne, forceAboveOne, 1e-4f);

            float forceBelowZero = DroneFlightMath.ComputeThrustForce(-1f, 1f, 9.81f, 2f);
            Assert.AreEqual(0f, forceBelowZero, 1e-4f);
        }

        [Test]
        public void ComputeRateTorqueAccel_ZeroErrorGivesZeroOutput()
        {
            Vector3 current = new Vector3(1f, 2f, 3f);
            Vector3 result = DroneFlightMath.ComputeRateTorqueAccel(current, current, Vector3.one * 10f, 100f);
            Assert.AreEqual(Vector3.zero, result);
        }

        [Test]
        public void ComputeRateTorqueAccel_PointsTowardTarget()
        {
            Vector3 current = Vector3.zero;
            Vector3 target = new Vector3(1f, 0f, 0f);
            Vector3 result = DroneFlightMath.ComputeRateTorqueAccel(current, target, Vector3.one * 10f, 1000f);

            Assert.Greater(result.x, 0f);
            Assert.AreEqual(0f, result.y, 1e-5f);
            Assert.AreEqual(0f, result.z, 1e-5f);
        }

        [Test]
        public void ComputeRateTorqueAccel_ClampsToMaxMagnitude()
        {
            Vector3 current = Vector3.zero;
            Vector3 target = new Vector3(100f, 0f, 0f);
            const float maxAccel = 5f;

            Vector3 result = DroneFlightMath.ComputeRateTorqueAccel(current, target, Vector3.one * 1000f, maxAccel);

            Assert.AreEqual(maxAccel, result.magnitude, 1e-3f);
        }
    }
}
